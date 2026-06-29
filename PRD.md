# PRD: Project BaboMOBA — MVP Phase 1

## Problem Statement

The team needs to validate that a twin-stick MOBA control scheme — combining Vampire Survivors-style auto-stream PvE farming with manual right-stick PvP aiming — feels intuitive and engaging on a gamepad. Before committing to full MOBA systems (networking, multiple heroes, abilities), we need a greybox sandbox that tests pure controller ergonomics.

## Solution

A single-player Unity sandbox with primitives only, one HotS-style lane, and a character that can auto-stream farm minions, manually aim at a patrolling enemy dummy, dash to dodge tower projectiles, and defend a destructible tower and core. No win condition beyond the core death clock — restart Play mode to iterate.

## User Stories

1. As a player, I want to move my character with the left stick, so that I can navigate the lane freely.
2. As a player, I want to aim manually with the right stick and fire with R2, so that I can shoot a specific target with precision.
3. As a player, I want my auto-stream projectiles to fire automatically at minions without holding a trigger, so that I can focus on positioning and dodging.
4. As a player, I want to toggle auto-stream on and off with R3, so that I can control when I'm passively farming.
5. As a player, I want auto-stream to suspend when I manually aim, so that I can switch cleanly between farming and precision shooting.
6. As a player, I want auto-stream to never target the enemy dummy, so that PvP always requires manual aim.
7. As a player, I want a visual highlight on whichever entity auto-stream is locked onto, so that I can tell at a glance what I'm about to shoot.
8. As a player, I want a laser beam or line renderer drawn from my character when manually aiming, so that I can see my projectile trajectory.
9. As a player, I want to dash with L2 on a short cooldown, so that I can evade incoming projectiles.
10. As a player, I want minions to spawn every 30 seconds and march down the lane toward my core, so that I have a steady stream of PvE targets to farm.
11. As a player, I want my tower to shoot the closest minion (or the enemy dummy if no minions are in range), so that I'm not overwhelmed by minion waves.
12. As a player, I want minions to attack and eventually destroy my tower, so that I feel pressure to farm them before they reach it.
13. As a player, I want minions to path to my core after the tower falls, so that the lane state progresses meaningfully.
14. As a player, I want the sandbox to end (game over) when my core is destroyed, so that there's a terminal consequence to poor defense.
15. As a player, I want the enemy dummy to patrol near the lane, so that I can test tracking aim on a moving PvP target.
16. As a developer, I want the dash cooldown to be character-configurable, so that future builds can tune mobility per hero.
17. As a developer, I want auto-stream to be a character property (some characters may not have it), so that hero design space isn't constrained.
18. As a developer, I want targeting mode (Harvester vs Demolition) to be a fixed character property in MVP, not a runtime-swappable stance, so that scope stays minimal.
19. As a developer, I want the D-Pad reserved for future ability slots, so that the controller mapping doesn't need to be redesigned later.
20. As a developer, I want all entities rendered as coloured primitives (spheres, cubes, cylinders), so that art doesn't slow down controller-feel iteration.

## Implementation Decisions

### Engine

Unity (to be installed). Rationale: built-in New Input System for gamepad mapping, NavMesh for minion pathfinding, and the TopDown Engine asset for rapid twin-stick prototyping.

### Auto-Stream System

A timer-driven projectile emitter that fires automatically at the entity selected by the character's Targeting Mode. PvE-only — the targeting system ignores the Enemy Dummy. Suspended while the right stick is active (manual aim mode). Toggled via R3 (Right Stick Click).

### Targeting Modes

Fixed per character. Two modes exist for MVP:

- **Harvester**: auto-stream targets the lowest-health minion in range.
- **Demolition**: auto-stream targets the nearest structure (tower, then core) in range.

These are NOT runtime-swappable via D-Pad in MVP. A character ships with one targeting mode.

### Controller Mapping

| Input | Function |
|-------|----------|
| Left Stick | Movement (physics-based translation) |
| Right Stick | Manual aim (rotation + projectile direction override) |
| R2 (Right Trigger) | Fire manual projectile |
| R3 (Right Stick Click) | Toggle auto-stream on/off |
| L2 (Left Trigger) | Dash (4s cooldown, one body-length distance) |
| D-Pad | Reserved for future abilities (unbound in MVP) |

### Tower Behavior (HotS Rules)

The tower scans for targets within its radius:
1. If any minion is in range → fire at the closest minion.
2. If no minions are in range but the Enemy Dummy is → fire at the Enemy Dummy.
3. Otherwise → idle.

Tower projectiles are dodgeable. The tower has HP and is destructible by minion attacks and the player's Demolition-mode projectiles.

### Lane Structure

A flat plane with:
- **Minion spawn point** at the far end
- **One tower** (yellow cylinder) at the midpoint
- **Core** (large cube) at the player's end
- **NavMesh baked** onto the plane so minions path from spawn → tower → core

When the tower breaks (HP = 0), minions re-path directly to the core. When the core breaks, the sandbox ends.

### Enemy Dummy Patrol

A red cube with a simple patrol script: moves between two waypoints near the lane at low speed. High health pool — not intended to die during testing. Used to test manual right-stick + R2 aiming.

### Visual Feedback

- **Target highlight**: a coloured glow, ring, or outline on the entity auto-stream is locked onto.
- **Laser beam**: a line renderer from the player to the manual-aim reticle, shown only while right stick is active.

## Testing Decisions

### Success Criteria (from original PRD)

1. **Ergonomic check**: The player can comfortably navigate a lane, shoot minions, and dodge an incoming projectile without taking their thumbs off the sticks.
2. **PvP aim**: The player can stand near the Enemy Dummy and land manual shots on it using right stick + R2.
3. **Readability**: The target highlight clearly indicates which entity the auto-fire system is currently locked onto.

### What Makes a Good Test

Only test external behaviour — controller feel, visual clarity, and system responsiveness. Do not test internal implementation details. The sandbox IS the test harness.

### Test Approach

Manual playtesting only for MVP. No automated tests. Iterate on cooldowns, projectile speed, minion wave size, and tower HP based on feel. Each playtest session runs until the core is destroyed (or you're satisfied with the controls) — restart and tune.

## Out of Scope

- Networking/multiplayer
- Audio
- Multiple characters or builds (single fixed character for MVP)
- Runtime stance-swapping via D-Pad
- Stance stat changes (damage modifiers, speed changes)
- Abilities/spell slots (D-Pad reserved but unbound)
- UI/HUD beyond the target highlight and laser beam
- Win state or match timer — only core destruction ends the sandbox
- Art beyond coloured primitives
- Full HotS lane structure (gates, forts, keeps, multiple lanes)
- Enemy heroes beyond the patrolling dummy
