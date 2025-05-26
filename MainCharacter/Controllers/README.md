# MainCharacter/Controllers Directory

This directory contains the main controller scripts for the player character and camera systems. These scripts serve as the central managers for player actions, movement, rail grinding, collision, animation, and camera mode switching.

## Script Overview

### 1. `ActionController.cs`
Handles player actions, movement, rail grinding, collision, and animation state management for the main character.

**Key Methods:**
- `Awake()`: Initializes references to components and sets up the player data container link.
- `Start()`: Sets initial player movement state and allows movement at the start of the game.
- `FixedUpdate()`: Handles physics updates, checks if the player is grounded, and updates all relevant actions.
- `IsGrounded()`: Checks if the player is grounded using a sphere cast; updates animation and landing effects.
- `IsGoingBackwards(Vector3 segmentForward)`: Determines if the player is moving backwards relative to a segment's forward vector.
- `IgnoreLastSpline()`: Ignores the last used rail spline and starts a cooldown before it can be used again.
- `CoolDownIgnoreSpline()`: Coroutine that waits for a cooldown before re-enabling the previously ignored spline.
- `OnCollisionEnter(Collision collision)`: Handles collision logic for entering rail segments and starting grind mechanics.

---

### 2. `CameraController.cs`
Controls the main camera's behavior, mode switching, and initialization for the player.

**Key Methods:**
- `Start()`: Initializes camera references and sets up the initial camera position and mode.
- `Update()`: Updates the camera each frame, executing the correct camera mode's logic depending on the current camera type.

---

## Usage Notes
- Controller scripts serve as the central managers for player and camera logic, delegating specialized behavior to other components and ScriptableObjects.
- The `ActionController` manages player movement, actions, and rail grinding, while the `CameraController` manages camera mode switching and updates.
- Extend or modify these controllers to add new gameplay features or camera behaviors as needed.

For more details, refer to the comments and summaries within each script.
