// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PlayerAttacks : ScriptableObject
{
    [SerializeField] private DataContainer_Player _playerData;
    [SerializeField] private Action_Dash _dash;
    [SerializeField] private float _attackRangeDistance;
    [SerializeField] private float _maxDashDistance;
    [SerializeField] private float _impulseForce;
    [SerializeField] private float _airImpulseForce;

    [SerializeField] private string _leftAnimBoolParameter;
    public string LeftAnimBoolParameter => _leftAnimBoolParameter;

    [SerializeField] private string _rightAnimBoolParameter;
    public string RightAnimBoolParameter => _rightAnimBoolParameter;

    public void TryAttack(KillEnemy enemy, Transform playerTrans, Animator anim)
    {
        bool b = false;

        //Dash to the enemy if it is in combat area but out of your punch range
        if(enemy != null 
        && Vector3.Distance(enemy.transform.position, playerTrans.position) > _attackRangeDistance
        && Vector3.Distance(enemy.transform.position, playerTrans.position) <= _maxDashDistance
        && b)
        {
            Vector3 endPos = enemy.transform.position + ((playerTrans.position - enemy.transform.position).normalized * _attackRangeDistance);

            Debug.Log("Dash");
            _dash.CallbackFunctions += AttackAction;
            _dash.Dash(endPos, false);
            playerTrans.LookAt(new Vector3(enemy.transform.position.x, playerTrans.position.y, enemy.transform.position.z));
        }

        //Normal attack
        else
        {
            if (enemy != null 
            && Vector3.Distance(enemy.transform.position, playerTrans.position) <= _attackRangeDistance)
            {
                playerTrans.LookAt(new Vector3(enemy.transform.position.x, playerTrans.position.y, enemy.transform.position.z));
            }   
            playerTrans.GetComponent<Rigidbody>().velocity = Vector3.zero;

            float impuls = _playerData.isGrounded? _impulseForce : _airImpulseForce;
            playerTrans.GetComponent<Rigidbody>().AddForce(playerTrans.forward * impuls, ForceMode.Impulse);

            AttackAction(anim);
        }
    }

    //End subsciptions of the action
    protected virtual void AttackAction(Animator anim)
    {
        _dash.CallbackFunctions -= AttackAction;   
    }

    private void OnDisable()
    {
        _dash.CallbackFunctions -= AttackAction;
    }
}
