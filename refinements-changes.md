# Refinements & Changes Log

## Day 1 – Planning
- Set up Unity project
- Connected Cursor to project
- Created documentation files using AI
- Used AI to generate player controller
- Modified movement and added interaction key

---

## Day 2 – Development
- Implemented `ReparingObjects`: hold `F` within `repairDistance` to repair over time and swap to `fixedObjectPrefab` (fixed vehicle forced upright).
- Added optional repair progress UI (standard `Slider` or auto-spawn world-space `Canvas` + `Slider` prefab; script finds `Slider` + TMP label inside prefab).
- Repair UI: “Repair” prompt via TextMeshPro; world-space bar locked above object (renderer bounds); full UI root hidden when repair completes so slider/text vanish on fix.
- Implemented `ThirdPersonCameraFollow`: camera follows behind player at an offset, with mouse-controlled yaw/pitch (clamped pitch).
- Updated `PlayerController` to support sprinting: hold `LeftShift` to increase movement speed.
- Implemented health system: added `Health` component (max/current HP + on-death event).
- Implemented `HazardZone`: hazard zone that damages the player over time.
- Updated `HazardZone` to be distance-based (radius check) so it no longer requires trigger colliders.
- Updated `PlayerController` to disable movement on `Health` death.
- Designed level for game, added prefabs in from Unity Asset store.
- Placed `HazardZone` instances across the level map to add challenge (ongoing damage when player is within radius).
- Added `PlayerHealthUI` to drive screen health bar from `Health`.
- Generated AI images for UI slider art (health bar and repair progress slider) and applied them in the project.

---

## Day 3 – Refinement
- Styled repair prompt: customised TMP font and colour in the world-space repair UI prefab.
- Added main menu / pause / death flow scripts (`MainMenu`, `PauseMenu`, `DeathScreen`) and supporting editor script to start from `MainMenu`.
- Implemented scoring system: `ScoreManager` (singleton), cleaning awards `+1`, repairing awards seconds spent repairing (1 point per second by default).
- Added TMP score UI (`ScoreUI`) to display the current score on the canvas.
- Added level countdown timer (`LevelTimer`): starts at 5 minutes, freezes on pause, turns red at 1 minute, and triggers death at 0.
- Updated death screen to display final score (TMP text) when player dies.
- Added `MusicPlayer` for background music with optional fade-out/fade-in looping to mask MP3 loop gaps.