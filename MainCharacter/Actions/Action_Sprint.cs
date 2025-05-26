// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling the player's sprint input and logic, toggling sprint state based on input events.
[CreateAssetMenu(fileName = "Action_Sprint", menuName = "ScriptableObjects/Actions/Action_Sprint", order = 1)]
public class Action_Sprint : PlayerActions
{
    /// <summary>
    /// Handles input for sprinting. Sets the IsSprinting flag on the player data when input starts or is canceled.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            // Enable sprinting when input starts.
            _playerData.IsSprinting = true;
        }

        if(context.phase == InputActionPhase.Canceled)
        {
            // Disable sprinting when input is released.
            _playerData.IsSprinting = false;
        }
    }

    /// <summary>
    /// Executes the effect of the sprint action. (Currently empty, can be extended for additional logic.)
    /// </summary>
    public override void ActionEffect()
    {
        // Add logic for the effect of the sprint action here if needed.
    }

}
