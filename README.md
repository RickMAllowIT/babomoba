# Project BaboMOBA — MVP Phase 1 Sandbox

Twin-stick MOBA greybox prototype. Built in Unity 2022.3 LTS.

## Quick Start

1. **Install Unity Hub** — download from https://unity.com/download
2. **Install Unity Editor** — version 2022.3.52f1 (or latest 2022.3 LTS) via Unity Hub
3. **Open this project** — Unity Hub → Add → select this folder
4. **Build the scene** — in Unity Editor: `Tools → BaboMOBA → Build Scene`
5. **Bake NavMesh** — `Window → AI → Navigation → Bake` (minions need this for pathfinding)
6. **Hit Play** — test with a gamepad

## Controller Mapping

| Input | Function |
|-------|----------|
| Left Stick | Movement |
| Right Stick | Manual aim |
| R2 (Right Trigger) | Fire manual projectile |
| R3 (Right Stick Click) | Toggle auto-stream on/off |
| L2 (Left Trigger) | Dash (4s cooldown) |

## Architecture

All entities are coloured Unity primitives:

| Entity | Shape | Color | Tag |
|--------|-------|-------|-----|
| Player | Sphere | Blue | — |
| Minion | Sphere (small) | Grey | `Minion` |
| Tower | Cylinder | Yellow | `Tower` |
| Core | Cube (large) | Green | `Core` |
| Enemy Dummy | Cube | Red | `Enemy` |

## Slices Implemented

| # | Slice | Script(s) |
|---|-------|-----------|
| 1 | Scaffold | `SceneBuilder.cs` (Editor), `manifest.json` |
| 2 | Player movement | `PlayerMovement.cs`, `CameraFollow.cs` |
| 3 | Right stick aim | `PlayerAim.cs` |
| 4 | Manual fire | `ManualFire.cs`, `Projectile.cs` |
| 5 | Minion spawn + NavMesh | `MinionSpawner.cs`, `Minion.cs` |
| 6 | Minion HP + death | `Health.cs` |
| 7 | Auto-stream baseline | `AutoStream.cs` |
| 8 | R3 toggle | `AutoStreamToggle.cs` |
| 9 | Harvester filter | `HarvesterFilter.cs` |
| 10 | Auto-stream suspension | `AutoStreamSuspension.cs` |
| 11 | Dash | `Dash.cs` |
| 12 | Tower placement + targeting | `TowerTargeting.cs` |
| 13 | Tower firing | `TowerFiring.cs` |
| 14 | Tower destruction | `TowerDestruction.cs` |
| 15 | Core | `Core.cs` |
| 16 | Game over | `GameOver.cs` |
| 17 | Enemy dummy (static) | `EnemyDummy.cs` |
| 18 | Dummy patrol | `DummyPatrol.cs` |
| 19 | Target highlight | `TargetHighlight.cs` |
| 20 | Laser beam | `LaserBeam.cs` |
| 21 | Demolition mode | `DemolitionMode.cs`, `TargetingMode.cs` |

## Input

`Assets/InputActions.json` — defines the Input System action map (Move, Aim, Fire, Dash, ToggleAutoStream).

Import this via Unity's Input System package once the project is open.
