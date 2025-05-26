// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles the player's dash action, including dash movement, cooldowns, and visual/sound effects.
// This script is responsible for managing the player's dash ability, including movement, cooldowns, and visual/sound effects.
[CreateAssetMenu(fileName = "Action_Dash", menuName = "ScriptableObjects/Actions/Action_Dash", order = 1)]
public class Action_Dash : PlayerActions
{
    // LayerMask to determine what the dash can collide with.
    // This layer mask is used to determine what objects the dash can collide with.
    [SerializeField] private LayerMask _layerMask;

    // Dash movement variables.
    // These variables are used to track the dash movement.
    private Vector3 _initialPosition; // Start position of the dash
    private Vector3 _endPosition;     // End position of the dash
    private float _currentDashTime = 0; // Current time spent dashing
    private float _definedTime = 0;      // Total dash duration
    private float _currentDelayTime = 0; // Time since last dash (for cooldown)
    private bool _dashing = false;       // Is the player currently dashing?
    private bool _dashAllowed = true;    // Is dash currently allowed?
    private bool _isGrappling = false;   // Is the dash part of a grapple?
    private CurveType _lastType;         // Last used curve type for dash
    private Func<float, float> Curve = null; // Curve function for dash interpolation

    // Reference to the combat system for combo resets and attack permissions.
    // This reference is used to interact with the combat system.
    [SerializeField] private CombatSystem _combatSystem;

    // Radius for spherecast to check obstacles during dash.
    // This radius is used to check for obstacles during the dash.
    [SerializeField] private float _sphereCastRadious;

    // Optional callback after dash ends.
    // This callback is called after the dash ends.
    public Action<Animator> CallbackFunctions;

    // Handles dash input from the player.
    // This method handles the dash input from the player.
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Vector3 endPos;
            Vector3 dashDirection;
            float dashDistance;
            RaycastHit hit;

            // Determine the dash direction based on input or facing direction.
            // This logic determines the direction of the dash based on the player's input or facing direction.
            if (_playerData.DirectionVector != Vector3.zero)
            {
                dashDirection = _playerData.DirectionVector;
                _playerData.ActionController.Transf.LookAt(_playerData.ActionController.Transf.position + dashDirection);
            }
            else
            {
                dashDirection = _playerData.ActionController.Transf.forward;
            }

            // Use spherecast to check for obstacles in dash direction.
            // This spherecast checks for obstacles in the dash direction.
            if (Physics.SphereCast(_playerData.ActionController.Transf.position, _sphereCastRadious, dashDirection, out hit, _playerData.DashDistance, _layerMask))
            {
                dashDistance = (hit.point - _playerData.ActionController.Transf.position).magnitude - _sphereCastRadious;
            }
            else
            {
                dashDistance = _playerData.DashDistance;
            }

            // Only dash if there is enough distance.
            // This logic checks if there is enough distance to perform the dash.
            if (dashDistance > 0.3f)
            {
                endPos = _playerData.ActionController.Transf.position + dashDirection * dashDistance;
                Dash(endPos, false);
            }
            // Uncomment to set sound switch for running (if using Wwise)
            // AkSoundEngine.SetSwitch("Velocity", "Run", _playerData.ActionController.gameObject);
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

