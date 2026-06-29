# Project BaboMOBA — Domain Context

> **What this is**: A glossary for the domain language.
> **What this is NOT**: A technical spec, scratch pad, or implementation guide.

## Key Terms

| Term | Definition |
|------|-----------|
| **Auto-Stream** | A Vampire Survivors-style passive projectile spray that fires automatically on a timer. PvE-only — never targets enemy heroes. Character-specific; some characters may not have it. Toggled on/off via R3 (Right Stick Click). Suspended while manually aiming. |
| **Manual Aim** | Right stick + R2 firing mode. The player directly controls projectile direction. Always required for PvP. Suspends auto-stream while active. |
| **Targeting Mode** | A fixed character property determining which entity type auto-stream prioritizes. Harvester = minions. Demolition = structures. Not a runtime-swappable stance in MVP. |
| **Target Highlight** | A visual glow, ring, or outline on the entity currently locked by auto-stream. Distinct from the laser beam used for manual/spell projectiles. |
| **Laser Beam** | A line renderer drawn from the player to the target, used for manual aim trajectory and spell visuals. Not used for lock-on indication. |
| **Dash** | A short-cooldown evade on L2 (Left Trigger). Character-configurable; MVP default is 4-second cooldown, one body-length distance. |
| **Minions** | Small grey spheres that spawn in waves and march down the lane toward the enemy core. Attack structures in their path. Hostile to the player. |
| **Tower** | A friendly yellow cylinder structure flanking the lane. Follows Heroes of the Storm targeting: attacks minions first, then enemy hero if no minions in range. Destructible by minions and the player. Stays dead once destroyed. |
| **Core** | The player's base structure at the end of the lane. Minions path to it after the tower is destroyed. When the core is destroyed, the sandbox ends (game over — restart Play mode). |
| **Enemy Dummy** | A red cube that patrols in a small area near the lane. Stand-in for a future enemy hero. Used to test manual PvP aim. High health pool, stationary origin with basic patrol path. |
| **Lane** | The path minions follow from spawn point → tower → core. NavMesh-driven pathfinding. |
