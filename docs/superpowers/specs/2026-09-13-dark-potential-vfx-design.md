# Dark Potential — Level Marker Fire VFX

Date: 2026-09-13
Status: Approved design (pending implementation plan)
Builds on: `docs/superpowers/specs/2026-09-13-dark-potential-markers-design.md`

## Overview

Show a persistent fire VFX on each of the 5 level (potential) markers while its level threshold is reached. The bar already refreshes on `DarkPotentialCmd.OnChanged`, so the flames are purely UI-derived from existing state — no gameplay/command changes.

## Approach

UI-driven persistent flames in `NDarkPotentialBar`:
- Pre-instantiate one `dark_fire_vfx.tscn` instance per level marker, as a **child of the marker `TextureRect`** (inherits the marker's rotation), hidden by default.
- On every `OnChanged` (Add/Clear/SetMax/SetAutoEnergy), recompute per marker: flame visible iff `Current >= threshold_i` where `threshold_i = Max · i / LevelCount`. Clear resets Current → all flames hide; re-filling re-lights them at their thresholds. At level 5, all 5 markers burn.
- Flames only on the 5 **potential** markers — never on the 4 energy markers.

## Placement

- Scene: `Interloper/scenes/othervfx/dark_fire_vfx.tscn` (root `Node2D`, child `TextureRect` 40×40, shader-based looping fire). Load once via `PreloadManager.Cache.GetScene("othervfx/dark_fire_vfx.tscn".ScenePath()).Instantiate<Node2D>()`.
- Marker-local anchor: flame centered on the marker at **60% of the marker's height** — `Position = (MarkerSize·0.5 − 20, MarkerSize·0.6 − 20)` so the 40×40 flame centers at (15, 18) on a 30px marker.
- Scale: tunable `FlameScale` constant (default 1.0 — flame slightly larger than the 30px marker). If changed, it also affects the anchor math.
- The flame node is a child of the rotated marker, so it follows the marker's outward/inward orientation.

## Refresh wiring

- `OnDarkPotentialChanged(player)` (already filters to the local player) additionally calls a `RefreshFlames()` that toggles `Visible` on each flame based on the threshold check. No new events or commands.

## Notes

- The shader loops on TIME, so toggling `Visible` restarts the effect — acceptable.
- Flame nodes are created once in `Initialize`'s `_levelMarkers.Count == 0` guard alongside the markers, so re-Activate doesn't stack them.