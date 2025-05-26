// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling the player's movement input and logic, including walking, running, and air movement.
[CreateAssetMenu(fileName = "Action_Movement", menuName = "ScriptableObjects/Actions/Action_Movement", order = 1)]
public class Action_Movement : PlayerActions
{
    // The player's current movement speed, updated each frame.
    private float _currentSpeed;
    // Lerp value used for smooth acceleration and deceleration.
    private float _lerp = 0;

    //input method
    /// <summary>
    /// Handles movement input, converts it to a direction vector, and aligns it with the camera's forward axis.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        // Grab the input data and convert it into a usable Vector3.
        Vector2 inputVector = context.ReadValue<Vector2>();
        _playerData.DirectionVector = new Vector3(inputVector.x, 0 , inputVector.y);

        // Convert the direction vector from the input to work with the camera axis.
        Vector3 camForward = new Vector3(_cameraData.CameraController.transform.forward.x, 0 , _cameraData.CameraController.transform.forward.z);
        float angle = Vector3.SignedAngle(Vector3.forward, camForward, Vector3.up);
        _playerData.DirectionVector = Quaternion.AngleAxis(angle, Vector3.up) * _playerData.DirectionVector;
    }

    //method which contains the behaviour of this input
    /// <summary>
    /// Handles the player's movement logic, including grounded, air, and animation-driven movement states.
    /// </summary>
    public override void ActionEffect()
    {
        // Grounded movement logic.
        if(_playerData.isGrounded && _playerData.MovementAllowed == true)
        {
            float moveSpeed = _playerData.IsSprinting? _playerData.SprintSpeed : _playerData.Speed;
            float rotationAngle = Vector3.SignedAngle(_playerData.DirectionVector, _playerData.ActionController.Transf.forward, Vector3.up);

            // Clamp rotation angle to maximum allowed rotation speed.
            if(rotationAngle > _playerData.RotationSpeed || rotationAngle < -_playerData.RotationSpeed)
            {
                rotationAngle =  rotationAngle > 0? _playerData.RotationSpeed : -_playerData.RotationSpeed;
            }

            // Calculate the movement vector based on rotation and direction.
            Vector3 movementVector = Quaternion.AngleAxis(-rotationAngle, Vector3.up) * _playerData.ActionController.Transf.forward * _playerData.DirectionVector.magnitude;

            // If speed matches target, reset lerp for no acceleration.
            if(_currentSpeed == moveSpeed)
            {
                _lerp = 0;
            }
            else
            {
                // Smoothly accelerate or decelerate to the target speed.
                moveSpeed = Mathf.Lerp(_currentSpeed, moveSpeed, _lerp / _playerData.AccelerationLerp);
                _lerp += Time.fixedDeltaTime;
            }

            // Apply movement using rigidbody and update facing direction.
            _playerData.ActionController.Rb.velocity = (movementVector.normalized * moveSpeed) + (Vector3.up * _playerData.ActionController.Rb.velocity.y);
            _playerData.ActionController.Transf.LookAt(_playerData.ActionController.Transf.position + movementVector);
            _currentSpeed = (new Vector2(_playerData.ActionController.Rb.velocity.x, _playerData.ActionController.Rb.velocity.z)).magnitude;

            // Update movement state and animation speed.
            CheckState();
            _playerData.ActionController.Anim.SetFloat("Speed", _currentSpeed / _playerData.Speed);

            // Update sound parameter for movement speed.
            var playerModel = _playerData.ActionController.gameObject.GetComponentInChildren<JettSoundFunctions>().gameObject;
            AkSoundEngine.SetRTPCValue("MoveSpeed", _currentSpeed, playerModel);
        }
        // Air movement logic.
        else if(!_playerData.isGrounded && _playerData.MovementAllowed == true)
        {
            // Apply air movement by adding force in the direction vector.
            _playerData.ActionController.Rb.velocity += (_playerData.DirectionVector.normalized * _playerData.AirSpeed * Time.fixedDeltaTime);
            _playerData.ActionController.Transf.LookAt(_playerData.ActionController.Transf.position + _playerData.DirectionVector);
            _currentSpeed = (new Vector2(_playerData.ActionController.Rb.velocity.x, _playerData.ActionController.Rb.velocity.z)).magnitude;

            // Update movement state and animation speed.
            CheckState();
            _playerData.ActionController.Anim.SetFloat("Speed", _currentSpeed / _playerData.Speed);
        }
        // Animation-driven movement logic (e.g., for cutscenes or special moves).
        else if(_playerData.AnimOverrideMovement)
        {
            _currentSpeed = 0;
            _playerData.ActionController.Anim.SetFloat("Speed", _currentSpeed / _playerData.Speed);
            _playerData.ActionController.Rb.velocity = _playerData.ActionController.Transf.forward * _playerData.AnimForwardMovement;
        }
    }

    //Check the movement state of the player
    /// <summary>
    /// Checks and updates the player's movement state (Stopped, Moving, or Running) based on current speed.
    /// </summary>
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