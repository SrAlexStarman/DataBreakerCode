// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the main camera's behavior, mode switching, and initialization for the player.
/// </summary>
public class CameraController : MonoBehaviour
{
    // Reference to the player data container.
    [SerializeField] private DataContainer_Player _playerData;
    // Reference to the camera data container.
    [SerializeField] private DataContainer_Camera _cameraData;

    // Cached transform for camera movement and positioning.
    internal Transform Transf;

    /// <summary>
    /// Initializes camera references and sets up the initial camera position and mode.
    /// </summary>
    private void Start()
    {
        _cameraData.CameraController = this;
        _cameraData.CurrentCamType = DataContainer_Camera.CamType.Free;

        // Set camera offsets and distance to initial values.
        _cameraData.CurrentOffsetX = _cameraData.OffsetX;
        _cameraData.CurrentOffsetY = _cameraData.OffsetY;
        _cameraData.CurrentDistance = 8f;

        Transf = GetComponent<Transform>();

        // Set the initial camera point behind the player with a vertical offset.
        _cameraData.CameraPoint = -_playerData.ActionController.Transf.forward;
        _cameraData.CameraPoint = Quaternion.AngleAxis(_cameraData.High,  _playerData.ActionController.Transf.right) * _cameraData.CameraPoint;
    }

    /// <summary>
    /// Updates the camera each frame, executing the correct camera mode's logic.
    /// </summary>
    private void Update()
    {
        switch(_cameraData.CurrentCamType)
        {
            case DataContainer_Camera.CamType.Free:
                // Free camera mode logic.
                _cameraData.CameraTypes[0].ActionEffect();
            break;

            case DataContainer_Camera.CamType.Targetted:
                // Targeted camera mode logic.
                _cameraData.CameraTypes[1].ActionEffect();
            break;

            case DataContainer_Camera.CamType.Grinding:
                // Grinding camera mode logic.
                _cameraData.CameraTypes[2].ActionEffect();
            break;
        }
    }
}
