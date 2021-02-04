using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CameraType_Grinding", menuName = "ScriptableObjects/Cameras/CameraType_Grinding", order = 2)]
public class CameraType_Grinding : CameraTypes
{
    [SerializeField] private float _rotationSpeed;

    public override void InputEffect(InputAction.CallbackContext context)
    {
    }

    public override void ActionEffect()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_playerData.ActionController.Transf.forward);
        _cameraData.CameraController.Transf.rotation = Quaternion.Slerp(_cameraData.CameraController.Transf.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        Vector3 cameraPosition = (_playerData.ActionController.Transf.position + (Vector3.up * _cameraData.CurrentOffsetY) +
         (_cameraData.CameraController.Transf.right * _cameraData.CurrentOffsetX));
        _cameraData.CameraPoint = -_cameraData.CameraController.Transf.forward;
        cameraPosition += _cameraData.CameraPoint * _cameraData.SprintDistance;

        _cameraData.CameraController.Transf.position = cameraPosition;
    }
}
