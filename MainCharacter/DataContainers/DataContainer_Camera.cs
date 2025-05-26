// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all camera-related parameters, references, and state for the player camera system.
/// </summary>
[CreateAssetMenu(fileName = "DataContainer_Camera", menuName = "ScriptableObjects/DataContainer/DataContainer_Camera", order = 1)]
public class DataContainer_Camera : ScriptableObject
{
    /// <summary>
    /// Camera mode enumeration.
    /// </summary>
    public enum CamType{
        Free,
        Targetted,
        Grinding
    }

    [Header("Current Camera Reference")]
    /// <summary>
    /// Reference to the camera controller script.
    /// </summary>
    public CameraController CameraController;

    [Header("Camera Modes")]
    /// <summary>
    /// Array of available camera type logic objects.
    /// </summary>
    public CameraTypes[] CameraTypes;

    [Header("Camera Parameters")]
    /// <summary>
    /// Horizontal camera offset.
    /// </summary>
    public float OffsetX;
    /// <summary>
    /// Vertical camera offset.
    /// </summary>
    public float OffsetY;
    /// <summary>
    /// Current horizontal camera offset (runtime).
    /// </summary>
    [ReadOnly] public float CurrentOffsetX;
    /// <summary>
    /// Current vertical camera offset (runtime).
    /// </summary>
    [ReadOnly] public float CurrentOffsetY;
    /// <summary>
    /// Vertical angle for camera positioning.
    /// </summary>
    [Range(-10f, 85f)] public float High;
    /// <summary>
    /// Layer mask for camera collision checks.
    /// </summary>
    [SerializeField] private LayerMask _layermask;
    /// <summary>
    /// Current vertical angle (runtime).
    /// </summary>
    [ReadOnly] public float Angle;
    /// <summary>
    /// Current camera distance from player.
    /// </summary>
    [ReadOnly] public float CurrentDistance;
    /// <summary>
    /// Current camera mode.
    /// </summary>
    [ReadOnly] public CamType CurrentCamType = CamType.Free;
    /// <summary>
    /// Directional vector for camera positioning.
    /// </summary>
    [ReadOnly] public Vector3 CameraPoint;

    [Header("Free Camera Parameters")]
    /// <summary>
    /// Idle camera distance.
    /// </summary>
    [Range(2f, 10f)] public float IdleDistance;
    /// <summary>
    /// Moving camera distance.
    /// </summary>
    [Range(2f, 10f)] public float MoveDistance;
    /// <summary>
    /// Sprinting camera distance.
    /// </summary>
    [Range(2f, 10f)] public float SprintDistance;
    /// <summary>
    /// Initial rotation speed for free camera mode.
    /// </summary>
    public float InitialRotationSpeed;
    /// <summary>
    /// Final rotation speed for free camera mode.
    /// </summary>
    public float FinalRotationSpeed;
    /// <summary>
    /// Curve type for free camera mode.
    /// </summary>
    public CurveType FreeCameraCurveType;
    /// <summary>
    /// Lerp speed value for free camera mode.
    /// </summary>
    public float LerpSpeedValue;
    /// <summary>
    /// Combat range for free camera mode.
    /// </summary>
    public float CombatRange;
    /// <summary>
    /// Invert X-axis for free camera mode.
    /// </summary>
    public bool InvertedX;
    /// <summary>
    /// Invert Y-axis for free camera mode.
    /// </summary>
    public bool InvertedY;

    [Header("Targeted Camera Parameters")]
    /// <summary>
    /// Target angle for targeted camera mode.
    /// </summary>
    public float TargetAngle;
    /// <summary>
    /// Target distance for targeted camera mode.
    /// </summary>
    public float TargetDistance;
    /// <summary>
    /// Target radius for targeted camera mode (percentage of screen height).
    /// </summary>
    [Tooltip("Percentage of the height of the screen.")]public float TargetRadius;
    /// <summary>
    /// Current angle for targeted camera mode (runtime).
    /// </summary>
    [ReadOnly] public float CurrentAngle;
    /// <summary>
    /// Enemy targeted by the camera (runtime).
    /// </summary>
    [ReadOnly] public KillEnemy EnemyTargeted;

    /// <summary>
    /// Performs a physical check (sphere cast) to determine the maximum allowed camera distance before hitting an obstacle.
    /// </summary>
    /// <param name="playerTrans">The player's transform.</param>
    /// <param name="cameraPosition">The desired camera position.</param>
    /// <param name="maxDistance">The maximum camera distance.</param>
    /// <returns>The allowed distance before collision.</returns>
    public float CameraPhysicalCheck(Transform playerTrans, Vector3 cameraPosition, float maxDistance)
    {
        RaycastHit hit;
        Vector3 origin = playerTrans.position + (CameraController.Transf.right * CurrentOffsetX) + (Vector3.up * CurrentOffsetY);

        Debug.DrawLine(origin, cameraPosition, Color.red);

        // Perform a sphere cast to check for obstacles between the player and the camera.
        if(Physics.SphereCast(origin, 0.1f, (cameraPosition - origin).normalized, out hit, maxDistance, _layermask))
        {   
            float dist = Vector3.Distance(origin, hit.point);
            // Optionally adjust offsets based on collision distance.
            //CurrentOffsetX = OffsetX * (dist/maxDistance);
            //CurrentOffsetY = OffsetY * (dist/maxDistance);
            return dist;
        }

        // If no collision, use default offsets and return max distance.
        CurrentOffsetX = OffsetX;
        CurrentOffsetY = OffsetY;
        return maxDistance;
    }
}