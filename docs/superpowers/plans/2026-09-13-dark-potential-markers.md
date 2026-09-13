# Dark Potential Threshold Markers & Energy Rewards — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add hoverable threshold markers to the Dark Potential arc (5 outward level markers + 4 gold inward energy markers) and a second set of energy thresholds (10/30/60/100% → +1/2/3/4 energy), grantable either automatically on crossing or on clear via a per-player toggle.

**Architecture:** Extends the existing Dark Potential system. State gains an `AutoGrantEnergy` bool + crossed-threshold tracking; `DarkPotentialCmd` gains the threshold arrays, a `SetAutoEnergy` command, and energy grants in `Add`/`Clear`/`SetMax`; `NDarkPotentialBar` spawns hoverable marker child Controls using `potential_marker.png`. Build-green is the verification (no test framework).

**Tech Stack:** C# / Godot 4.5 / STS2 modding (BaseLib, Harmony).

**Spec:** `docs/superpowers/specs/2026-09-13-dark-potential-markers-design.md`

---

## Task 1: State fields + energy arrays + SetAutoEnergy

**Files:**
- Modify: `InterloperCode/Potential/DarkPotentialState.cs`
- Modify: `InterloperCode/Potential/DarkPotentialCmd.cs`

- [ ] **Step 1:** Add the two fields to `DarkPotentialState`:

```csharp
public class DarkPotentialState
{
    public int Current { get; set; }
    public int Max { get; set; } = 100;
    public bool AutoGrantEnergy { get; set; }
    public int LastGrantedEnergyIndex { get; set; } = -1;
}
```

- [ ] **Step 2:** Add to `DarkPotentialCmd` (beside the `Levels` instance): the threshold arrays, `SetAutoEnergy`, and the private helpers:

```csharp
public static readonly int[] EnergyThresholds = { 10, 30, 60, 100 };
public static readonly int[] EnergyValues = { 1, 2, 3, 4 };

/// <summary>Auto grants energy when the bar crosses energy thresholds; otherwise energy is granted on clear.</summary>
/// NOTE: this performs no awaits; return `Task.CompletedTask` (NOT an async method) to avoid CS1998.
public static Task SetAutoEnergy(PlayerChoiceContext choiceContext, Player player, bool value)
{
    var state = player.PlayerCombatState?.GetDarkPotentialState();
    if (state == null)
        return;

    if (CombatManager.Instance.IsOverOrEnding)
        return;

    state.AutoGrantEnergy = value;
    ReSeedEnergyIndex(state);
    OnChanged?.Invoke(player);
    return Task.CompletedTask;
}

private static int GetEnergyThreshold(DarkPotentialState state, int index)
    => state.Max * EnergyThresholds[index] / 100;

private static int GetHighestReachedEnergyIndex(DarkPotentialState state)
{
    var index = -1;
    for (var i = 0; i < EnergyThresholds.Length; i++)
    {
        if (state.Current >= GetEnergyThreshold(state, i))
            index = i;
    }
    return index;
}

private static void ReSeedEnergyIndex(DarkPotentialState state)
    => state.LastGrantedEnergyIndex = GetHighestReachedEnergyIndex(state);
```

Add `using MegaCrit.Sts2.Core.GameActions.Multiplayer;` if not already present (it is). Note: `PlayerChoiceContext` must be imported.

- [ ] **Step 3:** Build → 0 errors. Commit:

```bash
git add InterloperCode/Potential/DarkPotentialState.cs InterloperCode/Potential/DarkPotentialCmd.cs
git commit -m "feat(potential): AutoGrantEnergy state + SetAutoEnergy command + energy thresholds"
```

## Task 2: Energy grants in Add/Clear/SetMax

**Files:**
- Modify: `InterloperCode/Potential/DarkPotentialCmd.cs`

