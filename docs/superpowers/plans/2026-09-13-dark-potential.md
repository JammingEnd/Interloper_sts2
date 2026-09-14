# Dark Potential — Glyph System Rework Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the glyph card/sequence system with a "Dark Potential" resource: a per-player flat integer (max 100) shown as an arc progress bar, cleared via a button to activate the highest reached level's effect.

**Architecture:** Mirrors the existing glyph architecture — per-player state on `PlayerCombatState` via `SpireField`, mutation only through a static command class (`DarkPotentialCmd`), a level-methods singleton with auto-computed thresholds, and a `Control`-based arc UI + networked clear action. Migrate all glyph dependents to Potential FIRST, then delete the glyph core.

**Tech Stack:** C# / Godot 4.5 / STS2 modding (BaseLib, Harmony). No unit test framework — verification is `dotnet build` staying green.

**Spec:** `docs/superpowers/specs/2026-09-13-dark-potential-design.md`

---

## Task 1: DarkPotentialState + SpireField + extensions

**Files:**
- Create: `InterloperCode/Potential/DarkPotentialState.cs`
- Create: `InterloperCode/Potential/DarkPotentialField.cs`
- Create: `InterloperCode/Potential/PlayerCombatStateDarkPotentialExtension.cs`
- Modify: `InterloperCode/Patches/PlayerCombatStateConstructorPatch.cs`

- [ ] **Step 1:** Create `DarkPotentialState` (plain class, `Current` int, `Max` int = 100).
- [ ] **Step 2:** Create `DarkPotentialField` with `SpireField<PlayerCombatState, DarkPotentialState> State = new(() => null)`.
- [ ] **Step 3:** Create extension methods `GetDarkPotentialState` / `GetDarkPotential` (0 when null) / `GetDarkPotentialMax` (100 when null) / `GetDarkPotentialProgress` (Current/Max clamped 0..1).
- [ ] **Step 4:** In `PlayerCombatStateConstructorPatch`, add `DarkPotentialField.State[__instance] = new DarkPotentialState();` (keep the existing `GlyphField.Queue[__instance] = ...` line until Task 7).
- [ ] **Step 5:** Build → 0 errors. Commit.

## Task 2: DarkPotentialLevels (5 empty methods + thresholds)

**Files:**
- Create: `InterloperCode/Potential/DarkPotentialLevels.cs`

- [ ] **Step 1:** Create `DarkPotentialLevels` with `const int LevelCount = 5`, `virtual Task Level1..Level5(PlayerChoiceContext ctx, Player player)` all returning `Task.CompletedTask`, and `virtual string GetDescription(int level) => ""`.
- [ ] **Step 2:** Build → 0 errors. Commit.

## Task 3: DarkPotentialCmd + hooks + events

**Files:**
- Create: `InterloperCode/Potential/IAfterDarkPotentialCleared.cs`, `IOnDarkPotentialGained.cs`, `DarkPotentialHook.cs`, `DarkPotentialCmd.cs`

- [ ] **Step 1:** Create the two hook interfaces.
- [ ] **Step 2:** Create `DarkPotentialHook` (mirror `GlyphHook`: iterate `combatState.IterateHookListeners().OfType<T>()`, `PushModel`/`InvokeExecutionFinished`/`PopModel`).
- [ ] **Step 3:** Create `DarkPotentialCmd` with `static event Action<Player>? OnChanged`, `Add(ctx, player, amount)` (clamp to Max, fire OnChanged + `DarkPotentialHook.OnGained`), `Clear(ctx, player)` (find highest reached level via `threshold_i = Max * i / LevelCount`, dispatch, reset Current, fire OnChanged + `DarkPotentialHook.AfterCleared`), `SetMax(ctx, player, value)` (re-clamp Current). Guard `CombatManager.Instance.IsOverOrEnding`.
- [ ] **Step 4:** Build → 0 errors. Commit.

## Task 4: NDarkPotentialBar UI + clear button

