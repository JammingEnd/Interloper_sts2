# Dark Potential — Threshold Markers & Energy Rewards

Date: 2026-09-13
Status: Approved design (pending implementation plan)
Builds on: `docs/superpowers/specs/2026-09-13-dark-potential-design.md`

## Overview

Add hoverable threshold markers to the Dark Potential arc, and a second set of thresholds that grant energy. Markers use `Interloper/images/ui/combat/dark_potential/potential_marker.png` (48×48, directional).

## Section 1 — State & data

`DarkPotentialState` gains:
- `AutoGrantEnergy` (bool, default **false**).
- `LastGrantedEnergyIndex` (int, **initialized to −1**; tracks the highest energy threshold already granted this fill cycle; reset to −1 on clear).

Energy thresholds as percentages (scale with `Max`), declared as **static readonly arrays on `DarkPotentialCmd`** (beside the private `Levels` instance, not on the per-combat state instance):
- `EnergyThresholds = [10, 30, 60, 100]` (%)
- `EnergyValues = [1, 2, 3, 4]`

Threshold integer math: `threshold = Max * pct / 100` (same convention as `Max * i / LevelCount`). The tooltip and the grant must use the same formula.

## Section 2 — Commands & energy logic

- `DarkPotentialCmd.SetAutoEnergy(PlayerChoiceContext choiceContext, Player player, bool value)` — toggles `AutoGrantEnergy` (for a future card/power/relic), fires `OnChanged`. When turning auto **on** mid-cycle, **re-seed** `LastGrantedEnergyIndex` to the highest threshold currently crossed at `Current` — do NOT retroactively grant.
- `DarkPotentialCmd.Add` — after clamping `Current`, **if auto mode**: for each energy threshold newly crossed this fill cycle, grant its value via `PlayerCmd.GainEnergy(value, player)` (each crossing is its own grant; reaching 100% has granted 1+2+3+4 = 10 total). **Advance `LastGrantedEnergyIndex` BEFORE the first `await`** of any energy grant, and before `OnChanged` / `DarkPotentialHook.OnGained`, so a hook-triggered re-entrant `Add` cannot double-grant.
- `DarkPotentialCmd.Clear` — capture `Current` before reset; after level dispatch + reset, **if clear mode** (default, `AutoGrantEnergy == false`): grant the value of the highest energy threshold the captured current reached (clear at 45% → 30% reached → +2 energy). Always reset `LastGrantedEnergyIndex` to −1.
- `DarkPotentialCmd.SetMax` — after re-clamping, **re-seed** `LastGrantedEnergyIndex` to the highest threshold crossed at the new `Max` (prevents both retroactive grants when raising and double-grants when lowering).
- Energy grants run inside the action pipeline (a `PlayerChoiceContext` is available in `Add`/`Clear`/`SetAutoEnergy`).

## Section 3 — UI markers

- `NDarkPotentialBar` spawns **5 level markers** at 20/40/60/80/100%:
  - Position = `center + radius · (cos θ, sin θ)`, θ = `ArcStartAngle + ArcSweep · p` (same formula as the fill). Radius = `ArcRadius` (50).
  - **Rotation:** the marker art (`potential_marker.png`) is a vertical dart whose point is at the **top** of the image (point axis = Godot **−Y**). Level markers rotate to `Rotation = θ + π/2`, so the art's point aligns with the radial outward direction `(cos θ, sin θ)`. Each marker Control must set `PivotOffset = Size · 0.5` so rotation spins about its own center.
  - Uses `potential_marker.png`, default tint, scaled to fit the arc (~16–20px).
- **4 energy markers** at 10/30/60/100%:
  - Same angle formula, but **radius = `ArcRadius − 12`** (inside the arc) so the 60/100 overlaps stay visible.
  - **Rotation = `θ + π/2 + π`** (i.e. the level marker rotation + π) → they point **radially inward** (toward the center).
  - **Tinted gold** to distinguish from level markers.
- Each marker is its own small child `Control` (hittable, `MouseFilter.Stop`) with `MouseEntered`/`MouseExited` → shows/hides its tooltip.
- Marker angular positions are fixed (percentages), so they do not move when `Max` changes; only their tooltip values recompute.

## Section 4 — Tooltips

- **Level marker hover** → `Level i: threshold/max — <GetDescription(i)>` where `threshold = Max · i / LevelCount` (recomputes if `Max` changes).
- **Energy marker hover** → via a new loc key in `static_hover_tips.json`, e.g. `"INTERLOPER-DARK_POTENTIAL.energy": "{Percent}% — +{Value} Energy"`.
- **Hovering the bar itself (not a marker) shows no tooltip.** The existing whole-bar summary tooltip is removed; remove `OnBarHovered`/`OnBarUnhovered` and the `MouseEntered`/`MouseExited` connections (keep `MouseFilter.Stop`). The now-unused `INTERLOPER-DARK_POTENTIAL.title`/`.description` keys are removed from loc.

## Notes

- Auto mode changes only *when energy is granted*; level activation on clear is unchanged (button always clears + dispatches the highest reached level).
- `PlayerCmd.GainEnergy(amount, player)` is the energy API (used elsewhere in the mod).
- Thresholds are percentages of `Max`, so the mutable max automatically rescales them.
