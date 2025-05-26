// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling camera reset input, aligning the camera behind the player.
// This class is responsible for managing the camera reset functionality, 
// allowing the camera to be reset to a position behind the player with a vertical offset.
[CreateAssetMenu(fileName = "CameraReset", menuName = "ScriptableObjects/Cameras/CameraReset", order = 1)]
public class CameraReset : CameraTypes
{
    /// <summary>
    /// Handles input for resetting the camera. When triggered, aligns the camera behind the player and applies a vertical offset.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    /// <remarks>
    /// This method is called when the camera reset input is triggered. It sets the camera point to a position directly behind the player,
    /// and then applies a vertical rotation to the camera point based on the _cameraData.High value.
    /// </remarks>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        // Check if the input has started
        if(context.phase == InputActionPhase.Started)
        {
            // Set camera point directly behind the player.
            // This line sets the camera point to the opposite direction of the player's forward vector.
            _cameraData.CameraPoint = -_playerData.ActionController.Transf.forward;
            
            // Apply a vertical rotation to the camera point.
            // This line applies a rotation to the camera point around the player's right axis, using the _cameraData.High value as the angle.
            _cameraData.CameraPoint = Quaternion.AngleAxis(_cameraData.High, _playerData.ActionController.Transf.right) * _cameraData.CameraPoint;
        }
    }

    /// <summary>
    /// Executes the effect of the camera reset action. (Currently empty, can be extended for additional logic.)
    /// </summary>
    /// <remarks>
    /// This method is currently empty, but can be extended to include additional logic for the camera reset effect.
    /// </remarks>
    public override void ActionEffect()
    {
        // Add logic for the effect of the camera reset here if needed.
        // This line is a placeholder for any additional logic that may be required for the camera reset effect.
    }
}
