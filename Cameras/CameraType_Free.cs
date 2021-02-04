// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CameraType_Free", menuName = "ScriptableObjects/Cameras/CameraType_Free", order = 1)]
public class CameraType_Free : CameraTypes
{
    private Vector2 _inputVector = new Vector2(0, 0);
    private float _camSpeed = 0;
    private float _lerp = 0;
    private CurveType _lastType;
    private Func<float, float> Curve = null;

    //input method
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            _camSpeed = _cameraData.InitialRotationSpeed;
            _lerp = 0;
        }

        _inputVector = context.ReadValue<Vector2>();

        if(_cameraData.InvertedX==false)
        {
            _inputVector.x *=-1;
        }

        if(_cameraData.InvertedY==true)
        {
            _inputVector.y *=-1;
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
        //THIS CODE WILL BE REMOVED TO THE BUILD TO INCREASE PERFORMANCE
        #region BuildExclusion
        
        if(_cameraData.FreeCameraCurveType != _lastType || Curve == null)
        {
            LerpCurves.SetCurve(ref Curve, _cameraData.FreeCameraCurveType);
            _lastType = _cameraData.FreeCameraCurveType;
        }

        #endregion

        //Apply lerp curve
        float t = _lerp / _cameraData.LerpSpeedValue;
        t = Curve(t);
        _camSpeed = Mathf.Lerp(_camSpeed, _cameraData.FinalRotationSpeed, t);
        _lerp += Time.deltaTime;

        //Apply input rotations
        _cameraData.CameraPoint = Quaternion.AngleAxis(_inputVector.x * _camSpeed * Time.deltaTime, Vector3.up) * _cameraData.CameraPoint;
        _cameraData.Angle = _cameraData.CameraController.Transf.rotation.eulerAngles.x;

        if(_cameraData.Angle > 180) _cameraData.Angle -= 360;

        if((_inputVector.y > 0 && _cameraData.Angle < 85f) || (_inputVector.y < 0 && _cameraData.Angle > -10f))
        {
            _cameraData.CameraPoint = Quaternion.AngleAxis(_inputVector.y * _camSpeed * Time.deltaTime, _cameraData.CameraController.Transf.right) * _cameraData.CameraPoint;
        }

        //Calculate the camera position without physics
        Vector3 newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.CurrentOffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.CurrentOffsetX)) + (_cameraData.CameraPoint * _cameraData.CurrentDistance);
        
        //Check collision
        _cameraData.CurrentDistance = _cameraData.CameraPhysicalCheck(_playerData.ActionController.Transf, newPos, _cameraData.CurrentDistance);

        //Camera position after collisions
        newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.OffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.OffsetX)) + (_cameraData.CameraPoint * _cameraData.CurrentDistance);

        _cameraData.CameraController.Transf.position = newPos;
        _cameraData.CameraController.Transf.LookAt(_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.CurrentOffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.CurrentOffsetX));

        CombatArea();
        CameraSmooth();
    }

    //Lerps to smooth the transions between speeds
    private void CameraSmooth()
    {
        if(_playerData.PlayerMovementState == DataContainer_Player.MovementState.Stopped)
        {
            _cameraData.CurrentDistance = Mathf.Lerp(_cameraData.CurrentDistance, _cameraData.IdleDistance, Time.deltaTime);
        }

        if(_playerData.PlayerMovementState == DataContainer_Player.MovementState.Moving)
        {
            _cameraData.CurrentDistance = Mathf.Lerp(_cameraData.CurrentDistance, _cameraData.MoveDistance, Time.deltaTime);
        }

        if(_playerData.PlayerMovementState == DataContainer_Player.MovementState.Running)
        {
            _cameraData.CurrentDistance = Mathf.Lerp(_cameraData.CurrentDistance, _cameraData.SprintDistance, Time.deltaTime);
        }
    }

    //this method check if there are enemies in an area around the player
    private void CombatArea()
    {
        List<KillEnemy> selectionUnits = new List<KillEnemy>();
        KillEnemy temp = null;
        Vector3 playerPos = _playerData.ActionController.Transf.position;
        float dist = 0;

        //check all the enemies in the area
        foreach(KillEnemy enemy in KillEnemy.EnemiesList)
        {
            if((enemy.transform.position.y > playerPos.y - 1f && enemy.transform.position.y < playerPos.y + 1f) && Vector3.Distance(playerPos, enemy.transform.position) <= _cameraData.CombatRange)
            {
                RaycastHit hit;

                if(Physics.Raycast(playerPos, (enemy.transform.position - playerPos).normalized, out hit, Mathf.Infinity) && hit.transform.gameObject == enemy.gameObject)
                {
                    selectionUnits.Add(enemy);
                }
            }
        }

        if(selectionUnits.Count == 0)
        {
            _cameraData.EnemyTargeted = null;
            return;
        } 
        
        //select the closest enemy in the area
        if(_playerData.DirectionVector.magnitude == 0)
        {
            foreach(KillEnemy enemy in selectionUnits)
            {
                float newDist = Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position);

                if(temp == null || newDist < dist)
                {
                    temp = enemy;
                    dist = newDist;
                }
            }
        }

        else
        {
            foreach(KillEnemy enemy in selectionUnits)
            {
                float enemyDot = -Vector3.Dot(_playerData.DirectionVector.normalized, (_playerData.ActionController.Transf.position - enemy.transform.position).normalized);
                float enemyAngle = Vector3.Angle(_playerData.DirectionVector, enemy.transform.position - _playerData.ActionController.Transf.position);

                if(temp == null)
                {
                    temp = enemy;
                    continue;
                }

                float tempDot = Vector3.Dot(_playerData.DirectionVector, _playerData.ActionController.Transf.position - temp.transform.position);

                if(enemyDot > tempDot && enemyAngle < 90f)
                {
                    temp = enemy;
                }
            }
        }


        _cameraData.EnemyTargeted = temp;
    }
}