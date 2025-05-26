// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling free camera movement, including rotation, smoothing, and enemy targeting logic.
/// <summary>
/// ScriptableObject for handling free camera movement, including rotation, smoothing, and enemy targeting logic.
/// </summary>
[CreateAssetMenu(fileName = "CameraType_Free", menuName = "ScriptableObjects/Cameras/CameraType_Free", order = 1)]
public class CameraType_Free : CameraTypes
{
    // Stores the current input vector for camera movement.
    /// <summary>
    /// The current input vector for camera movement.
    /// </summary>
    private Vector2 _inputVector = new Vector2(0, 0);

    // Current speed of camera rotation.
    /// <summary>
    /// The current speed of camera rotation.
    /// </summary>
    private float _camSpeed = 0;

    // Lerp value for smooth camera speed transitions.
    /// <summary>
    /// The lerp value for smooth camera speed transitions.
    /// </summary>
    private float _lerp = 0;

    // Last used curve type for camera smoothing.
    /// <summary>
    /// The last used curve type for camera smoothing.
    /// </summary>
    private CurveType _lastType;

    // Reference to the current smoothing curve function.
    /// <summary>
    /// The reference to the current smoothing curve function.
    /// </summary>
    private Func<float, float> Curve = null;

    // Input method for handling free camera movement.
    /// <summary>
    /// Handles input for free camera movement, adjusting speed and direction based on player input and inversion settings.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        // Initialize camera speed and lerp when input starts.
        if (context.phase == InputActionPhase.Started)
        {
            _camSpeed = _cameraData.InitialRotationSpeed;
            _lerp = 0;
        }

        // Read the input vector for camera movement.
        _inputVector = context.ReadValue<Vector2>();

        // Invert X axis if needed.
        if (_cameraData.InvertedX == false)
        {
            _inputVector.x *= -1;
        }

