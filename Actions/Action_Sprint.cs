// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Action_Sprint", menuName = "ScriptableObjects/Actions/Action_Sprint", order = 1)]
public class Action_Sprint : PlayerActions
{
    public override void InputEffect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            _playerData.IsSprinting = true;
        }

        if(context.phase == InputActionPhase.Canceled)
        {
            _playerData.IsSprinting = false;
        }
    }

    public override void ActionEffect()
    {

    }

}
