// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script represents a checkpoint in the game. When the player collides with it, it triggers the next checkpoint and disables itself.
[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour
{
    private CheckPointSystem _checkPointSystem;
    private BoxCollider _boxCollider;
    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Sets the checkpoint system that this checkpoint belongs to.
    public void SetCheckPointSystem(CheckPointSystem CheckPointSystem)
    {
        _checkPointSystem = CheckPointSystem;
    }

    // Triggered when another collider enters this checkpoint's collider.
    private void OnTriggerEnter(Collider other)
    {
        // Only respond if the player enters the checkpoint.
        if(other.CompareTag("Player"))
        {
            // Notify the checkpoint system to proceed to the next checkpoint.
            _checkPointSystem.NextCheckPoint();
            // Disable this checkpoint so it can't be triggered again.
            _boxCollider.enabled = false;
        }
    }
}