        // Invert Y axis if needed.
        if (_cameraData.InvertedY == true)
        {
            _inputVector.y *= -1;
        }
    }
    /* THIS CODE WILL BE ADDED TO THE BUILD TO INCREASE PERFORMANCE
    #region BuildInclusion
        public void OnStart()
        {
            LerpCurves.SetCurve(ref Curve, _type);
        }
    #endregion
    */
    //method which contains the behaviour of this input
    public override void ActionEffect()
    {
        // Ensure the correct curve is set for camera smoothing.
        #region BuildExclusion
        if (_cameraData.FreeCameraCurveType != _lastType || Curve == null)
        {
            LerpCurves.SetCurve(ref Curve, _cameraData.FreeCameraCurveType);
            _lastType = _cameraData.FreeCameraCurveType;
        }
        #endregion

        // Apply lerp curve for smooth camera speed transition.
        float t = _lerp / _cameraData.LerpSpeedValue;
        t = Curve(t);
        _camSpeed = Mathf.Lerp(_camSpeed, _cameraData.FinalRotationSpeed, t);
        _lerp += Time.deltaTime;

        // Apply input-based rotation to the camera point.
        _cameraData.CameraPoint = Quaternion.AngleAxis(_inputVector.x * _camSpeed * Time.deltaTime, Vector3.up) * _cameraData.CameraPoint;
        _cameraData.Angle = _cameraData.CameraController.Transf.rotation.eulerAngles.x;

        if (_cameraData.Angle > 180) _cameraData.Angle -= 360;

        // Apply vertical rotation only within allowed angle limits.
        if ((_inputVector.y > 0 && _cameraData.Angle < 85f) || (_inputVector.y < 0 && _cameraData.Angle > -10f))
        {
            _cameraData.CameraPoint = Quaternion.AngleAxis(_inputVector.y * _camSpeed * Time.deltaTime, _cameraData.CameraController.Transf.right) * _cameraData.CameraPoint;
        }

        // Calculate the desired camera position before collision checks.
        Vector3 newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.CurrentOffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.CurrentOffsetX)) + (_cameraData.CameraPoint * _cameraData.CurrentDistance);

        // Check for collisions and adjust camera distance accordingly.
        _cameraData.CurrentDistance = _cameraData.CameraPhysicalCheck(_playerData.ActionController.Transf, newPos, _cameraData.CurrentDistance);

        // Update camera position after collision adjustment.
        newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.OffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.OffsetX)) + (_cameraData.CameraPoint * _cameraData.CurrentDistance);

        _cameraData.CameraController.Transf.position = newPos;
        _cameraData.CameraController.Transf.LookAt(_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.CurrentOffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.CurrentOffsetX));

        // Check for nearby enemies and update camera behavior if needed.
        CombatArea();
        // Smooth camera transitions between different player movement states.
        CameraSmooth();
    }

    // Method for smoothing camera transitions between different player movement states.
    /// <summary>
    /// Smoothly lerps the camera's distance based on the player's movement state (stopped, moving, running).
    /// </summary>
    private void CameraSmooth()
    {
        if (_playerData.PlayerMovementState == DataContainer_Player.MovementState.Stopped)
        {
            _cameraData.CurrentDistance = Mathf.Lerp(_cameraData.CurrentDistance, _cameraData.IdleDistance, Time.deltaTime);
        }

        if (_playerData.PlayerMovementState == DataContainer_Player.MovementState.Moving)
        {
            _cameraData.CurrentDistance = Mathf.Lerp(_cameraData.CurrentDistance, _cameraData.MoveDistance, Time.deltaTime);
        }

        if (_playerData.PlayerMovementState == DataContainer_Player.MovementState.Running)
        {
            _cameraData.CurrentDistance = Mathf.Lerp(_cameraData.CurrentDistance, _cameraData.SprintDistance, Time.deltaTime);
        }
    }

    // Method for checking if there are enemies in an area around the player.
    /// <summary>
    /// Checks for enemies within a certain area around the player and selects the closest or most relevant one for targeting.
    /// </summary>
    private void CombatArea()
    {
        List<KillEnemy> selectionUnits = new List<KillEnemy>();
        KillEnemy temp = null;
        Vector3 playerPos = _playerData.ActionController.Transf.position;
        float dist = 0;

        // Check all the enemies in the area.
        foreach (KillEnemy enemy in KillEnemy.EnemiesList)
        {
            if ((enemy.transform.position.y > playerPos.y - 1f && enemy.transform.position.y < playerPos.y + 1f) && Vector3.Distance(playerPos, enemy.transform.position) <= _cameraData.CombatRange)
            {
                RaycastHit hit;

                if (Physics.Raycast(playerPos, (enemy.transform.position - playerPos).normalized, out hit, Mathf.Infinity) && hit.transform.gameObject == enemy.gameObject)
                {
                    selectionUnits.Add(enemy);
                }
            }
        }

        // If no enemies found, clear target.
        if (selectionUnits.Count == 0)
        {
            _cameraData.EnemyTargeted = null;
            return;
        }

        // Select the closest enemy if the player is not moving.
        if (_playerData.DirectionVector.magnitude == 0)
        {
            foreach (KillEnemy enemy in selectionUnits)
            {
                float newDist = Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position);

                if (temp == null || newDist < dist)
                {
                    temp = enemy;
                    dist = newDist;
                }
            }
        }

        // Otherwise, select the enemy most in front of the player.
        else
        {
            foreach (KillEnemy enemy in selectionUnits)
            {
                float enemyDot = -Vector3.Dot(_playerData.DirectionVector.normalized, (_playerData.ActionController.Transf.position - enemy.transform.position).normalized);
                float enemyAngle = Vector3.Angle(_playerData.DirectionVector, enemy.transform.position - _playerData.ActionController.Transf.position);

                if (temp == null)
                {
                    temp = enemy;
                    continue;
                }

                float tempDot = Vector3.Dot(_playerData.DirectionVector, _playerData.ActionController.Transf.position - temp.transform.position);

                if (enemyDot > tempDot && enemyAngle < 90f)
                {
                    temp = enemy;
                }
            }
        }

        // Set the selected enemy as the current target.
        _cameraData.EnemyTargeted = temp;
    }
}