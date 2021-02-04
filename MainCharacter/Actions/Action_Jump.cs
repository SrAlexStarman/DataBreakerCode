// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Action_Jump", menuName = "ScriptableObjects/Actions/Action_Jump", order = 1)]
public class Action_Jump : PlayerActions
{
    [SerializeField]
    public BooleanSO GamePaused;

    public override void InputEffect(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            //We check if we are either grinding or grounded to perform the jump
            if (_playerData.isGrounded || _playerData.PlayerMovementState == DataContainer_Player.MovementState.Grinding)
            {
                //Final check to see if the game is paused
                if (GamePaused.state == false)
                {
                    if (_playerData.PlayerMovementState == DataContainer_Player.MovementState.Grinding)
                    {
                        JumpOffRail();
                    }
                    else
                    {
                        Jump();
                    }
                }
            }
        }
    }

    private void JumpOffRail()
    {
        //Set the flag
        _playerData.Jumped = true;
        
        _playerData.ActionController.Anim.SetTrigger("Jump");

        //Set the data container
        
    }

    void Jump()
    {
        //Set the data container
        _playerData.Jumped = true;
        _playerData.ActionController.Rb.velocity += Vector3.up * _playerData.JumpForce;
        _playerData.ActionController.Anim.SetTrigger("Jump");

    }

    public override void ActionEffect()
    {

    }
}