// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages all checkpoints in the level and handles player respawn.
public class CheckPointSystem : MonoBehaviour
{
    [SerializeField] private CheckPoint[] _checkPoints;
    [SerializeField] private DataContainer_Player _playerData;
    [SerializeField] private Action_Dash _dashAction;

    private int _checkPointIndex = 0;

    // Initialize checkpoints and assign this system to each checkpoint.
    private void Awake()
    {
        foreach(var item in _checkPoints)
        {
            item.SetCheckPointSystem(this);
        }
    }

    // On start, respawn the player at the current checkpoint.
    private void Start()
    {
        Respawn();
    }

    // Advance to the next checkpoint in the array.
    public void NextCheckPoint()
    {
        _checkPointIndex++;
    }

    // Respawn the player at the current checkpoint position.
    public void Respawn()
    {
        // Move player to the checkpoint position.
        _playerData.ActionController.Transf.position = _checkPoints[_checkPointIndex].transform.position;

        // If the player is invincible (e.g., dashing), end the dash.
        if(_playerData.IsInvinsible)
        {
            _dashAction.EndDash();
        }
    }
}