**Files:**
- Create: `InterloperCode/Nodes/NDarkPotentialBar.cs`
- Create: `InterloperCode/Field/DarkPotentialNode.cs`
- Create: `InterloperCode/Patches/NCombatUiDarkPotentialPatch.cs`
- Research: how the base game enqueues networked player actions (End Turn button / `EndPlayerTurnAction`), `ActionQueueSynchronizer.RequestEnqueue(GameAction)`

- [ ] **Step 1 (discovery spike):** Find how the base game enqueues a networked action from a UI button. Inspect the End Turn button handler and `EndPlayerTurnAction`; confirm `GameAction` subclassing + `ActionQueueSynchronizer.RequestEnqueue`. Record the pattern in a comment in the patch.
- [ ] **Step 2:** Create `NDarkPotentialBar : Control` — `_Draw` arc (DrawArc + QueueRedraw), fill = `_player.PlayerCombatState?.GetDarkPotentialProgress()`. Subscribe `DarkPotentialCmd.OnChanged` in `_EnterTree` (only `QueueRedraw` for the local player), unsubscribe in `_ExitTree`. Add hover via `NHoverTipSet` showing level thresholds + `DarkPotentialLevels.GetDescription(level)`.
- [ ] **Step 3:** Create `DarkPotentialNode` (`AddedNode<NCombatUi, NDarkPotentialBar>`).
- [ ] **Step 4:** Create `NCombatUiDarkPotentialPatch` (postfix on `NCombatUi.Activate`): if local character is Interloper, attach the bar and a clear `Button` beside the energy counter. The button's press enqueues a `GameAction` (from step 1) that calls `DarkPotentialCmd.Clear` inside the deterministic pipeline. If the action API blocks, implement it and confirm it compiles; do NOT fall back to a non-deterministic direct call.
- [ ] **Step 5:** Build → 0 errors. Commit.

## Task 5: Migrate generators + token-consumers to Potential

**Files (modify each):**
- `Cards/Common/DeepGaze.cs`, `BorderOfnothing.cs`, `OnesService.cs`, `OutwardStrength.cs`, `Sideye.cs`, `focus.cs`
- `Cards/Uncommon/ToTheDepths.cs`, `Cards/Rare/DistordPlane.cs`
- `Powers/ByStringsPower.cs`, `Powers/AddedBenefitPower.cs` (NOTE: its `AfterCardChangedPiles` has no `PlayerChoiceContext`; obtain a context via the `PainfulRenewalPower` dev-console precedent OR add a context-free `DarkPotentialCmd.Add` overload that captures the combat state — pick one and document)
- `Potions/DarkInsightPotion.cs`
- Token-consumers: `Cards/Uncommon/Consume.cs`, `Cards/Rare/Goodwill.cs`, `Cards/Rare/WideOpen.cs`, `Cards/Uncommon/Intervention.cs`, `Cards/Uncommon/Shhhh.cs`, `Cards/Uncommon/PrayersHeard.cs`, `Cards/Uncommon/LongEnd.cs`

- [ ] **Step 1:** Add `DarkPotential` keyword to `InterloperKeywords` + `INTERLOPER-DARK_POTENTIAL.*` in `card_keywords.json` (before cards reference it).
- [ ] **Step 2:** For each generator, replace glyph-token creation with `DarkPotentialCmd.Add(choiceContext, player, X)` (start 5–10; exact per-card chosen during implementation). DarkInsightPotion grants a fixed Potential (e.g., 15).
- [ ] **Step 3:** Rework token-consumers (Goodwill, Consume, WideOpen, Intervention, Shhhh, PrayersHeard, LongEnd) to grant/bonus Potential or remove.
- [ ] **Step 4:** Update each card's loc to "Gain Potential X".
- [ ] **Step 5:** Build → 0 errors. Commit.

## Task 6: Migrate consumers

