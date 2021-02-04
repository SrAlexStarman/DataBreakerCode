// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public abstract class PlayerActions : ScriptableObject
{
    [SerializeField] protected DataContainer_Player _playerData;
    [SerializeField] protected DataContainer_Camera _cameraData;

    public abstract void InputEffect(InputAction.CallbackContext context);
    public abstract void ActionEffect();
}
