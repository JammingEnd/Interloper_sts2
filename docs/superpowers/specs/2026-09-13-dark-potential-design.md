# Dark Potential — Rework of the Glyph System

Date: 2026-09-13
Status: Approved design (pending implementation plan)

## Overview

Replace the glyph card/sequence system with a **Dark Potential** resource: a flat integer that accumulates on the player and is represented as a progress bar (arc) in the combat UI. Clearing it (via a button beside the energy counter) activates the highest reached level's effect and resets the bar. Cards that previously granted glyphs now grant flat "Potential" values.

## Section 1 — State & data model

- `DarkPotentialState` stored on `PlayerCombatState` via a `SpireField` (mirrors `GlyphField`).
- Initialized in the existing `PlayerCombatState` constructor patch.
- Fields:
  - `Current` (int)
  - `Max` (int, default **100**)
- Extension methods on `PlayerCombatState`:
  - `GetDarkPotential()` → `Current`
  - `GetDarkPotentialMax()` → `Max`
  - `GetDarkPotentialProgress()` → `Current / Max`, clamped to 0..1 (for the bar)

## Section 2 — Levels & thresholds

- `DarkPotentialLevels` class with **5 empty `virtual Task` methods**: `Level1` … `Level5`. The user fills them in.
- Thresholds auto-computed from method count: `threshold_i = Max * i / LevelCount` → 20%, 40%, 60%, 80%, 100% of max for 5 levels.
- `Clear` behavior: find the highest `i` where `Current >= threshold_i`, call that level's method, then reset `Current = 0`.

## Section 3 — Command layer

- `DarkPotentialCmd.Add(ctx, player, amount)` — increments `Current`, clamped to `Max`.
- `DarkPotentialCmd.Clear(ctx, player)` — dispatch highest-reached-level method + full reset.
- `DarkPotentialCmd.SetMax(player, value)` — for a rare card that raises/lowers `Max` (re-clamps `Current`).

## Section 4 — UI

- `NDarkPotentialBar : Control` — arc progress bar drawn with `_Draw` (arc `_draw` technique), fill = `GetDarkPotentialProgress()`.
- A **clear button** beside the energy counter (`NCombatUi` patch) that calls `DarkPotentialCmd.Clear`.
- `NGlyphArch`, the arch placements/highlights, and glyph VFX/sounds removed/replaced by the bar.
- Bar hover tooltip shows the level thresholds and what each level does.

## Section 5 — Cards

- Cards that granted glyphs now grant "Potential X" and call `DarkPotentialCmd.Add`.
- A "Dark Potential" keyword/tooltip on those cards.

## Section 6 — Migration & removal

**Remove:**
- GlyphEye/Mouth/Tail cards, `GlyphCard`, glyph powers, glyph models/queue/field/hook/cmd/sequence, `NGlyphArch`, `GlyphNode`, `LocHelper` glyph logic, glyph VFX/scenes/sounds.

**Migrate generators** (DeepGaze, BorderOfnothing, OnesService, OutwardStrength, Sideye, focus, ByStrings, AddedBenefit, etc.) to grant Potential.

**Migrate consumers:**
- WitnessMe → damage on Potential clear.
- Absolute → buff on Potential clear.
- OldInscriptions → extra effect on clear.
- GreatestGlyph → interacts with Max/clear.
- ReactiveChains → reacts to Potential gain.
- CompletelyLost, DarkDivinity, Flail, Goodwill, Consume, ToTheDepths, etc. → reworked or removed.

## Notes

- Multiplayer-safe: state per-player on `PlayerCombatState`, mutations only via `DarkPotentialCmd` (awaited inside the action pipeline).
- The 5 level methods are intentionally empty for the user to fill in.
- `Max` is mutable to support a rare card that changes it.