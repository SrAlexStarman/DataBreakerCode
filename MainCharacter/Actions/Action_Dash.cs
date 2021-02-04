// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Action_Dash", menuName = "ScriptableObjects/Actions/Action_Dash", order = 1)]
public class Action_Dash : PlayerActions
{
    [SerializeField] private LayerMask _layerMask;

    private Vector3 _initialPosition;
    private Vector3 _endPosition;
    private float _currentDashTime = 0;
    private float _definedTime = 0;
    private float _currentDelayTime = 0;
    private bool _dashing = false;
    private bool _dashAllowed = true;
    private bool _isGrappling = false;
    private CurveType _lastType;
    private Func<float, float> Curve = null;

    [SerializeField] private CombatSystem _combatSystem;

    [SerializeField] private float _sphereCastRadious;

    public Action<Animator> CallbackFunctions;

    public override void InputEffect(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Vector3 endPos;
            Vector3 dashDirection;
            float dashDistance;
            RaycastHit hit;

            if (_playerData.DirectionVector != Vector3.zero)
            {
                dashDirection = _playerData.DirectionVector;
                _playerData.ActionController.Transf.LookAt(_playerData.ActionController.Transf.position + dashDirection);
            }

            else
            {
                dashDirection = _playerData.ActionController.Transf.forward;
            }

            if (Physics.SphereCast(_playerData.ActionController.Transf.position, _sphereCastRadious, dashDirection, out hit, _playerData.DashDistance, _layerMask))
            {
                dashDistance = (hit.point - _playerData.ActionController.Transf.position).magnitude - _sphereCastRadious;
            }

            else
            {
                dashDistance = _playerData.DashDistance;
            }

            if (dashDistance > 0.3f)
            {
                endPos = _playerData.ActionController.Transf.position + dashDirection * dashDistance;

                Dash(endPos, false);
            }

            //Set the sound switch
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

    public override void ActionEffect() //Updated function of the dash. Moves the player during dash action
    {
        if (_dashing == true)
        {
            //THIS CODE WILL BE REMOVED TO THE BUILD TO INCREASE PERFORMANCE
            #region BuildExclusion
            
            if(_cameraData.FreeCameraCurveType != _lastType || Curve == null)
            {
                LerpCurves.SetCurve(ref Curve, _playerData.DashCurveType);
                _lastType = _playerData.DashCurveType;
            }

            #endregion

            //Apply lerp curve
            float t = _currentDashTime / _definedTime;
            t = Curve(t);
            _playerData.ActionController.Transf.position = Vector3.Lerp(_initialPosition, _endPosition, t);
            _currentDashTime += Time.fixedDeltaTime;

            if (Vector3.Distance(_playerData.ActionController.Transf.position, _endPosition) < 0.1f) //dash ends
            {
                ShowPlayer();
                _combatSystem.AllowAttack();
                _dashing = false;
                _playerData.ActionController.Rb.velocity = Vector3.zero;

                if (_isGrappling == false)
                {
                    //APPLY THIs LINE FOR FAKE MOMENTUN AFTER AIR DASH
                    //_playerData.ActionController.Rb.velocity = _playerData.ActionController.Transf.forward * _playerData.DashSpeed * 1.5f;
                    _playerData.ActionController.Rb.useGravity = true;
                    _currentDashTime = 0;
                    _playerData.MovementAllowed = true;
                }

                if (CallbackFunctions != null)
                {
                    CallbackFunctions(_playerData.ActionController.Anim);
                }
            }
        }

        else if (_dashAllowed == false) // timer that avoid the dash an amount of time between dashes
        {
            _currentDelayTime += Time.fixedDeltaTime;

            if (_currentDelayTime >= _playerData.TimeBetweenDashes)
            {
                _dashAllowed = true;
                _currentDelayTime = 0;
            }
        }
    }

    private void ShowPlayer() // Toggles the visibility of the player
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

    public void Dash(Vector3 endPoint, bool isGrappling) // Starts the dash movement
    {
        Debug.Log($"DashAction {_dashAllowed}");

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

            //Play dash sound
            AkSoundEngine.PostEvent("SFX_Jett_Dash", _playerData.ActionController.gameObject);

            _combatSystem.ResetCombat();
        }
    }

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


        //Set the sound switch
        AkSoundEngine.SetSwitch("Velocity", "Walk", _playerData.ActionController.gameObject);
    }
}
