# MainCharacter/DataContainers Directory

This directory contains ScriptableObjects that store and manage all runtime parameters, references, and state for the player and camera systems. These data containers are central to the modular architecture, enabling controllers and actions to share and update gameplay state efficiently.

## Script Overview

### 1. `DataContainer_Player.cs`
Stores all player-related parameters, references, and state for the main character, including movement, health, actions, dashing, grappling, grinding, attacks, and special modes.

**Key Properties & Fields:**
- `ActionController`: Reference to the player's action controller script.
- `MaxHealth`, `CurrentHealth`, `IsInvinsible`: Health and invincibility state.
- `PlayerMovementState`, `GrindingBackwards`, `DistanceInPath`, `IsSprinting`, `isGrounded`, `Jumped`: Movement and action state.
- `Speed`, `SprintSpeed`, `AccelerationLerp`, `RotationSpeed`, `AirSpeed`: Movement parameters.
- `JumpForce`: Jumping parameter.
- `DashSpeed`, `DashDistance`, `TimeBetweenDashes`, `DashCurveType`: Dash parameters.
- `TargetRadius`, `GrapplingSpeed`, `GrapRange`: Grappling parameters.
- `GrindSpeed`: Rail grinding parameter.
- `TimeBetweenCombos`, `TimeToResetCombo`, `SmallSmashRange`, `BigSmashRange`: Attack parameters.
- `KarnageCombo`, `KarnageTime`, `IsRestTime`, `RestTime`, `IsKarnageON`, `KarnagePoints`, `DecreaseKarnagePointSpeed`, `MaxKarnagePoints`, `MaxKarnageCombo`, `MaxKarnageTimer`, `TimeBeforeDecrease`, `TimerAcceleration`: Special mode (Karnage) parameters.

**Key Methods:**
- `OnHit(float damage)`: Reduces player health by the specified damage amount unless invincible.

---

### 2. `DataContainer_Camera.cs`
Stores all camera-related parameters, references, and state for the player camera system, including camera mode, position, offsets, smoothing, targeting, and collision checks.

**Key Properties & Fields:**
- `CameraController`: Reference to the camera controller script.
- `CameraTypes[]`: Array of available camera mode logic objects.
- `OffsetX`, `OffsetY`, `CurrentOffsetX`, `CurrentOffsetY`, `High`, `Angle`, `CurrentDistance`, `CameraPoint`: Camera position and orientation parameters.
- `CurrentCamType`: Current camera mode (Free, Targetted, Grinding).
- `IdleDistance`, `MoveDistance`, `SprintDistance`, `InitialRotationSpeed`, `FinalRotationSpeed`, `FreeCameraCurveType`, `LerpSpeedValue`, `CombatRange`, `InvertedX`, `InvertedY`: Free camera parameters.
- `TargetAngle`, `TargetDistance`, `TargetRadius`, `CurrentAngle`, `EnemyTargeted`: Targeted camera parameters.

**Key Methods:**
- `CameraPhysicalCheck(Transform playerTrans, Vector3 cameraPosition, float maxDistance)`: Performs a sphere cast to determine the maximum allowed camera distance before hitting an obstacle.

---

## Usage Notes
- Data containers are ScriptableObjects designed for sharing runtime state and parameters between controllers, actions, and camera logic.
- Extend these containers to add new gameplay variables or state as your project evolves.
- Refer to the comments and summaries within each script for detailed descriptions of each field and method.
