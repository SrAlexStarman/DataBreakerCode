// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling the player's heavy attack input and logic, integrating with the combat system.
[CreateAssetMenu(fileName = "Action_HeavyAttack", menuName = "ScriptableObjects/Actions/Action_HeavyAttack", order = 1)]
public class Action_HeavyAttack : PlayerActions
{
    // Reference to the CombatSystem, used to queue and execute heavy attacks.
    public CombatSystem CSystem;

    /// <summary>
    /// Handles input for performing a heavy attack. When the input action is started, creates a new attack element and sends it to the combat system.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            // Create a new combat queue element for the heavy attack.
            CombatQueueElement newAttack = new CombatQueueElement();
            newAttack.Attack = AttackType.Heavy;
            newAttack.IsGrounded = _playerData.isGrounded;
            newAttack.Quality = 1;

            // Send the attack to the combat system for processing.
            CSystem.CombatInput(newAttack);
        }        
    }

    /// <summary>
    /// Executes the effect of the heavy attack. (Currently empty, can be extended for visual/sound effects or additional logic.)
    /// </summary>
    public override void ActionEffect()
    {
        // Add logic for the effect of the heavy attack here if needed.
    }
}