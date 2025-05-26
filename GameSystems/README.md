# GameSystems Directory

This directory contains the core scripts responsible for managing game systems such as checkpoints, combat, and overall game system coordination in the project.

## Script Overview

### 1. `CheckPoint.cs`
Represents a single checkpoint in the game world. When the player collides with a checkpoint, it:
- Notifies the `CheckPointSystem` to activate the next checkpoint.
- Disables itself to prevent further triggers.

**Key Methods:**
- `SetCheckPointSystem(CheckPointSystem)`: Assigns the checkpoint system this checkpoint belongs to.
- `OnTriggerEnter(Collider)`: Handles player collision logic for checkpoint activation.

---

### 2. `CheckPointSystem.cs`
Manages all checkpoints in the level and handles player respawn logic. It:
- Keeps track of the current active checkpoint.
- Moves the player to the correct checkpoint on respawn.
- Advances to the next checkpoint when notified by a `CheckPoint`.

**Key Methods:**
- `Awake()`: Initializes all checkpoints and assigns this system to each.
- `Start()`: Respawns the player at the current checkpoint at the start of the level.
- `NextCheckPoint()`: Advances to the next checkpoint in the sequence.
- `Respawn()`: Moves the player to the active checkpoint and handles invincibility/dash state.

---

### 3. `CombatSystem.cs`
A `ScriptableObject` that manages player combat, combos, attack logic, and special states. It:
- Handles weak and heavy attacks, combo chaining, and attack timing.
- Manages hit detection via punch colliders.
- Controls special combat states such as "Karnage" mode, including timers and point decay.
- Provides ground smash attack logic and camera shake effects.

**Key Methods:**
- `OnStart(BoxCollider, BoxCollider)`: Initializes combat variables and references.
- `OnUpdate()`: Updates combo timers, player states, and Karnage logic each frame.
- `CombatInput(CombatQueueElement)`: Processes player input for attacks and combos.
- `BeginAttack()`, `NextAttack()`, `EndAttack()`, `ResetCombat()`, `AllowAttack()`: Manage the lifecycle of attacks and combos.
- `ActivateLeftCollider()`, `ActivateRightCollider()`, `DeactivateLeftCollider()`, `DeactivateRightCollider()`: Enable/disable hit detection colliders.
- `SmashGround(bool)`: Performs a ground smash attack, applies camera shake, and damages nearby enemies.
- `KarnageTimer()`: Manages Karnage mode timers and point decay.

**Other Types:**
- `CombatQueueElement`: Struct representing an attack in the combat queue.
- `AttackType`: Enum for weak/heavy attack types.

---

### 4. `GameSystemsController.cs`
Coordinates the initialization and updating of core game systems, especially the combat system. It:
- Sets up references to player colliders, animator, and transform for the combat system.
- Calls the combat system's update method each frame.

**Key Methods:**
- `Start()`: Passes references to the combat system and initializes it.
- `Update()`: Calls the combat system's update method every frame.

---

## Usage Notes
- The checkpoint system ensures smooth player progression and respawning.
- The combat system is modular and can be extended for additional attack types or special abilities.
- `GameSystemsController` should be attached to a central game object to coordinate system initialization.

For further details, refer to the comments and summaries within each script.
