// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Action_Movement", menuName = "ScriptableObjects/Actions/Action_Movement", order = 1)]
public class Action_Movement : PlayerActions
{
    private float _currentSpeed;
    private float _lerp = 0;

    //input method
    public override void InputEffect(InputAction.CallbackContext context)
    {
        //grab the input data and convert it into a usable data (vector3)
        Vector2 inputVector = context.ReadValue<Vector2>();
        _playerData.DirectionVector = new Vector3(inputVector.x, 0 , inputVector.y);

        //convert the direction vector from the input to work with the camera axis
        Vector3 camForward = new Vector3(_cameraData.CameraController.transform.forward.x, 0 , _cameraData.CameraController.transform.forward.z);
        float angle = Vector3.SignedAngle(Vector3.forward, camForward, Vector3.up);
        _playerData.DirectionVector = Quaternion.AngleAxis(angle, Vector3.up) * _playerData.DirectionVector;
    }

    //method which contains the behaviour of this input
    public override void ActionEffect()
    {
        if(_playerData.isGrounded && _playerData.MovementAllowed == true)
        {
            float moveSpeed = _playerData.IsSprinting? _playerData.SprintSpeed : _playerData.Speed;
            float rotationAngle = Vector3.SignedAngle(_playerData.DirectionVector, _playerData.ActionController.Transf.forward, Vector3.up);

            if(rotationAngle > _playerData.RotationSpeed || rotationAngle < -_playerData.RotationSpeed)
            {
                rotationAngle =  rotationAngle > 0? _playerData.RotationSpeed : -_playerData.RotationSpeed;
            }

            Vector3 movementVector = Quaternion.AngleAxis(-rotationAngle, Vector3.up) * _playerData.ActionController.Transf.forward * _playerData.DirectionVector.magnitude;

            if(_currentSpeed == moveSpeed)
            {
                _lerp = 0;
            }

            else
            {
                //acceleration
                moveSpeed = Mathf.Lerp(_currentSpeed, moveSpeed, _lerp / _playerData.AccelerationLerp);
                _lerp += Time.fixedDeltaTime;
            }

            //Apply the movement by rigidbody
            _playerData.ActionController.Rb.velocity = (movementVector.normalized * moveSpeed) + (Vector3.up * _playerData.ActionController.Rb.velocity.y);
            _playerData.ActionController.Transf.LookAt(_playerData.ActionController.Transf.position + movementVector);
            _currentSpeed = (new Vector2(_playerData.ActionController.Rb.velocity.x, _playerData.ActionController.Rb.velocity.z)).magnitude;

            CheckState();
            _playerData.ActionController.Anim.SetFloat("Speed", _currentSpeed / _playerData.Speed);
            var playerModel = _playerData.ActionController.gameObject.GetComponentInChildren<JettSoundFunctions>().gameObject;
            AkSoundEngine.SetRTPCValue("MoveSpeed", _currentSpeed, playerModel);
        }

        else if(!_playerData.isGrounded && _playerData.MovementAllowed == true)
        {
            //Apply the movement by rigidbody
            _playerData.ActionController.Rb.velocity += (_playerData.DirectionVector.normalized * _playerData.AirSpeed * Time.fixedDeltaTime);
            _playerData.ActionController.Transf.LookAt(_playerData.ActionController.Transf.position + _playerData.DirectionVector);
            _currentSpeed = (new Vector2(_playerData.ActionController.Rb.velocity.x, _playerData.ActionController.Rb.velocity.z)).magnitude;

            CheckState();
            _playerData.ActionController.Anim.SetFloat("Speed", _currentSpeed / _playerData.Speed);
        }

        else if(_playerData.AnimOverrideMovement)
        {
            _currentSpeed = 0;
            _playerData.ActionController.Anim.SetFloat("Speed", _currentSpeed / _playerData.Speed);
            _playerData.ActionController.Rb.velocity = _playerData.ActionController.Transf.forward * _playerData.AnimForwardMovement;
        }
    }

    //Check the movement state of the player
    private void CheckState()
    {
        if(_currentSpeed < 1f)
        {
            _playerData.PlayerMovementState = DataContainer_Player.MovementState.Stopped;
        }

        else if(_currentSpeed <= _playerData.Speed)
        {
            _playerData.PlayerMovementState = DataContainer_Player.MovementState.Moving;
        }

        else
        {
            _playerData.PlayerMovementState = DataContainer_Player.MovementState.Running;
        }
    }
}