// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Action_Grap", menuName = "ScriptableObjects/Actions/Action_Grap", order = 1)]
public class Action_Grap : PlayerActions
{
    [SerializeField]
    private Action_Dash _dash;

    [SerializeField]
    private float _timeToLostReference;

    private float _timer = 0;
    private float _currentDashTime = 0;
    private float _handDistance = 0;
    private GrapplingPoint _targetedUnit;
    private GrapplingPoint _lastTargetedUnit;
    private bool _grappling = false;
    private Vector3 _initialPosition;

    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(_targetedUnit != null && _grappling == false)
            {
                _initialPosition = _playerData.ActionController.Transf.position + Vector3.forward * 1f + Vector3.up * 0.2f;
                _playerData.ActionController.Hand.transform.position = _initialPosition;
                _playerData.ActionController.Hand.transform.LookAt(_targetedUnit.transform.position);
                _playerData.ActionController.Hand.SetActive(true);
                _grappling = true;
                _lastTargetedUnit = _targetedUnit;
                _handDistance = Vector3.Distance(_playerData.ActionController.transform.position, _lastTargetedUnit.transform.position) / (_playerData.Speed * 2f);
                Vector3 lookRot = new Vector3(_targetedUnit.transform.position.x, _playerData.ActionController.transform.position.y, _targetedUnit.transform.position.z);
                _playerData.ActionController.Transf.LookAt(lookRot);

                //Play sound on the hand
                AkSoundEngine.SetSwitch("Hit", "GrPoint", _playerData.ActionController.gameObject);
                AkSoundEngine.PostEvent("SFX_Jett_Graplinghook_Start_Play", _playerData.ActionController.gameObject);

                if(_playerData.isGrounded == false)
                    _playerData.ActionController.Rb.useGravity = false;
            }
        }

        if(context.phase == InputActionPhase.Canceled)
        {
            if(_grappling == true)
            {
                _dash.EndDash();
                _playerData.ActionController.Hand.SetActive(false);
                _grappling = false;
                _playerData.MovementAllowed = true;
                _timer = 0;
            }    
        }
    }

    public override void ActionEffect()
    {
        GrapplingPoint tempUnit = TargetGrappling();

        if((_targetedUnit == null && tempUnit != null) || (_targetedUnit != null && tempUnit == null) || 
        (_targetedUnit != null && tempUnit != null && _targetedUnit != tempUnit))
        {
            _targetedUnit?.SwitchMaterial();
            tempUnit?.SwitchMaterial();
            _targetedUnit = tempUnit;
        }

        if(_grappling == false)
        {
            if(_timer <= _timeToLostReference)
            {
                _timer += Time.fixedDeltaTime;
            }
            else
            {
                _lastTargetedUnit = null;
            }
        }

        else if(_playerData.ActionController.Hand.transform.position != _lastTargetedUnit.transform.position)
        {
            _playerData.ActionController.Hand.transform.position = Vector3.Lerp(_playerData.ActionController.Hand.transform.position, _lastTargetedUnit.transform.position, _currentDashTime /_handDistance);

            _currentDashTime += Time.fixedDeltaTime;

            if(_playerData.ActionController.Hand.transform.position == _lastTargetedUnit.transform.position)
            {
                _playerData.MovementAllowed = false;
                _dash.Dash(_lastTargetedUnit.PlayerAttachingPoint.position, true);
                _currentDashTime = 0;
            }
        }
    }

    private GrapplingPoint TargetGrappling()
    {
        List<GrapplingPoint> unitsOnScreen = new List<GrapplingPoint>();
        List<GrapplingPoint> selectionUnits = new List<GrapplingPoint>();
        GrapplingPoint temp = null;
        float dist = 0;

        foreach(GrapplingPoint unit in GrapplingPoint.GrapplingList)
        {
            if(unit)
            {
                if(unit.OnScreen() && 
                unit != _lastTargetedUnit && 
                Vector3.Distance(_playerData.ActionController.Transf.position, unit.transform.position) <= _playerData.GrapRange)
                {
                    unitsOnScreen.Add(unit);
                }
            }
        }
        
        if(unitsOnScreen.Count == 0) return null;

        foreach(GrapplingPoint gp in unitsOnScreen)
        {
            Vector3 screenPos = _cameraData.CameraController.GetComponent<Camera>().WorldToScreenPoint(gp.transform.position);
            Vector2 sP = new Vector2(screenPos.x, screenPos.y);

            RaycastHit hit;

            if(Physics.Raycast(_playerData.ActionController.Transf.position, 
            gp.transform.position - _playerData.ActionController.Transf.position, out hit, Vector3.Distance(_playerData.ActionController.Transf.position, gp.transform.position) + 1f))
            {
                if(Vector2.Distance(sP, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)) < ((_cameraData.TargetRadius * 0.01f) * (Screen.height * 0.5f)) &&  hit.transform.gameObject == gp.gameObject)
                {
                    selectionUnits.Add(gp);
                }
            }
        }

        if(selectionUnits.Count == 0) return null;

        foreach(GrapplingPoint gp in selectionUnits)
        {
            if(temp == null || Vector3.Distance(_playerData.ActionController.Transf.position, gp.transform.position) < dist)
            {
                temp = gp;
                dist = Vector3.Distance(_playerData.ActionController.Transf.position, gp.transform.position);
            }
        }

        return temp;
    }
}