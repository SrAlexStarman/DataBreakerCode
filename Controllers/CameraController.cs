// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private DataContainer_Player _playerData;
    [SerializeField] private DataContainer_Camera _cameraData;

    internal Transform Transf;

    private void Start()
    {
        _cameraData.CameraController = this;
        _cameraData.CurrentCamType = DataContainer_Camera.CamType.Free;

        _cameraData.CurrentOffsetX = _cameraData.OffsetX;
        _cameraData.CurrentOffsetY = _cameraData.OffsetY;

        _cameraData.CurrentDistance = 8f;

        Transf = GetComponent<Transform>();

        _cameraData.CameraPoint = -_playerData.ActionController.Transf.forward;
        _cameraData.CameraPoint = Quaternion.AngleAxis(_cameraData.High,  _playerData.ActionController.Transf.right) * _cameraData.CameraPoint;
    }

    private void Update()
    {
        switch(_cameraData.CurrentCamType)
        {
            case DataContainer_Camera.CamType.Free:
                _cameraData.CameraTypes[0].ActionEffect();
            break;

            case DataContainer_Camera.CamType.Targetted:
                _cameraData.CameraTypes[1].ActionEffect();
            break;

            case DataContainer_Camera.CamType.Grinding:
                _cameraData.CameraTypes[2].ActionEffect();
            break;
        }
    }
}
