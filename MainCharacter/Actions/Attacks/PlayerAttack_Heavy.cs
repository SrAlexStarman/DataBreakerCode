// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject for handling the player's heavy attack logic, triggering the appropriate animation and sound.
[CreateAssetMenu(fileName = "PlayerAttack_Heavy", menuName = "ScriptableObjects/PlayerAttacks/PlayerAttack_Heavy", order = 1)]
public class PlayerAttack_Heavy : PlayerAttacks
{
    // Triggers the heavy attack animation and plays the heavy punch sound effect.
    protected override void AttackAction( Animator anim)
    {
        base.AttackAction(anim);

        // Set the trigger for the heavy attack animation.
        anim.SetTrigger("HeavyAttack");

        // Play the heavy punch sound effect using Wwise.
        AkSoundEngine.PostEvent("SFX_Jett_HeavyPunch_Action_Play", anim.gameObject);
    }
}
