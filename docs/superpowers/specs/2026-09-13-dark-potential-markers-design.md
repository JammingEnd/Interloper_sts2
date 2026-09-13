# Dark Potential — Threshold Markers & Energy Rewards

Date: 2026-09-13
Status: Approved design (pending implementation plan)
Builds on: `docs/superpowers/specs/2026-09-13-dark-potential-design.md`

## Overview

Add hoverable threshold markers to the Dark Potential arc, and a second set of thresholds that grant energy. Markers use `Interloper/images/ui/combat/dark_potential/potential_marker.png` (48×48, directional).

## Section 1 — State & data

`DarkPotentialState` gains:
- `AutoGrantEnergy` (bool, default **false**).
- `LastGrantedEnergyIndex` (int, tracks the highest energy threshold already granted this fill cycle; reset to −1 on clear).
- Energy thresholds as percentages (scale with `Max`): `EnergyThresholds = [10, 30, 60, 100]` (%) → `EnergyValues = [1, 2, 3, 4]` (static readonly arrays).

## Section 2 — Commands & energy logic

- `DarkPotentialCmd.SetAutoEnergy(player, bool)` — toggles `AutoGrantEnergy` (for a future card/power/relic), fires `OnChanged`.
- `DarkPotentialCmd.Add` — after clamping `Current`, **if auto mode**: for each energy threshold newly crossed this fill cycle, grant its value via `PlayerCmd.GainEnergy(value, player)` (each crossing is its own grant; reaching 100% has granted 1+2+3+4 = 10 total), then advance `LastGrantedEnergyIndex`.
- `DarkPotentialCmd.Clear` — capture `Current` before reset; after level dispatch + reset, **if clear mode** (default, `AutoGrantEnergy == false`): grant the value of the highest energy threshold the captured current reached (clear at 45% → 30% reached → +2 energy). Always reset `LastGrantedEnergyIndex` to −1.
- Energy grants run inside the action pipeline (a `PlayerChoiceContext` is available in `Add`/`Clear`).

## Section 3 — UI markers

- `NDarkPotentialBar` spawns **5 level markers** at 20/40/60/80/100%:
  - Position = `center + radius · (cos θ, sin θ)`, θ = `ArcStartAngle + ArcSweep · p` (same formula as the fill).
  - Rotated to point **radially outward** from the center, so they follow the curve.
  - Uses `potential_marker.png`, default tint, scaled to fit the arc (~16–20px).
- **4 energy markers** at 10/30/60/100%:
  - Same angle/position formula, but at a **slightly different radius** so the 60/100 overlaps stay visible.
  - Rotated to point **radially inward** (toward the center) — 180° from the level markers.
  - **Tinted gold** to distinguish from level markers.
- Each marker is its own small child `Control` (hittable, `MouseFilter.Stop`) with `MouseEntered`/`MouseExited` → shows/hides its tooltip.
- Marker angular positions are fixed (percentages), so they do not move when `Max` changes; only their tooltip values recompute.

## Section 4 — Tooltips

- **Level marker hover** → `Level i: threshold/max — <GetDescription(i)>` where `threshold = Max · i / LevelCount` (recomputes if `Max` changes).
- **Energy marker hover** → `M% — +N Energy` (e.g. "30% — +2 Energy"), via loc keys in `static_hover_tips.json`.
- **Hovering the bar itself (not a marker) shows no tooltip.** The existing whole-bar summary tooltip is removed; tooltips are shown exclusively by the markers.

## Notes

- Auto mode changes only *when energy is granted*; level activation on clear is unchanged (button always clears + dispatches the highest reached level).
- `PlayerCmd.GainEnergy(amount, player)` is the energy API (used elsewhere in the mod).
- Thresholds are percentages of `Max`, so the mutable max automatically rescales them.
