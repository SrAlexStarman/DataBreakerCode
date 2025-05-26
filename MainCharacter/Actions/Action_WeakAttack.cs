// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ScriptableObject for handling the player's weak attack input and logic, integrating with the combat system.
[CreateAssetMenu(fileName = "Action_WeakAttack", menuName = "ScriptableObjects/Actions/Action_WeakAttack", order = 1)]
public class Action_WeakAttack : PlayerActions
{
    // Reference to the CombatSystem, used to queue and execute weak attacks.
    public CombatSystem CSystem;

    /// <summary>
    /// Handles input for performing a weak attack. When the input action is started, creates a new attack element and sends it to the combat system.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            // Create a new combat queue element for the weak attack.
            CombatQueueElement newAttack = new CombatQueueElement();
            newAttack.Attack = AttackType.Weak;
            newAttack.IsGrounded = _playerData.isGrounded;
            newAttack.Quality = 1;

            // Send the attack to the combat system for processing.
            CSystem.CombatInput(newAttack);
        }        
    }

    /// <summary>
    /// Executes the effect of the weak attack. (Currently empty, can be extended for visual/sound effects or additional logic.)
    /// </summary>
    public override void ActionEffect()
    {
        // Add logic for the effect of the weak attack here if needed.
    }
}