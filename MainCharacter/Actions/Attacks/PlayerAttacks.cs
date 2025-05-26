// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract base ScriptableObject for all player attack types. Handles attack logic, dash-to-enemy logic, and animation triggers.
[System.Serializable]
public abstract class PlayerAttacks : ScriptableObject
{
    // Reference to the player's data container, used for state and physics.
    [SerializeField] private DataContainer_Player _playerData;
    // Reference to the dash action, used for dashing toward enemies.
    [SerializeField] private Action_Dash _dash;
    // Distance within which an attack can be performed.
    [SerializeField] private float _attackRangeDistance;
    // Maximum distance at which the player can dash to an enemy.
    [SerializeField] private float _maxDashDistance;
    // Impulse force applied to the player during a grounded attack.
    [SerializeField] private float _impulseForce;
    // Impulse force applied to the player during an aerial attack.
    [SerializeField] private float _airImpulseForce;

    // Animation parameter for left-handed attacks.
    [SerializeField] private string _leftAnimBoolParameter;
    public string LeftAnimBoolParameter => _leftAnimBoolParameter;

    // Animation parameter for right-handed attacks.
    [SerializeField] private string _rightAnimBoolParameter;
    public string RightAnimBoolParameter => _rightAnimBoolParameter;

    /// <summary>
    /// Attempts to perform an attack on the specified enemy. If the enemy is out of range but within dash distance, dashes to the enemy first.
    /// Otherwise, performs a normal attack with an impulse and triggers the attack animation.
    /// </summary>
    /// <param name="enemy">The enemy to attack.</param>
    /// <param name="playerTrans">The player's transform.</param>
    /// <param name="anim">The player's animator.</param>
    public void TryAttack(KillEnemy enemy, Transform playerTrans, Animator anim)
    {
        // Dash to the enemy if it is in combat area but out of your punch range.
        if(enemy != null 
        && Vector3.Distance(enemy.transform.position, playerTrans.position) > _attackRangeDistance
        && Vector3.Distance(enemy.transform.position, playerTrans.position) <= _maxDashDistance)
        {
            Vector3 endPos = enemy.transform.position + ((playerTrans.position - enemy.transform.position).normalized * _attackRangeDistance);

            Debug.Log("Dash");
            // Register callback to trigger attack after dash completes.
            _dash.CallbackFunctions += AttackAction;
            _dash.Dash(endPos, false);
            playerTrans.LookAt(new Vector3(enemy.transform.position.x, playerTrans.position.y, enemy.transform.position.z));
        }

        // Normal attack if in range.
        else
        {
            if (enemy != null 
            && Vector3.Distance(enemy.transform.position, playerTrans.position) <= _attackRangeDistance)
            {
                playerTrans.LookAt(new Vector3(enemy.transform.position.x, playerTrans.position.y, enemy.transform.position.z));
            }   
            // Stop player movement before attack.
            playerTrans.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // Apply impulse based on grounded or air state.
            float impuls = _playerData.isGrounded? _impulseForce : _airImpulseForce;
            playerTrans.GetComponent<Rigidbody>().AddForce(playerTrans.forward * impuls, ForceMode.Impulse);

            // Trigger the attack animation and logic.
            AttackAction(anim);
        }
    }

    /// <summary>
    /// Virtual method to trigger the attack animation. Can be overridden by subclasses for specific attack types.
    /// Also used to unregister the callback after a dash attack.
    /// </summary>
    protected virtual void AttackAction(Animator anim)
    {
        _dash.CallbackFunctions -= AttackAction;   
    }

    /// <summary>
    /// Ensures that the callback is unregistered when this ScriptableObject is disabled.
    /// </summary>
    private void OnDisable()
    {
        _dash.CallbackFunctions -= AttackAction;
    }
}
