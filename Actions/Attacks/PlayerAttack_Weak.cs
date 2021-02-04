// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttack_Weak", menuName = "ScriptableObjects/PlayerAttacks/PlayerAttack_Weak", order = 1)]
public class PlayerAttack_Weak : PlayerAttacks
{
    protected override void AttackAction( Animator anim)
    {
        base.AttackAction(anim);

        anim.SetTrigger("WeakAttack");

        AkSoundEngine.PostEvent("SFX_Jett_Punch_Start_Play", anim.gameObject);
    }
}