    // Handles the dash movement, dash cooldown, and dash end logic.
    // This method handles the dash movement, cooldown, and end logic.
    public override void ActionEffect() //Updated function of the dash. Moves the player during dash action
    {
        if (_dashing == true)
        {
            // Update the dash curve if needed.
            #region BuildExclusion
            if (_cameraData.FreeCameraCurveType != _lastType || Curve == null)
            {
                LerpCurves.SetCurve(ref Curve, _playerData.DashCurveType);
                _lastType = _playerData.DashCurveType;
            }
            #endregion

            // Apply interpolation curve to dash movement.
            // This logic applies the interpolation curve to the dash movement.
            float t = _currentDashTime / _definedTime;
            t = Curve(t);
            _playerData.ActionController.Transf.position = Vector3.Lerp(_initialPosition, _endPosition, t);
            _currentDashTime += Time.fixedDeltaTime;

            // End dash if close enough to the target position.
            // This logic checks if the player is close enough to the target position to end the dash.
            if (Vector3.Distance(_playerData.ActionController.Transf.position, _endPosition) < 0.1f)
            {
                ShowPlayer();
                _combatSystem.AllowAttack();
                _dashing = false;
                _playerData.ActionController.Rb.velocity = Vector3.zero;

                if (_isGrappling == false)
                {
                    // End dash and restore movement/gravity.
                    // This logic ends the dash and restores movement/gravity.
                    _playerData.ActionController.Rb.useGravity = true;
                    _currentDashTime = 0;
                    _playerData.MovementAllowed = true;
                }

                // Call any registered callback functions.
                // This logic calls any registered callback functions.
                if (CallbackFunctions != null)
                {
                    CallbackFunctions(_playerData.ActionController.Anim);
                }
            }
        }
        // Handle dash cooldown timer.
        // This logic handles the dash cooldown timer.
        else if (_dashAllowed == false)
        {
            _currentDelayTime += Time.fixedDeltaTime;

            if (_currentDelayTime >= _playerData.TimeBetweenDashes)
            {
                _dashAllowed = true;
                _currentDelayTime = 0;
            }
        }
    }

    // Toggles the visibility of the player and dash particles, and sets invincibility during dash.
    // This method toggles the visibility of the player and dash particles, and sets invincibility during dash.
    private void ShowPlayer() 
    {
        if (_playerData.ActionController.Anim.gameObject.activeInHierarchy)
        {
            _playerData.ActionController.Anim.gameObject.SetActive(false);
            _playerData.ActionController.DashParticles.SetActive(true);
            _playerData.IsInvinsible = true;
        }
        else
        {
            _playerData.ActionController.Anim.gameObject.SetActive(true);
            _playerData.ActionController.DashParticles.SetActive(false);
            _playerData.IsInvinsible = false;
        }
    }

    // Starts the dash movement toward the specified endpoint. If isGrappling is true, dash is part of a grapple.
    // This method starts the dash movement toward the specified endpoint.
    public void Dash(Vector3 endPoint, bool isGrappling) 
    {
        Debug.Log($"DashAction {_dashAllowed}");

        // Only start dash if allowed and not grinding.
        // This logic checks if the dash is allowed and not grinding.
        if (_dashAllowed == true && _playerData.PlayerMovementState != DataContainer_Player.MovementState.Grinding)
        {
            Debug.Log("DashActionP");
            _isGrappling = isGrappling;
            _initialPosition = _playerData.ActionController.Transf.position;
            _dashing = true;
            ShowPlayer();
            _playerData.MovementAllowed = false;
            _playerData.ActionController.Rb.useGravity = false;
            _dashAllowed = false;
            _definedTime = Vector3.Distance(_playerData.ActionController.Transf.position, endPoint) / _playerData.DashSpeed;

            _endPosition = endPoint;

            // Play dash sound with Wwise.
            // This logic plays the dash sound with Wwise.
            AkSoundEngine.PostEvent("SFX_Jett_Dash", _playerData.ActionController.gameObject);

            // Reset combat state during dash.
            // This logic resets the combat state during dash.
            _combatSystem.ResetCombat();
        }
    }

    // Ends the dash, restores player visibility and velocity, and resets state.
    // This method ends the dash, restores player visibility and velocity, and resets state.
    public void EndDash()
    {
        if (!_playerData.ActionController.Anim.gameObject.activeInHierarchy)
        {
            ShowPlayer();
            _playerData.ActionController.Rb.velocity = (_endPosition - _playerData.ActionController.Transf.position).normalized * _playerData.DashSpeed * 1.5f;
        }

        _dashing = false;
        _currentDashTime = 0;
        _isGrappling = false;
        _playerData.MovementAllowed = true;
        _playerData.ActionController.Transf.gameObject.GetComponent<Rigidbody>().useGravity = true;

        // Set the sound switch for walking (Wwise).
        AkSoundEngine.SetSwitch("Velocity", "Walk", _playerData.ActionController.gameObject);
    }
}
