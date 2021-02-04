// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CameraType_Targeted", menuName = "ScriptableObjects/Cameras/CameraType_Targeted", order = 1)]
public class CameraType_Targetted : CameraTypes
{
    [SerializeField]
    private LayerMask _layerMask;

    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(_cameraData.CurrentCamType != DataContainer_Camera.CamType.Targetted)
            {
                TargettingEnemies();
            }

            else
            {
                _cameraData.EnemyTargeted = null;
                _cameraData.CurrentCamType = DataContainer_Camera.CamType.Free;
            }
        }
    }

    public override void ActionEffect()
    {
        if(_cameraData.EnemyTargeted == null)
        {
            _cameraData.CurrentCamType = DataContainer_Camera.CamType.Free;
            return;
        }

        Vector3 pointingEnemy = _playerData.ActionController.Transf.position - _cameraData.EnemyTargeted.transform.position;
        _cameraData.CameraPoint = (Vector3.ProjectOnPlane(pointingEnemy, Vector3.up)).normalized;
        float angleTo = Vector3.SignedAngle(pointingEnemy, Vector3.ProjectOnPlane(pointingEnemy, Vector3.up), _playerData.ActionController.Transf.right);
        angleTo = angleTo > -2? _cameraData.TargetAngle : _cameraData.TargetAngle + angleTo * 0.75f;
        _cameraData.CurrentAngle = Mathf.Lerp(_cameraData.CurrentAngle, angleTo, Time.deltaTime);
        _cameraData.CameraPoint = (Quaternion.AngleAxis(_cameraData.CurrentAngle, _cameraData.CameraController.Transf.right) * _cameraData.CameraPoint).normalized;


        Vector3 newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.OffsetY) + (_cameraData.CameraController.Transf.right * _cameraData.OffsetX)) + (_cameraData.CameraPoint * _cameraData.TargetDistance);
        float distance = _cameraData.CameraPhysicalCheck(_playerData.ActionController.Transf, newPos, _cameraData.TargetDistance);
        newPos = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.OffsetY) + (_cameraData.CameraController.Transf.right * _cameraData.OffsetX)) + (_cameraData.CameraPoint * distance);
        _cameraData.CameraController.Transf.position = newPos;
        _cameraData.CameraController.Transf.LookAt(_cameraData.EnemyTargeted.transform.position);
    }

    private void TargettingEnemies()
    {     
        List<KillEnemy> selectionUnits = new List<KillEnemy>();
        KillEnemy temp = null;
        float dist = 0;
        _cameraData.CurrentAngle = _cameraData.TargetAngle;
        
        foreach(KillEnemy enemy in KillEnemy.EnemiesList)
        {
            Vector3 screenPos = _cameraData.CameraController.GetComponent<Camera>().WorldToScreenPoint(enemy.transform.position);
            Vector2 sP = new Vector2(screenPos.x, screenPos.y);

            RaycastHit hit;

            if(Physics.Raycast(_playerData.ActionController.Transf.position, enemy.transform.position - _playerData.ActionController.Transf.position, 
            out hit, Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position) + 1f, _layerMask))
            {                
                if(Vector2.Distance(sP, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)) < ((_cameraData.TargetRadius * 0.01f) * (Screen.height * 0.5f)) &&  hit.transform.gameObject == enemy.gameObject)
                {
                    selectionUnits.Add(enemy);
                }
            }
        }

        if(selectionUnits.Count == 0) return;

        foreach(KillEnemy enemy in selectionUnits)
        {
            if(temp == null || Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position) < dist)
            {
                temp = enemy;
                dist = Vector3.Distance(_playerData.ActionController.Transf.position, enemy.transform.position);
            }
        }

        _cameraData.EnemyTargeted = temp;

        if(_cameraData.EnemyTargeted != null)
            _cameraData.CurrentCamType = DataContainer_Camera.CamType.Targetted;
    }
}