// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling camera behavior while the player is grinding on rails.
// This class is responsible for controlling the camera's movement and rotation when the player is grinding.
[CreateAssetMenu(fileName = "CameraType_Grinding", menuName = "ScriptableObjects/Cameras/CameraType_Grinding", order = 2)]
public class CameraType_Grinding : CameraTypes
{
    // Speed at which the camera rotates to align with the player's direction while grinding.
    // This value controls how quickly the camera adjusts to the player's movement.
    [SerializeField] private float _rotationSpeed;

    /// <summary>
    /// Handles input for the grinding camera type. (Currently empty, as grinding camera is auto-controlled.)
    /// </summary>
    /// <param name="context">The input callback context.</param>
    /// <remarks>This method is currently not used, as the grinding camera is controlled automatically.</remarks>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        // No input handling required for grinding camera.
    }

    /// <summary>
    /// Updates the camera's position and rotation to follow and align with the player while grinding.
    /// </summary>
    /// <remarks>This method is called every frame to update the camera's position and rotation.</remarks>
    public override void ActionEffect()
    {
        // Smoothly rotate camera to align with the player's forward direction.
        // This ensures the camera is always facing the direction the player is moving.
        Quaternion targetRotation = Quaternion.LookRotation(_playerData.ActionController.Transf.forward);
        _cameraData.CameraController.Transf.rotation = Quaternion.Slerp(_cameraData.CameraController.Transf.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        // Position the camera behind and above the player, at the grinding distance.
        // This calculation takes into account the player's position, the camera's offset, and the grinding distance.
        Vector3 cameraPosition = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.CurrentOffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.CurrentOffsetX));
        _cameraData.CameraPoint = -_cameraData.CameraController.Transf.forward;
        cameraPosition += _cameraData.CameraPoint * _cameraData.SprintDistance;

        // Update the camera's position to the calculated value.
        _cameraData.CameraController.Transf.position = cameraPosition;
    }
}
