// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataContainer_Camera", menuName = "ScriptableObjects/DataContainer/DataContainer_Camera", order = 1)]
public class DataContainer_Camera : ScriptableObject
{
    public enum CamType{
        Free,
        Targetted,
        Grinding
    }

    [Header("Current Camera Reference")]
    public CameraController CameraController;

    [Header("Camera Modes")]
    public CameraTypes[] CameraTypes;

    [Header("Camera Parameters")]
    public float OffsetX;
    public float OffsetY;
    [ReadOnly] public float CurrentOffsetX;
    [ReadOnly] public float CurrentOffsetY;
    [Range(-10f, 85f)] public float High;
    [SerializeField] private LayerMask _layermask;
    [ReadOnly] public float Angle;
    [ReadOnly] public float CurrentDistance;
    [ReadOnly] public CamType CurrentCamType = CamType.Free;
    [ReadOnly] public Vector3 CameraPoint;

    [Header("Free Camera Parameters")]
    [Range(2f, 10f)] public float IdleDistance;
    [Range(2f, 10f)] public float MoveDistance;
    [Range(2f, 10f)] public float SprintDistance;
    public float InitialRotationSpeed;
    public float FinalRotationSpeed;
    public CurveType FreeCameraCurveType;
    public float LerpSpeedValue;
    public float CombatRange;
    public bool InvertedX;
    public bool InvertedY;

    [Header("Targeted Camera Parameters")]
    public float TargetAngle;
    public float TargetDistance;
    [Tooltip("Percentage of the height of the screen.")]public float TargetRadius;
    [ReadOnly] public float CurrentAngle;
    [ReadOnly] public KillEnemy EnemyTargeted;

    public float CameraPhysicalCheck(Transform playerTrans, Vector3 cameraPosition, float maxDistance)
    {
        RaycastHit hit;
        Vector3 origin = playerTrans.position + (CameraController.Transf.right * CurrentOffsetX) + (Vector3.up * CurrentOffsetY);

        Debug.DrawLine(origin, cameraPosition, Color.red);

        if(Physics.SphereCast(origin, 0.1f, (cameraPosition - origin).normalized, out hit, maxDistance, _layermask))
        {   
            float dist = Vector3.Distance(origin, hit.point);
            //CurrentOffsetX = OffsetX * (dist/maxDistance);
            //CurrentOffsetY = OffsetY * (dist/maxDistance);
            return dist;
        }

        CurrentOffsetX = OffsetX;
        CurrentOffsetY = OffsetY;
        return maxDistance;
    }
}