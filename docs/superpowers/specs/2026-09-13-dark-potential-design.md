# Dark Potential — Rework of the Glyph System

Date: 2026-09-13
Status: Approved design (pending implementation plan)

## Overview

Replace the glyph card/sequence system with a **Dark Potential** resource: a flat integer that accumulates on the player and is represented as a progress bar (arc) in the combat UI. Clearing it (via a button beside the energy counter) activates the highest reached level's effect and resets the bar. Cards that previously granted glyphs now grant flat "Potential" values.

## Section 1 — State & data model

- `DarkPotentialState` stored on `PlayerCombatState` via a `SpireField` (mirrors `GlyphField`), initialized in the existing `PlayerCombatState` constructor patch.
- Fields:
  - `Current` (int)
  - `Max` (int, default **100**)
- Extension methods on `PlayerCombatState`:
  - `GetDarkPotential()` → `Current`, or `0` if state is null
  - `GetDarkPotentialMax()` → `Max`, or `100` if state is null
  - `GetDarkPotentialProgress()` → `Current / Max`, clamped to 0..1 (for the bar)

## Section 2 — Levels & thresholds

- `DarkPotentialLevels` is a **concrete singleton** with `public const int LevelCount = 5` and **5 empty `public virtual Task` methods**:
  `Level1(PlayerChoiceContext ctx, Player player)` … `Level5(...)`. The user fills them in. Each method receives the context and player so it can run combat actions (draw, `PowerCmd.Apply`, `CreatureCmd.Damage`, etc.).
- Thresholds auto-computed from the count at clear time: `threshold_i = Max * i / LevelCount` → 20%, 40%, 60%, 80%, 100% of `Max`.
- `Clear` behavior: find the highest `i` where `Current >= threshold_i`, call `DarkPotentialLevels.Level_i(ctx, player)`, then reset `Current = 0`.
- For the bar tooltip, `DarkPotentialLevels.GetDescription(int level)` is a `virtual string` (empty for now; user fills alongside the methods), plus a localization key per level.

## Section 3 — Command layer

- `DarkPotentialCmd.Add(ctx, player, amount)` — increments `Current`, clamped to `Max`. Fires the "gained" hook + `OnChanged` event.
- `DarkPotentialCmd.Clear(ctx, player)` — dispatch highest-reached-level method + full reset. Fires the "cleared" hook + `OnChanged`.
- `DarkPotentialCmd.SetMax(ctx, player, value)` — for a rare card that raises/lowers `Max`; clamps `Current` to the new max and fires `OnChanged`. Takes `ctx` so it runs inside the action pipeline like the others.

## Section 4 — UI

- `NDarkPotentialBar : Control` — arc progress bar drawn with `_Draw` (`DrawArc` + `QueueRedraw`), fill = `GetDarkPotentialProgress()`. Reads live `Max`/`Current` each frame, so max changes update immediately. (First use of `DrawArc` in this codebase; existing UI is `TextureRect`-based — this is the intended new control.)
- A **clear button** beside the energy counter (`NCombatUi` patch). Clicking it enqueues a **custom networked player action** (a `GameAction` that runs in the deterministic action pipeline with a `PlayerChoiceContext`), which calls `DarkPotentialCmd.Clear`. This keeps the clear deterministic and multiplayer-safe; it is not a raw UI callback.
- Bar hover tooltip shows the level thresholds and `DarkPotentialLevels.GetDescription(level)` for each level.
- `NGlyphArch`, the arch placements/highlights, and glyph VFX/sounds removed/replaced by the bar. The UI subscribes to `DarkPotentialCmd.OnChanged` to refresh (replacing the old `GlyphCmd.OnSequenceActivated` subscription).

## Section 5 — Cards

- Cards that granted glyphs now grant "Potential X" and call `DarkPotentialCmd.Add(ctx, player, X)`.
- A "Dark Potential" keyword/tooltip on those cards.
- A "Potential X" DynamicVar/keyword on the card text.

## Section 6 — Hooks

- `IAfterDarkPotentialCleared` — dispatched by `DarkPotentialHook.AfterCleared(combatState, ctx, player, level)` via `combatState.IterateHookListeners()` (replaces `IAfterSequenceActivated`).
- `IOnDarkPotentialGained` — dispatched by `DarkPotentialHook.OnGained(combatState, player, amount)`.
- `DarkPotentialCmd.OnChanged` (static event) for the UI bar, replacing `GlyphCmd.OnSequenceActivated`.
- `IAfterDarkPotentialMaxChanged` — for max-changing effects, if needed.

## Section 7 — Migration & removal

**Remove:**
- GlyphEye/Mouth/Tail cards, `GlyphCard`, glyph powers, glyph models/queue/field/hook/cmd/sequence (`GlyphCmd`, `GlyphHook`, `GlyphQueue`, `GlyphField`, `GlyphModel`, `GlyphSequence`, `IAfterSequenceActivated`), `NGlyphArch`, `GlyphNode`, `LocHelper` glyph logic, glyph VFX/scenes/sounds.
- `Glyph` / `Sequence` keywords (`InterloperKeywords.cs`) and their loc.
- `CombatVarTracker` glyph/sequence tracking (`totalGlypsPlayedInCombat` and related).

**Migrate generators** (each card that created a glyph token now grants Potential instead):
- DeepGaze, BorderOfnothing, OnesService, OutwardStrength, Sideye, focus, ToTheDepths, ByStringsPower, AddedBenefitPower — each gains a "Potential X" value (values chosen during implementation; default small values such as 5–10).
- Cards that consumed glyph tokens for effects (Consume, Goodwill, WideOpen, Intervention, Shhhh, PrayersHeard, LongEnd) — reworked to grant/bonus Potential or removed.

**Migrate consumers:**
- WitnessMe → damage to all enemies on `AfterDarkPotentialCleared` (scaled by power amount).
- Absolute → permanent damage buff on `AfterDarkPotentialCleared`.
- OldInscriptions → extra effect on `AfterDarkPotentialCleared`.
- GreatestGlyph → interacts with `Max`/clear (e.g., when potential is cleared at high value, consume half corruption).
- ReactiveChains → grants Strength on `OnDarkPotentialGained`.
- CompletelyLost → reworked (no longer counts glyph cards in exhaust).
- DarkDivinity → no longer counts glyph cards in exhaust; reworked to scale off Dark Potential or removed.
- Flail → no longer counts GlyphTail in exhaust; reworked or removed.
- TouchOfFaithPower → no longer uses `GlyphCmd.Activate`/`GlyphCard`; reworked or removed.

## Notes

- Multiplayer-safe: state per-player on `PlayerCombatState`, mutations only via `DarkPotentialCmd` (awaited inside the action pipeline, or via the networked clear action).
- The 5 level methods + `GetDescription` are intentionally empty for the user to fill in.
- `Max` is mutable; the bar and thresholds read it live, so lowering `Max` clamps `Current` and crossing a now-below-current threshold does nothing until the next clear.