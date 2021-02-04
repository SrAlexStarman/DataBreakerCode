// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CameraReset", menuName = "ScriptableObjects/Cameras/CameraReset", order = 1)]
public class CameraReset : CameraTypes
{
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            _cameraData.CameraPoint = -_playerData.ActionController.Transf.forward;
            _cameraData.CameraPoint = Quaternion.AngleAxis(_cameraData.High, _playerData.ActionController.Transf.right) * _cameraData.CameraPoint;
        }
    }

    public override void ActionEffect()
    {

    }
}
