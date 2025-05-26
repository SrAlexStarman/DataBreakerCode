// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using UnityEngine;
using UnityEngine.InputSystem;

// Abstract base ScriptableObject for all camera types, providing a structure for camera input and effect logic.
[System.Serializable]
public abstract class CameraTypes : ScriptableObject
{
    // Reference to the player's data container, used for accessing and modifying player state.
    [SerializeField] protected DataContainer_Player _playerData;
    // Reference to the camera's data container, used for camera-relative movement and effects.
    [SerializeField] protected DataContainer_Camera _cameraData;

    /// <summary>
    /// Abstract method to handle input effects for the camera type. Must be implemented by subclasses.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public abstract void InputEffect(InputAction.CallbackContext context);

    /// <summary>
    /// Abstract method to execute the effect of the camera type. Must be implemented by subclasses.
    /// </summary>
    public abstract void ActionEffect();
}
