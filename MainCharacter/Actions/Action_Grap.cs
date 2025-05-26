// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles the player's grappling hook action, including targeting, movement, and visual/sound effects.
// This script is responsible for managing the player's grappling hook, including finding and targeting grappling points,
// moving the hand towards the target, and starting a dash when attached.
[CreateAssetMenu(fileName = "Action_Grap", menuName = "ScriptableObjects/Actions/Action_Grap", order = 1)]
public class Action_Grap : PlayerActions
{
    // Reference to the dash action (used for grappling dash).
    // This field holds a reference to the dash action, which is used when the player is grappling.
    [SerializeField]
    private Action_Dash _dash;

    // Time allowed before losing reference to a grappling point.
    // This field determines how long the player can grapple before losing reference to the grappling point.
    [SerializeField]
    private float _timeToLostReference;

    // Internal state variables.
    // These variables keep track of the player's grappling state.
    private float _timer = 0;             // Timer for losing reference
    private float _currentDashTime = 0;   // Time spent dashing toward grappling point
    private float _handDistance = 0;      // Distance from hand to grappling point
    private GrapplingPoint _targetedUnit;     // Currently targeted grappling point
    private GrapplingPoint _lastTargetedUnit; // Last targeted grappling point
    private bool _grappling = false;      // Is the player currently grappling?
    private Vector3 _initialPosition;     // Initial hand position at start of grapple

    // Handles input for starting and canceling the grappling action.
    // This method is called when the player inputs a grappling action.
    public override void InputEffect(InputAction.CallbackContext context)
    {
        // Start grappling if a target is available and not already grappling.
        // If the player starts grappling and there is a valid target, this block of code is executed.
        if (context.phase == InputActionPhase.Started)
        {
            if (_targetedUnit != null && _grappling == false)
            {
                // Set initial hand position and rotation.
                // The hand is positioned and rotated to face the grappling point.
                _initialPosition = _playerData.ActionController.Transf.position + Vector3.forward * 1f + Vector3.up * 0.2f;
                _playerData.ActionController.Hand.transform.position = _initialPosition;
                _playerData.ActionController.Hand.transform.LookAt(_targetedUnit.transform.position);
                _playerData.ActionController.Hand.SetActive(true);
                _grappling = true;
                _lastTargetedUnit = _targetedUnit;
                _handDistance = Vector3.Distance(_playerData.ActionController.transform.position, _lastTargetedUnit.transform.position) / (_playerData.Speed * 2f);
                Vector3 lookRot = new Vector3(_targetedUnit.transform.position.x, _playerData.ActionController.transform.position.y, _targetedUnit.transform.position.z);
                _playerData.ActionController.Transf.LookAt(lookRot);

                // Play sound on the hand (Wwise).
                // A sound effect is played when the player starts grappling.
                AkSoundEngine.SetSwitch("Hit", "GrPoint", _playerData.ActionController.gameObject);
                AkSoundEngine.PostEvent("SFX_Jett_Graplinghook_Start_Play", _playerData.ActionController.gameObject);

                // Disable gravity if airborne.
                // If the player is airborne, gravity is disabled to prevent the player from falling.
                if (_playerData.isGrounded == false)
                    _playerData.ActionController.Rb.useGravity = false;
            }
        }

        // Cancel grappling on input release.
        // If the player releases the grappling input, this block of code is executed.
        if (context.phase == InputActionPhase.Canceled)
        {
            if (_grappling == true)
            {
                _dash.EndDash();
                _playerData.ActionController.Hand.SetActive(false);
                _grappling = false;
                _playerData.MovementAllowed = true;
                _timer = 0;
            }
        }
    }

    // Handles grappling logic, including targeting, hand movement, and starting dash when attached.
    // This method is called every frame to update the grappling logic.
    public override void ActionEffect()
    {
        GrapplingPoint tempUnit = TargetGrappling();

        // Update target highlighting if the target changes.
        // If the target changes, the highlighting is updated.
        if ((_targetedUnit == null && tempUnit != null) || (_targetedUnit != null && tempUnit == null) ||
        (_targetedUnit != null && tempUnit != null && _targetedUnit != tempUnit))
        {
            _targetedUnit?.SwitchMaterial();
            tempUnit?.SwitchMaterial();
            _targetedUnit = tempUnit;
        }

        // If not grappling, increment timer for losing reference.
        // If the player is not grappling, the timer is incremented.
        if (_grappling == false)
        {
            if (_timer <= _timeToLostReference)
            {
                _timer += Time.fixedDeltaTime;
            }
            else
            {
                _lastTargetedUnit = null;
            }
        }

        // If grappling, move hand toward target and start dash when attached.
        // If the player is grappling, the hand is moved towards the target and a dash is started when attached.
        else if (_playerData.ActionController.Hand.transform.position != _lastTargetedUnit.transform.position)
        {
            _playerData.ActionController.Hand.transform.position = Vector3.Lerp(_playerData.ActionController.Hand.transform.position, _lastTargetedUnit.transform.position, _currentDashTime / _handDistance);

            _currentDashTime += Time.fixedDeltaTime;

            if (_playerData.ActionController.Hand.transform.position == _lastTargetedUnit.transform.position)
            {
                _playerData.MovementAllowed = false;
                _dash.Dash(_lastTargetedUnit.PlayerAttachingPoint.position, true);
                _currentDashTime = 0;
            }
        }
    }

    // Finds the best grappling point to target based on screen position, distance, and visibility.
    // This method finds the best grappling point to target based on the player's screen position, distance, and visibility.
    private GrapplingPoint TargetGrappling()
    {
        List<GrapplingPoint> unitsOnScreen = new List<GrapplingPoint>();
        List<GrapplingPoint> selectionUnits = new List<GrapplingPoint>();
        GrapplingPoint temp = null;
        float dist = 0;

        // Gather all grappling points on screen and in range.
        foreach (GrapplingPoint unit in GrapplingPoint.GrapplingList)
        {
            if (unit)
            {
                if (unit.OnScreen() &&
                unit != _lastTargetedUnit &&
                Vector3.Distance(_playerData.ActionController.Transf.position, unit.transform.position) <= _playerData.GrapRange)
                {
                    unitsOnScreen.Add(unit);
                }
            }
        }

        if (unitsOnScreen.Count == 0) return null;

        // Check which units are close to the center of the screen and not occluded.
        foreach (GrapplingPoint gp in unitsOnScreen)
        {
            Vector3 screenPos = _cameraData.CameraController.GetComponent<Camera>().WorldToScreenPoint(gp.transform.position);
            Vector2 sP = new Vector2(screenPos.x, screenPos.y);

            RaycastHit hit;

            if (Physics.Raycast(_playerData.ActionController.Transf.position,
            gp.transform.position - _playerData.ActionController.Transf.position, out hit, Vector3.Distance(_playerData.ActionController.Transf.position, gp.transform.position) + 1f))
            {
                if (Vector2.Distance(sP, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)) < ((_cameraData.TargetRadius * 0.01f) * (Screen.height * 0.5f)) && hit.transform.gameObject == gp.gameObject)
                {
                    selectionUnits.Add(gp);
                }
            }
        }

        if (selectionUnits.Count == 0) return null;

        // Select the closest valid grappling point.
        foreach (GrapplingPoint gp in selectionUnits)
        {
            if (temp == null || Vector3.Distance(_playerData.ActionController.Transf.position, gp.transform.position) < dist)
            {
                temp = gp;
                dist = Vector3.Distance(_playerData.ActionController.Transf.position, gp.transform.position);
            }
        }

        return temp;
    }
}