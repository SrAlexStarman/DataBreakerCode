# MainCharacter/Actions Directory

This directory contains the core scripts responsible for managing the player's actions, such as movement, jumping, attacking, dashing, grinding, grappling, and more.

## Script Overview

### 1. `PlayerActions.cs`
Abstract base `ScriptableObject` for all player action types. Provides a structure for input and action effect logic, and references to player and camera data containers.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Abstract method for handling input effects (to be implemented by subclasses).
- `ActionEffect()`: Abstract method for executing the action's effect (to be implemented by subclasses).

---

### 2. `Action_Movement.cs`
Handles player movement input and logic, including walking, running, and air movement. Converts input to world direction, manages speed, state transitions, and updates animation and sound.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Processes movement input and aligns direction with camera.
- `ActionEffect()`: Handles grounded, air, and animation-driven movement logic.
- `CheckState()`: Updates the player's movement state (stopped, moving, running).

---

### 3. `Action_Jump.cs`
Handles jump input and logic, including jumping from the ground or while grinding rails. Manages jump force, animation triggers, and special logic for jumping off rails.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles input for jumping, allowing jumps when grounded or grinding.
- `JumpOffRail()`: Handles jumping off a rail.
- `Jump()`: Handles a standard jump from the ground.
- `ActionEffect()`: Executes the effect of the jump action (extendable).

---

### 4. `Action_Sprint.cs`
Handles sprint input, toggling the player's sprint state based on input events.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Sets or unsets the sprinting flag when input starts or is canceled.
- `ActionEffect()`: Executes the effect of the sprint action (extendable).

---

### 5. `Action_Dash.cs`
Manages the player's dash ability, including dash movement, cooldowns, collision detection, and visual/sound effects. Integrates with the combat system to reset combos and manage invincibility during dashes.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles dash input, direction, and obstacle detection.
- `ActionEffect()`: Handles dash movement, cooldown, and end logic.
- `ShowPlayer()`: Toggles player visibility and invincibility during dash.
- `Dash(Vector3, bool)`: Starts dash movement toward the endpoint.
- `EndDash()`: Ends the dash, restores state and velocity.

---

### 6. `Action_Grind.cs`
Handles the player's rail grinding action, managing movement along splines, state transitions, and regaining control after leaving a rail. Supports both looping and non-looping rails.

**Key Methods:**
- `ActionEffect()`: Updates grinding logic and movement along the rail.
- `InputEffect(InputAction.CallbackContext)`: Handles input while grinding (currently unused).
- `GrindNonLoop()`: Handles grinding on non-looping rails.
- `GrindLoop()`: Handles grinding on looping rails.
- `Grind()`: Performs movement and animation updates for grinding.
- `RegainControls()`: Restores player control and applies launch velocity after leaving the rail.

---

### 7. `Action_Grap.cs`
Implements the player's grappling hook ability, including targeting, hand movement, and starting a dash when attached to a grappling point. Finds the best grappling point based on screen position, distance, and visibility.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles input for starting and canceling the grappling action.
- `ActionEffect()`: Updates grappling logic, hand movement, and dash initiation.
- `TargetGrappling()`: Finds and selects the best grappling point to target.

---

### 8. `Action_WeakAttack.cs`
Handles weak attack input, creating and sending attack events to the combat system.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles input for performing a weak attack, creating a combat event.
- `ActionEffect()`: Executes the effect of the weak attack (extendable).

---

### 9. `Action_HeavyAttack.cs`
Handles heavy attack input, creating and sending attack events to the combat system.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles input for performing a heavy attack, creating a combat event.
- `ActionEffect()`: Executes the effect of the heavy attack (extendable).

---

### 10. `PunchCollision.cs`
Handles punch collision detection, applying damage and effects to enemies and breakable objects. Spawns punch particle effects, applies force, triggers sound, and disables itself after a successful hit.

**Key Methods:**
- `OnTriggerEnter(Collider)`: Handles collision with enemies and breakable objects, applies damage, force, effects, and disables itself after a successful hit.

---

## Attacks Subfolder

This subfolder contains specialized scripts for handling different types of player attacks, including base logic and specific implementations for weak and heavy attacks.

### 11. `Attacks/PlayerAttacks.cs`
Abstract base `ScriptableObject` for all player attack types. Handles general attack logic, dash-to-enemy logic, and animation triggers.

**Key Methods:**
- `TryAttack(KillEnemy enemy, Transform playerTrans, Animator anim)`: Attempts to perform an attack on the specified enemy. If out of range but within dash distance, dashes to the enemy first; otherwise, performs a normal attack and triggers the animation.
- `AttackAction(Animator anim)`: Virtual method to trigger the attack animation. Can be overridden for specific attack types. Also used to unregister the callback after a dash attack.
- `OnDisable()`: Ensures callbacks are unregistered when the ScriptableObject is disabled.

---

### 12. `Attacks/PlayerAttack_Weak.cs`
Implements the logic for the player's weak attack, triggering the appropriate animation and sound.

**Key Methods:**
- `AttackAction(Animator anim)`: Triggers the weak attack animation and plays the punch sound effect.

---

### 13. `Attacks/PlayerAttack_Heavy.cs`
Implements the logic for the player's heavy attack, triggering the appropriate animation and sound.

**Key Methods:**
- `AttackAction(Animator anim)`: Triggers the heavy attack animation and plays the heavy punch sound effect.

---

## Usage Notes
- Each action script is modular and can be assigned to the player as needed.
- Actions are typically triggered via Unity's Input System and are responsible for both input handling and effect execution.
- The structure allows for easy extension: new actions can be added by inheriting from `PlayerActions` and implementing the required methods.
- Attack scripts interact with the combat system for combo and damage logic.

For more details, refer to the comments and summaries within each script.
