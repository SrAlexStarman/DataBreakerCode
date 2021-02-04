// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Action_WeakAttack", menuName = "ScriptableObjects/Actions/Action_WeakAttack", order = 1)]
public class Action_WeakAttack : PlayerActions
{
    public CombatSystem CSystem;

    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            CombatQueueElement newAttack = new CombatQueueElement();
            newAttack.Attack = AttackType.Weak;
            newAttack.IsGrounded = _playerData.isGrounded;
            newAttack.Quality = 1;

            CSystem.CombatInput(newAttack);
        }        
    }

    public override void ActionEffect()
    {
        
    }
}