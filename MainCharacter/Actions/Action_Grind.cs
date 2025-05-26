// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using SplineMesh;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

// Handles the player's rail grinding action, including movement along splines and regaining control.
[CreateAssetMenu(fileName = "Action_Grind", menuName = "ScriptableObjects/Actions/Action_Grind", order = 4)]
public class Action_Grind : PlayerActions
{
    // Called every frame to update grinding logic and movement along the rail.
    public override void ActionEffect()
    {
        var spline = _playerData.ActionController.Spline;
        if (spline != null)
        {
            if (spline.IsLoop)
            {
                GrindLoop();
            }
            else
            {
                GrindNonLoop();
            }
        }

    }

    // Handles input while grinding (currently unused).
    public override void InputEffect(InputAction.CallbackContext context)
    {
        //Can we add inputs when railing?
    }

    // Handles movement and logic for grinding on non-looping rails.
    void GrindNonLoop()
    {
        //Perform the grinding
        Grind();

        //Check if player jumped
        if (_playerData.Jumped)
        {
            RegainControls();
            _playerData.ActionController.Rb.velocity += _playerData.ActionController.Transf.up * Mathf.Abs(_playerData.GrindSpeed);
            return;
        }

        //check if we finished our path
        if (_playerData.GrindingBackwards)
        {
            if (_playerData.DistanceInPath <= 0)
            {
                RegainControls();
            }
        }
        else
        {
            if (_playerData.DistanceInPath >= _playerData.ActionController.Spline.Length)
            {
                RegainControls();
            }
        }
    }

    // Handles movement and logic for grinding on looping rails.
    void GrindLoop()
    {

        //Distance in path if we looped throught everything
        if (_playerData.GrindingBackwards)
        {
            if (_playerData.DistanceInPath < 0f)
            {
                _playerData.DistanceInPath = _playerData.ActionController.Spline.Length;
            }
        }
        else
        {
            if (_playerData.DistanceInPath > _playerData.ActionController.Spline.Length)
            {
                _playerData.DistanceInPath = 0f;
            }
        }

        //Perform the grinding
        Grind();

        //Only leave if jumped
        if (_playerData.Jumped)
        {
            RegainControls();
            _playerData.ActionController.Rb.velocity += _playerData.ActionController.Transf.up * Mathf.Abs(_playerData.GrindSpeed);
        }

    }

    // Performs the actual movement and animation updates for grinding along the rail.
    void Grind()
    {
        //Play the rail particles
        if (_playerData.ActionController.GrindParticles != null)
        {
            _playerData.ActionController.GrindParticles.SetBool("isGrinding", true);
        }

        //Start grinding animation
        _playerData.ActionController.Anim.SetBool("Railing", true);

        _cameraData.CurrentCamType = DataContainer_Camera.CamType.Grinding;

        //Check if we have a spline to rail on
        if (_playerData.ActionController.Spline == null) return;

        //Remove player controls
        _playerData.MovementAllowed = false;

        //Make rigidbody kinemactic
        _playerData.ActionController.Rb.isKinematic = true;

        //Set our state to grinding
        _playerData.PlayerMovementState = DataContainer_Player.MovementState.Grinding;

        //Grab our distance in the rail
        var sample = _playerData.ActionController.Spline.GetSampleAtDistance(_playerData.DistanceInPath);

        //Set the offset of the player model
        var offset = new Vector3(0f, 1f, 0f);
        _playerData.ActionController.transform.position = sample.location + offset;

        //Get the forward of the rail
        var forward = sample.Rotation.eulerAngles;

        //Get the grinding speed
        var speed = _playerData.GrindSpeed;

        //Move according to the direction
        if (_playerData.GrindingBackwards)
        {
            //change the rotation based if we are going backwards
            forward.y += 180f;

            //also change the velocity
            speed = -Mathf.Abs(_playerData.GrindSpeed);
        }

        //face the rail direction
        _playerData.ActionController.transform.rotation = Quaternion.Euler(forward);

        //move along the rails
        _playerData.DistanceInPath += speed * Time.fixedDeltaTime;
    }

    //After we leave the rail get regain our controls
    // Called when the player leaves the rail to restore normal controls and apply launch velocity.
    void RegainControls()
    {
        //Play Sound
        AkSoundEngine.PostEvent("SFX_Jett_Rail_Stop", _playerData.ActionController.gameObject);

        //Stop grinding animation
        _playerData.ActionController.Anim.SetBool("Railing", false);
        _cameraData.CurrentCamType = DataContainer_Camera.CamType.Free;

        _playerData.ActionController.Rb.isKinematic = false;
        //Stop the grinding particles
        if (_playerData.ActionController.GrindParticles != null)
        {
            _playerData.ActionController.GrindParticles.SetBool("isGrinding", false);
        }

        //Set the ignoring spline
        _playerData.ActionController.IgnoringSpline = _playerData.ActionController.Spline;

        //Initiate the cooldown
        _playerData.ActionController.IgnoreLastSpline();

        //Give back player controls
        _playerData.MovementAllowed = true;

        //reset the value from the position in the rail
        _playerData.DistanceInPath = 0;

        //Reset the gameobject
        var transform = _playerData.ActionController.transform;

        //Launch player
        _playerData.ActionController.Rb.velocity += transform.forward * Mathf.Abs(_playerData.SprintSpeed);

    }
}
