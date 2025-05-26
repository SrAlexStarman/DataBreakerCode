// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling targeted camera mode, focusing on enemies and adjusting camera position accordingly.
// This script is responsible for managing the camera's behavior when in targeted mode, 
// including selecting enemies to target, updating the camera's position and rotation, 
// and exiting targeting mode when necessary.
[CreateAssetMenu(fileName = "CameraType_Targeted", menuName = "ScriptableObjects/Cameras/CameraType_Targeted", order = 1)]
public class CameraType_Targetted : CameraTypes
{
    // LayerMask used for raycasting to detect enemies for targeting.
    // This layer mask is used to filter out objects that are not enemies when raycasting.
    [SerializeField]
    private LayerMask _layerMask;

    /// <summary>
    /// Handles input for toggling targeted camera mode. Selects an enemy to target or exits targeting mode.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        // Check if the input action has started.
        if (context.phase == InputActionPhase.Started)
        {
            // If the current camera type is not targeted, enter targeting mode and select an enemy.
            if (_cameraData.CurrentCamType != DataContainer_Camera.CamType.Targetted)
            {
                TargettingEnemies();
            }
            // If the current camera type is targeted, exit targeting mode and return to free camera.
            else
            {
                _cameraData.EnemyTargeted = null;
                _cameraData.CurrentCamType = DataContainer_Camera.CamType.Free;
            }
        }
    }

    /// <summary>
    /// Updates the camera's position and rotation to focus on the targeted enemy. Exits targeting mode if no enemy is targeted.
    /// </summary>
    public override void ActionEffect()
    {
        // If no enemy is targeted, exit targeting mode.
        if (_cameraData.EnemyTargeted == null)
        {
            _cameraData.CurrentCamType = DataContainer_Camera.CamType.Free;
            return;
        }

        // Calculate the direction and angle to the enemy.
        // First, calculate the vector from the player to the enemy.
        Vector3 pointingEnemy = _playerData.ActionController.Transf.position - _cameraData.EnemyTargeted.transform.position;
        // Project this vector onto the plane defined by the up vector to get the horizontal direction.
        _cameraData.CameraPoint = (Vector3.ProjectOnPlane(pointingEnemy, Vector3.up)).normalized;
        // Calculate the angle to the enemy using the signed angle function.
        float angleTo = Vector3.SignedAngle(pointingEnemy, Vector3.ProjectOnPlane(pointingEnemy, Vector3.up), _playerData.ActionController.Transf.right);
        // If the angle is close to the target angle, use the target angle. Otherwise, smoothly interpolate to the target angle.
        angleTo = angleTo > -2 ? _cameraData.TargetAngle : _cameraData.TargetAngle + angleTo * 0.75f;
        // Update the current angle using linear interpolation.
        _cameraData.CurrentAngle = Mathf.Lerp(_cameraData.CurrentAngle, angleTo, Time.deltaTime);
        // Rotate the camera point by the current angle around the right vector.
        _cameraData.CameraPoint = (Quaternion.AngleAxis(_cameraData.CurrentAngle, _cameraData.CameraController.Transf.right) * _cameraData.CameraPoint).normalized;

        // Calculate and set the camera position after collision checks.
        // First, calculate the new position based on the camera point and target distance.
        Vector3 newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.OffsetY) + (_cameraData.CameraController.Transf.right * _cameraData.OffsetX)) + (_cameraData.CameraPoint * _cameraData.TargetDistance);
        // Perform a physical check to ensure the camera doesn't collide with objects.
        float distance = _cameraData.CameraPhysicalCheck(_playerData.ActionController.Transf, newPos, _cameraData.TargetDistance);
        // Update the new position based on the physical check.
        newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.OffsetY) + (_cameraData.CameraController.Transf.right * _cameraData.OffsetX)) + (_cameraData.CameraPoint * distance);
        // Set the camera position and rotation.
        _cameraData.CameraController.Transf.position = newPos;
        _cameraData.CameraController.Transf.LookAt(_cameraData.EnemyTargeted.transform.position);
    }

    /// <summary>
    /// Selects an enemy for targeting based on visibility, distance, and screen position.
    /// </summary>
    private void TargettingEnemies()
    {
        // Create a list to store potential enemies to target.
        List<KillEnemy> selectionUnits = new List<KillEnemy>();
        KillEnemy temp = null;
        float dist = 0;
        _cameraData.CurrentAngle = _cameraData.TargetAngle;

        // Check all enemies in the scene for visibility and proximity.
        foreach (KillEnemy enemy in KillEnemy.EnemiesList)
        {
            // Calculate the screen position of the enemy.
            Vector3 screenPos = _cameraData.CameraController.GetComponent<Camera>().WorldToScreenPoint(enemy.transform.position);
            Vector2 sP = new Vector2(screenPos.x, screenPos.y);

            RaycastHit hit;

            // Perform a raycast to check if the enemy is visible and within the targeting radius.
            if (Physics.Raycast(_playerData.ActionController.Transf.position, enemy.transform.position - _playerData.ActionController.Transf.position,
                out hit, Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position) + 1f, _layerMask))
            {
                // Only consider enemies within the targeting radius and visible.
                if (Vector2.Distance(sP, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)) < ((_cameraData.TargetRadius * 0.01f) * (Screen.height * 0.5f)) && hit.transform.gameObject == enemy.gameObject)
                {
                    selectionUnits.Add(enemy);
                }
            }
        }

        // If no enemies found, do nothing.
        if (selectionUnits.Count == 0) return;

        // Select the closest enemy from the candidates.
        foreach (KillEnemy enemy in selectionUnits)
        {
            if (temp == null || Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position) < dist)
            {
                temp = enemy;
                dist = Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position);
            }
        }

        _cameraData.EnemyTargeted = temp;

        // Switch to targeted camera mode if an enemy is selected.
        if (_cameraData.EnemyTargeted != null)
            _cameraData.CurrentCamType = DataContainer_Camera.CamType.Targetted;
    }
}