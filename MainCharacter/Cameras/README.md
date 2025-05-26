# MainCharacter/Cameras Directory

This directory contains the core scripts responsible for managing the camera systems in the project, including different camera modes (free, grinding, targeted), camera reset functionality, and the abstract base for camera behaviors.

## Script Overview

### 1. `CameraTypes.cs`
Abstract base `ScriptableObject` for all camera types, providing a structure for camera input and effect logic, and references to player and camera data containers.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Abstract method for handling camera input effects (to be implemented by subclasses).
- `ActionEffect()`: Abstract method for executing the camera effect (to be implemented by subclasses).

---

### 2. `CameraType_Free.cs`
Handles free camera movement, including rotation, smoothing, and enemy targeting logic. Supports smooth transitions based on player movement state and dynamically targets enemies in combat.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles input for free camera movement, adjusting speed and direction based on player input and inversion settings.
- `ActionEffect()`: Handles the free camera's logic, including smoothing, rotation, collision, and enemy targeting.
- `CameraSmooth()`: Smoothly lerps the camera's distance based on the player's movement state.
- `CombatArea()`: Checks for nearby enemies and updates camera targeting behavior.

---

### 3. `CameraType_Grinding.cs`
Handles camera behavior while the player is grinding on rails, controlling camera movement and rotation to follow and align with the player.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Currently unused, as the grinding camera is auto-controlled.
- `ActionEffect()`: Updates the camera's position and rotation to follow and align with the player while grinding.

---

### 4. `CameraType_Targetted.cs`
Handles targeted camera mode, focusing on enemies and adjusting camera position accordingly. Manages the selection of enemies, camera focus, and switching between targeted and free camera modes.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles input for toggling targeted camera mode and enemy selection.
- `ActionEffect()`: Updates the camera's position and rotation to focus on the targeted enemy, or exits targeting mode if no enemy is targeted.
- `TargettingEnemies()`: Selects an enemy for targeting based on visibility, distance, and screen position.

---

### 5. `CameraReset.cs`
Handles camera reset input, aligning the camera behind the player with a vertical offset. Allows the camera to be reset to a default position for better player orientation.

**Key Methods:**
- `InputEffect(InputAction.CallbackContext)`: Handles input for resetting the camera, aligning it behind the player and applying a vertical offset.
- `ActionEffect()`: Executes the effect of the camera reset action (currently empty, can be extended).

---

## Usage Notes
- Each camera type is modular and can be assigned or switched as needed based on game state or player input.
- Camera scripts are designed to be extendable; new camera modes can be added by inheriting from `CameraTypes` and implementing the required methods.
- The system supports smooth transitions, targeted enemy focus, and context-specific camera behaviors for enhanced gameplay experience.

For more details, refer to the comments and summaries within each script.