- [ ] **Step 1:** Modify `Add` to grant auto energy (advancing the index BEFORE any await so a re-entrant hook can't double-grant):

```csharp
state.Current += actual;

// Auto mode: grant energy for each newly-crossed threshold. The index advances before the
// first await so a hook-triggered re-entrant Add cannot double-grant.
if (state.AutoGrantEnergy)
    await GrantAutoEnergy(choiceContext, player, state);

OnChanged?.Invoke(player);
```

- [ ] **Step 2:** Add `GrantAutoEnergy`:

```csharp
private static async Task GrantAutoEnergy(PlayerChoiceContext choiceContext, Player player, DarkPotentialState state)
{
    // Re-read the index each iteration: a re-entrant Add during GainEnergy may advance it.
    for (var i = state.LastGrantedEnergyIndex + 1; i < EnergyThresholds.Length; i = state.LastGrantedEnergyIndex + 1)
    {
        if (state.Current < GetEnergyThreshold(state, i))
            break;

        state.LastGrantedEnergyIndex = i;
        await PlayerCmd.GainEnergy(EnergyValues[i], player);
    }
}
```

Add `using MegaCrit.Sts2.Core.Commands;` (or wherever `PlayerCmd` resolves — verify the existing import in e.g. `InterloperCode/Cards/Rare/WideOpen.cs` which uses `PlayerCmd.GainEnergy`).

- [ ] **Step 3:** Modify `Clear` — grant the highest energy threshold's value in clear mode using the PRE-reset current, then reset:

```csharp
// Capture BEFORE the dispatch await so a level effect that mutates Current can't skew the grant.
var level = GetReachedLevel(state);
var energyIndex = !state.AutoGrantEnergy ? GetHighestReachedEnergyIndex(state) : -1;
if (level > 0)
    await DispatchLevel(choiceContext, player, level);

// Clear mode (default): pay the highest energy threshold the pre-reset current reached.
if (energyIndex >= 0)
    await PlayerCmd.GainEnergy(EnergyValues[energyIndex], player);

state.Current = 0;
state.LastGrantedEnergyIndex = -1;
OnChanged?.Invoke(player);

// KEEP the existing AfterCleared dispatch (was lines 54-58 of the current file) — it fires
// IAfterDarkPotentialCleared on Absolute/WitnessMePower/OldInscriptionsRelic/GreatestGlyphRelic:
var combatState = player.Creature.CombatState;
if (combatState != null && level > 0)
    await DarkPotentialHook.AfterCleared(combatState, choiceContext, player, level);
```

- [ ] **Step 4:** Modify `SetMax` — re-seed the energy index after re-clamping:

```csharp
state.Max = Math.Max(1, value);
state.Current = Math.Min(state.Current, state.Max);
ReSeedEnergyIndex(state);
OnChanged?.Invoke(player);
```

- [ ] **Step 5:** Build → 0 errors. Commit:

```bash
git add InterloperCode/Potential/DarkPotentialCmd.cs
git commit -m "feat(potential): energy grants on auto-crossing and on clear"
```

## Task 3: Marker child controls in NDarkPotentialBar

**Files:**
- Modify: `InterloperCode/Nodes/NDarkPotentialBar.cs`
- Research: how the mod loads textures (e.g. `GD.Load<Texture2D>`) — check an existing node for the pattern.

- [ ] **Step 1:** Load the marker texture once (static field). Use the mod's texture-loading convention (see `InterloperCode/Character/InterloperCardPool.cs`), not `GD.Load`:

```csharp
private static readonly Texture2D MarkerTexture =
    PreloadManager.Cache.GetTexture2D("ui/combat/dark_potential/potential_marker.png".ImagePath());
```

Add `using Interloper.InterloperCode.Extensions;` (for `ImagePath`) and `using BaseLib.Abstracts;` (for `PreloadManager` — see `InterloperCardPool.cs`).

- [ ] **Step 2:** In `Initialize(Player player)` (after setting `_player` and `QueueRedraw`), spawn the 9 markers — but only if not already spawned (guard `if (GetChildCount() == 0)`, since `Initialize` may be called on re-Activate): Add a `CreateMarker` helper:

```csharp
private TextureRect CreateMarker(double progress, Color tint, bool pointInward)
{
    var marker = new TextureRect
    {
        Texture = MarkerTexture,
        Size = new Vector2(20f, 20f),
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
        StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        Modulate = tint,
        MouseFilter = MouseFilterEnum.Stop
    };

    marker.PivotOffset = marker.Size * 0.5f;

    float angle = ArcStartAngle + ArcSweep * (float)progress;
    float radius = pointInward ? ArcRadius - 12f : ArcRadius;
    var arcPoint = Size * 0.5f + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    marker.Position = arcPoint - marker.Size * 0.5f;
    // Art points up (-Y); level markers rotate to theta+90deg (outward), energy +180deg more (inward).
    marker.Rotation = angle + Mathf.Pi / 2f + (pointInward ? Mathf.Pi : 0f);

    AddChild(marker);
    return marker;
}
```

- [ ] **Step 3:** Call `CreateMarker` for the 5 level thresholds and 4 energy thresholds. **Use float division** — `int/int` collapses to 0 and stacks all markers at the arc start: level progress = `i / (float)DarkPotentialLevels.LevelCount` for i = 1..5, energy progress = `DarkPotentialCmd.EnergyThresholds[i] / 100f` for i = 0..3. Level markers: default tint = `Colors.White`. Energy markers: gold tint = `new Color(1f, 0.84f, 0f)`, pointInward: true. Store the created level markers and energy markers in lists so Task 4 can attach tooltips.

- [ ] **Step 4:** Build → 0 errors. Commit:

```bash
git add InterloperCode/Nodes/NDarkPotentialBar.cs
git commit -m "feat(potential): threshold markers on the arc"
```

## Task 4: Marker tooltips + remove whole-bar hover + loc

**Files:**
- Modify: `InterloperCode/Nodes/NDarkPotentialBar.cs`
- Modify: `Interloper/localization/eng/static_hover_tips.json`

- [ ] **Step 1:** In `static_hover_tips.json`, remove `INTERLOPER-DARK_POTENTIAL.title` and `INTERLOPER-DARK_POTENTIAL.description`; add:

```json
"INTERLOPER-DARK_POTENTIAL.energy": "{Percent}% — +{Value} Energy",
"INTERLOPER-DARK_POTENTIAL.levelTitle": "Level {Level}",
"INTERLOPER-DARK_POTENTIAL.level": "{Threshold} / {Max}{Description}"
```

(`Description` is an optional var — empty, or `" — <GetDescription text>"`). These keys are REQUIRED: the loc-key analyzer (STS001) errors on any `LocString` key missing from the JSON.

- [ ] **Step 2:** Attach hover to each level marker (in `Initialize`, INSIDE the `GetChildCount() == 0` guard, right after the markers are created — so re-Activate doesn't stack duplicate `MouseEntered` handlers):

```csharp
private void AttachLevelHover(TextureRect marker, int level)
{
    marker.MouseEntered += () => ShowLevelTip(marker, level);
    marker.MouseExited += () => NHoverTipSet.Remove(marker);
}

private void ShowLevelTip(TextureRect marker, int level)
{
    if (_player == null) return;
    int max = _player.PlayerCombatState?.GetDarkPotentialMax() ?? 100;
    int threshold = max * level / DarkPotentialLevels.LevelCount;
    string description = Levels.GetDescription(level);

    var title = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.levelTitle");
    title.Add("Level", level);
    var body = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.level");
    body.Add("Threshold", threshold);
    body.Add("Max", max);
    body.Add("Description", string.IsNullOrWhiteSpace(description) ? "" : $" — {description}");

    var hoverTip = new HoverTip(title, body);
    var set = NHoverTipSet.CreateAndShow(marker, hoverTip, HoverTip.GetHoverTipAlignment(marker));
    set?.SetExtraFollowOffset(new Vector2(20f, -20f));
    set?.SetFollowOwner();
}
```

NOTE: keep `GetDescription` for when the user fills levels. `BuildLevelsText` is removed in Step 4.

- [ ] **Step 3:** Attach hover to each energy marker (same `GetChildCount() == 0` guard scope):

```csharp
private void AttachEnergyHover(TextureRect marker, int index)
{
    marker.MouseEntered += () => ShowEnergyTip(marker, index);
    marker.MouseExited += () => NHoverTipSet.Remove(marker);
}

private void ShowEnergyTip(TextureRect marker, int index)
{
    var loc = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.energy");
    loc.Add("Percent", DarkPotentialCmd.EnergyThresholds[index]);
    loc.Add("Value", DarkPotentialCmd.EnergyValues[index]);
    var hoverTip = new HoverTip(loc, loc);
    var set = NHoverTipSet.CreateAndShow(marker, hoverTip, HoverTip.GetHoverTipAlignment(marker));
    set?.SetExtraFollowOffset(new Vector2(20f, -20f));
    set?.SetFollowOwner();
}
```

(Verify `HoverTip` constructor signatures against the old `OnBarHovered` in this file / the deleted NGlyphArch — match the exact shape used before.)

- [ ] **Step 4:** Remove the whole-bar hover: delete `OnBarHovered`, `OnBarUnhovered`, `BuildLevelsText`, and the `MouseEntered`/`MouseExited` connections in `_Ready` (keep `MouseFilter = MouseFilterEnum.Stop`).

- [ ] **Step 5:** Build → 0 errors (loc-key analyzer validates keys). Commit:

```bash
git add InterloperCode/Nodes/NDarkPotentialBar.cs Interloper/localization/eng/static_hover_tips.json
git commit -m "feat(potential): marker tooltips, remove whole-bar hover"
```

## Task 5: Final verification

- [ ] **Step 1:** `dotnet build Interloper.csproj` → 0 errors.
- [ ] **Step 2:** `rg -n "OnBarHovered|OnBarUnhovered|BuildLevelsText|INTERLOPER-DARK_POTENTIAL.description" InterloperCode Interloper/localization` → empty (all removed).
- [ ] **Step 3:** Confirm the energy marker loc key exists and the auto-grant fields are wired (`rg -n "AutoGrantEnergy|LastGrantedEnergyIndex|SetAutoEnergy" InterloperCode`).
- [ ] **Step 4:** Commit if any leftover cleanup remains.