# DataBreakerCode Project Overview

Welcome to the DataBreakerCode project! This repository contains the core systems, scripts, and modular architecture for a third-person action game in Unity. The project is organized into well-documented directories, each responsible for a major gameplay or system feature. This README serves as a high-level guide and entry point for understanding the structure, purpose, and interconnections of all major components.

DataBreaker Itchio link: https://vfs-gdpg.itch.io/data-breaker
---

## Project Structure

### 1. GameSystems
Manages global game systems such as checkpoints, combat, and system coordination.
- **CheckPoint & CheckPointSystem:** Handle player progression, respawning, and checkpoint activation.
- **CombatSystem:** Manages player combat, combos, attacks, special states (like "Karnage"), and hit detection.
- **GameSystemsController:** Coordinates initialization and updates for core systems.

*See [`GameSystems/README.md`](./GameSystems/README.md) for detailed script and usage documentation.*

---

### 2. MainCharacter/Actions
Contains modular scripts for all player actions: movement, jumping, sprinting, dashing, grinding, grappling, and attacking.
- **Core Action Scripts:** Each action (e.g., movement, jump, dash) is encapsulated in its own `ScriptableObject` for modularity and extensibility.
- **Attacks Subfolder:** Houses all attack logic, including weak/heavy attacks and their base classes.
- **Collision & Effects:** Includes punch collision detection and visual/audio feedback.

*See [`MainCharacter/Actions/README.md`](./MainCharacter/Actions/README.md) for a per-script breakdown and extension notes.*

---

### 3. MainCharacter/Cameras
Implements the camera system, supporting multiple camera modes (free, targeted, grinding) and camera reset functionality.
- **CameraTypes:** Abstract base for all camera logic.
- **CameraType_*:** Specialized scripts for each camera mode.
- **CameraReset:** Script for aligning the camera behind the player.

*See [`MainCharacter/Cameras/README.md`](./MainCharacter/Cameras/README.md) for script details and usage guidance.*

---

### 4. MainCharacter/Controllers
Central managers for player and camera logic.
- **ActionController:** Handles player movement, actions, rail grinding, collision, and animation state.
- **CameraController:** Manages camera mode switching and updates.

*See [`MainCharacter/Controllers/README.md`](./MainCharacter/Controllers/README.md) for controller responsibilities and extension notes.*

---

### 5. MainCharacter/DataContainers
ScriptableObjects that store all runtime parameters, references, and state for the player and camera systems.
- **DataContainer_Player:** Stores all player state, movement, health, attack, and special mode data.
- **DataContainer_Camera:** Stores all camera state, offsets, targeting, and collision logic.

*See [`MainCharacter/DataContainers/README.md`](./MainCharacter/DataContainers/README.md) for a summary of fields and methods.*

---

For any questions or further guidance, consult the README files in each directory or review the comments within the scripts themselves. Happy developing!
