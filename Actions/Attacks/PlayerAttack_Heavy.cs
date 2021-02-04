// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttack_Heavy", menuName = "ScriptableObjects/PlayerAttacks/PlayerAttack_Heavy", order = 1)]
public class PlayerAttack_Heavy : PlayerAttacks
{
    protected override void AttackAction( Animator anim)
    {
        base.AttackAction(anim);

        anim.SetTrigger("HeavyAttack");

        AkSoundEngine.PostEvent("SFX_Jett_HeavyPunch_Action_Play", anim.gameObject);
    }
}