**Files (modify each):**
- Implement `IAfterDarkPotentialCleared`: `Powers/WitnessMePower.cs`, `Cards/Rare/Absolute.cs`, `Relics/OldInscriptionsRelic.cs`
- Implement `IOnDarkPotentialGained`: `Powers/ReactiveChainsPower.cs`
- Rework/remove glyph-counting consumers: `Cards/Rare/DarkDivinity.cs`, `Cards/Rare/Flail.cs`, `Powers/CompletelyLostPower.cs`, `Relics/GreatestGlyphRelic.cs`, `Powers/TouchOfFaithPower.cs`
- Fix hover tip: `Cards/Rare/WitnessMe.cs` (uses `InterloperKeywords.Sequence` → new Dark Potential keyword + loc)
- Remove stray usings: `Cards/Uncommon/Injection.cs`, `Cards/Uncommon/LittleControl.cs`, `Powers/NeverAgainPower.cs` (each `using Interloper.InterloperCode.Cards.Glyph;`)

- [ ] **Step 1:** Implement `IAfterDarkPotentialCleared` on WitnessMe (damage on clear), Absolute (buff on clear), OldInscriptions (extra clear effect). Implement `IOnDarkPotentialGained` on ReactiveChains (strength on gain).
- [ ] **Step 2:** Rework/remove glyph-counting consumers (DarkDivinity, Flail, CompletelyLost, Goodwill, Consume, GreatestGlyph, TouchOfFaithPower).
- [ ] **Step 3:** Fix `WitnessMe.cs` hover tip; remove the 3 stray `using ...Cards.Glyph;`.
- [ ] **Step 4:** Build → 0 errors. Commit.

## Task 7: Remove the glyph core

**Files (delete):**
- `InterloperCode/Glyphs/*`
- `InterloperCode/Cards/Glyph/*` (cards), `InterloperCode/Cards/GlyphCard.cs`
- `InterloperCode/Field/GlyphNode.cs`, `InterloperCode/Nodes/NGlyphArch.cs`, `InterloperCode/Utils/GlyphResource.cs`
- `InterloperCode/Patches/NCombatUiGlyphArchPatch.cs`
- `Interloper/scenes/Glyphs/*`, `Interloper/sound/glyph_queue.ogg`, `Interloper/sound/sequence.ogg`, glyph images

**Files (modify):**
- `InterloperCode/Keywords/InterloperKeywords.cs` — remove `Glyph`/`Sequence` keywords (keep `DarkPotential`)
- `InterloperCode/Entries/CombatVarTracker.cs` — remove `totalGlypsPlayedInCombat` + related
- `InterloperCode/Entries/RunVarTracker.cs` — remove `totalSequencesPlayedInRun` + related
- `InterloperCode/Helpers/Extentions.cs` — remove `GetSequenceByIndex`
- `InterloperCode/Helpers/LocHelper.cs` — remove glyph logic
- `InterloperCode/Patches/PlayerCombatStateConstructorPatch.cs` — remove the `GlyphField.Queue[...] = ...` line
- Assets: delete `images/ui/combat/glyphs/*`, `images/powers/glyph_{eye,mouth,tail}_power.png`, orphaned `images/powers/glyph_storage.png`; keep `greatest_glyph_relic.png` if the relic is reworked. Loc: remove `Glyph`/`Sequence` entries and glyph hover-tip entries

- [ ] **Step 1:** Delete the glyph files.
- [ ] **Step 2:** Remove keyword, tracker, helper, LocHelper, and patch-line glyph remnants.
- [ ] **Step 3:** Build → 0 errors. Commit.

## Task 8: Final build + cleanup

- [ ] **Step 1:** `dotnet build Interloper.csproj` → 0 errors.
- [ ] **Step 2:** `rg -il "glyph|sequence" InterloperCode` → empty (also catches GlyphField, GlyphHook, GlyphNode, NGlyphArch, GlyphResource, totalGlypsPlayedInCombat, totalSequencesPlayedInRun, GetSequenceByIndex, InterloperKeywords.Glyph/Sequence).
- [ ] **Step 3:** Remove leftover glyph loc entries / assets; confirm no `Glyph`/`Sequence` keywords remain.
- [ ] **Step 4:** Commit.