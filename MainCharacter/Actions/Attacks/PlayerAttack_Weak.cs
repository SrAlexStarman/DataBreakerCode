// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject for handling the player's weak attack logic, triggering the appropriate animation and sound.
[CreateAssetMenu(fileName = "PlayerAttack_Weak", menuName = "ScriptableObjects/PlayerAttacks/PlayerAttack_Weak", order = 1)]
public class PlayerAttack_Weak : PlayerAttacks
{
    // Triggers the weak attack animation and plays the punch sound effect.
    protected override void AttackAction( Animator anim)
    {
        base.AttackAction(anim);

        // Set the trigger for the weak attack animation.
        anim.SetTrigger("WeakAttack");

        // Play the punch sound effect using Wwise.
        AkSoundEngine.PostEvent("SFX_Jett_Punch_Start_Play", anim.gameObject);
    }
}
