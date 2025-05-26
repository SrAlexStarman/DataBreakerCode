// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling the player's jump input and logic, including jumping from ground or while grinding rails.
[CreateAssetMenu(fileName = "Action_Jump", menuName = "ScriptableObjects/Actions/Action_Jump", order = 1)]
public class Action_Jump : PlayerActions
{
    // Reference to a BooleanSO that indicates if the game is paused.
    [SerializeField]
    public BooleanSO GamePaused;

    /// <summary>
    /// Handles input for jumping. Allows jumping if grounded or grinding, and not paused.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // Check if the player is either grounded or grinding to allow jumping.
            if (_playerData.isGrounded || _playerData.PlayerMovementState == DataContainer_Player.MovementState.Grinding)
            {
                // Final check to see if the game is paused.
                if (GamePaused.state == false)
                {
                    if (_playerData.PlayerMovementState == DataContainer_Player.MovementState.Grinding)
                    {
                        // If grinding, perform a jump off the rail.
                        JumpOffRail();
                    }
                    else
                    {
                        // If grounded, perform a normal jump.
                        Jump();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles jumping off a rail when the player is grinding.
    /// </summary>
    private void JumpOffRail()
    {
        // Set the jumped flag in the player data.
        _playerData.Jumped = true;
        // Trigger the jump animation.
        _playerData.ActionController.Anim.SetTrigger("Jump");
        // Additional logic for setting data container can be added here.
    }

    /// <summary>
    /// Handles a standard jump from the ground.
    /// </summary>
    void Jump()
    {
        // Set the jumped flag in the player data.
        _playerData.Jumped = true;
        // Apply upward force to the player's rigidbody.
        _playerData.ActionController.Rb.velocity += Vector3.up * _playerData.JumpForce;
        // Trigger the jump animation.
        _playerData.ActionController.Anim.SetTrigger("Jump");
    }

    /// <summary>
    /// Executes the effect of the jump action. (Currently empty, can be extended for additional logic.)
    /// </summary>
    public override void ActionEffect()
    {
        // Add logic for the effect of the jump action here if needed.
    }
}