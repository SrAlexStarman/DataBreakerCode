// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

// ScriptableObject that manages player combat, combos, and attack logic.
[CreateAssetMenu(fileName = "System_Combat", menuName = "ScriptableObjects/GameSystems/System_Combat", order = 1)]
public class CombatSystem : ScriptableObject
{
    [SerializeField] private DataContainer_Player _playerData;
    [SerializeField] private DataContainer_Camera _cameraData;
    [SerializeField] private Action_HeavyAttack _heavyInput;
    [SerializeField] private Action_WeakAttack _weakInput;
    [SerializeField] private PlayerAttack_Weak _weakAttack;
    [SerializeField] private PlayerAttack_Heavy _heavyAttack;

    private BoxCollider _leftCollider;
    private BoxCollider _rightCollider;

    [ReadOnly] [SerializeField] private bool _attackAllowed = true;
    [ReadOnly] [SerializeField] private bool _resetCombo = false;
    [ReadOnly] [SerializeField] private bool _marginCombo = false;

    public int _maxCombo;

    [SerializeField] private CameraType_Targetted _targetting;

    [ReadOnly] public int _comboIndex = 0;

    internal Transform PlayerTransf;
    internal Animator Anim;
    private float _animTime = 0;
    [ReadOnly] [SerializeField] private float _resetComboTime = 0;
    [ReadOnly] [SerializeField] private float _marginComboTime = 0;

    private bool _chainedAnimation = false;

    [SerializeField] private ShakePreset _smallSmashShake;
    [SerializeField] private ShakePreset _bigSmashShake;

    // Called at the start of the game to initialize combat variables and references.
    public void OnStart(BoxCollider leftCollider, BoxCollider rightCollider)
    {
        _comboIndex = 0;
        _playerData.KarnageTime = _playerData.MaxKarnageTimer;
        _playerData.KarnageCombo = 0;
        _playerData.IsRestTime = true;
        _playerData.RestTime = _playerData.TimeBeforeDecrease;
        _playerData.IsKarnageON = false;
        _playerData.KarnagePoints = 0;
        
        //_combatQueue = new Queue<CombatQueueElement>();
        _leftCollider = leftCollider;
        _rightCollider = rightCollider;

        _attackAllowed = true;
        _playerData.AnimOverrideMovement = false;
    }

    // Called every frame to update combo timers and player states.
    public void OnUpdate()
    {
        // Handle combo reset timer.
        if(_resetCombo == true)
        {
            _resetComboTime -= Time.deltaTime;

            if(_resetComboTime <= 0)
            {
                _attackAllowed = true;
                _resetCombo = false;
            }
        }

        // Handle margin time for chaining combos.
        if(_marginCombo == true)
        {
            _marginComboTime -= Time.deltaTime;

            if(_marginComboTime <= 0)
            {
                _marginCombo = false;
                _comboIndex = 0;
                _playerData.MovementAllowed = true;
            }
        }

        // Update the Karnage timer and forward movement parameter for animation.
        KarnageTimer();
        _playerData.AnimForwardMovement = Anim.GetFloat("ForwardMovement");
    }

    // Handles player input for combat, processes combo logic and triggers attacks.
    public void CombatInput(CombatQueueElement element)
    {
        Debug.Log($"ComboIndex: {_comboIndex} || MaxCombo: {_maxCombo}");

        if(_attackAllowed == true)
        {
            // Increment combo index and update animation.
            _comboIndex++;
            Anim.SetInteger("AttackIndex", _comboIndex);

            // Disable gravity and stop player movement during attack.
            _playerData.ActionController.Rb.useGravity = false;
            _playerData.ActionController.Rb.velocity = Vector3.zero;

            // Reset margin combo timer.
            _marginCombo = false;
            _marginComboTime = _playerData.TimeToResetCombo;

            // Trigger the appropriate attack based on input type.
            if(element.Attack == AttackType.Weak)
                _weakAttack.TryAttack(_cameraData.EnemyTargeted, PlayerTransf, Anim);

            else if(element.Attack == AttackType.Heavy)
                _heavyAttack.TryAttack(_cameraData.EnemyTargeted, PlayerTransf, Anim); 

            // Prevent further attacks until allowed.
            _attackAllowed = false;
            _chainedAnimation = true;
        }
    }

    // Called at the start of an attack animation. Disables movement and overrides movement for animation.
    public void BeginAttack()
    {
        Debug.Log("BEGIN EVENT");
        // Disable movement and override movement for animation.
        _playerData.MovementAllowed = false;
        _playerData.AnimOverrideMovement = true;
    }

    // Called to continue to the next attack in the combo if possible.
    // This method is used to continue to the next attack in the combo if possible.
    public void NextAttack()
    {
        // Check if the combo index is less than the maximum combo.
        if(_comboIndex < _maxCombo)
        {
            // Allow the next attack and reset the chained animation flag.
            _attackAllowed = true;
            _chainedAnimation = false;
        }
    }

    // Called at the end of an attack animation. Handles combo reset and movement re-enabling.
    public void EndAttack()
    {
        Debug.Log("END EVENT");
        _playerData.ActionController.Rb.useGravity = true;

        if(_chainedAnimation == false)
        {
            _playerData.AnimOverrideMovement = false;
        }

        if(_comboIndex >= _maxCombo)
        {
            Debug.Log("END COMBO");
            _comboIndex = 0;
            _resetCombo = true;
            _resetComboTime = _playerData.TimeBetweenCombos;
            _playerData.MovementAllowed = true;
        }
        else
        {
            // Allow a margin for chaining another attack.
            _marginCombo = true;
        }
    }

    // Resets all combat variables and disables attack.
    public void ResetCombat()
    {
        _comboIndex = 0;
        _attackAllowed = false;
        _resetCombo = false;
        _marginCombo = false;
        _playerData.AnimOverrideMovement = false;
    }

    // Allows the player to attack again and re-enables movement/gravity.
    public void AllowAttack()
    {
        _attackAllowed = true;
        _playerData.ActionController.Rb.useGravity = true;
        _playerData.MovementAllowed = true;
    }

    // Enable/disable left and right attack colliders for hit detection.
    public void ActivateLeftCollider()
    {
        _leftCollider.enabled = true;
    }

    public void ActivateRightCollider()
    {
        _rightCollider.enabled = true;
    }

    public void DeactivateLeftCollider()
    {
        _leftCollider.enabled = false;
    }

    public void DeactivateRightCollider()
    {
        _rightCollider.enabled = false;
    }

    // Performs a ground smash attack, applies camera shake, and damages nearby enemies.
    public void SmashGround(bool isSmall)
    {
        float range = isSmall? _playerData.SmallSmashRange : _playerData.BigSmashRange;
        ShakePreset currentShake = isSmall? _smallSmashShake : _bigSmashShake;
        Shaker.ShakeAll(currentShake);
        _playerData.ActionController.SmashEffect.transform.position = _playerData.ActionController.Transf.position - Vector3.up * 0.8f;
        _playerData.ActionController.SmashEffect.SetActive(true);
        _playerData.ActionController.SmashEffect.GetComponent<Renderer>().sharedMaterial.SetFloat("Timer", - 0.44f);
        Vector3 playerPos = _playerData.ActionController.Transf.position;

        // Check all the enemies in the area for damage.
        foreach(KillEnemy enemy in KillEnemy.EnemiesList)
        {
            // Only affect enemies within range and roughly at the same height.
            if((enemy.transform.position.y > playerPos.y - 1f && enemy.transform.position.y < playerPos.y + 1f) && Vector3.Distance(playerPos, enemy.transform.position) <= range)
            {
                RaycastHit hit;

                // Ensure there are no obstacles between player and enemy.
                if(Physics.Raycast(playerPos, (enemy.transform.position - playerPos).normalized, out hit, Mathf.Infinity) && hit.transform.gameObject == enemy.gameObject)
                {
                    enemy.GetComponent<Enemy>().OnHit(10);
                }
            }
        }
    }

    // Manages the Karnage timer, which controls special combat states and point decay.
    public void KarnageTimer()
    {
        if(_playerData.IsKarnageON == false)
        {
            // While not in Karnage mode, count down combo and rest timers.
            if(_playerData.KarnageCombo > 0)
            {
                if(_playerData.IsRestTime)
                {
                    _playerData.RestTime -= Time.deltaTime;
                    
                    if(_playerData.RestTime <= 0)
                    {
                        _playerData.IsRestTime = false;
                    }
                }

                else
                {
                    _playerData.KarnageTime -= Time.deltaTime;

                    if(_playerData.KarnageTime <= 0)
                    {
                        _playerData.KarnageCombo--;
                        _playerData.KarnageTime = _playerData.MaxKarnageTimer;
                    }
                }
            }
        }
        else
        {
            // In Karnage mode, decrease Karnage points over time.
            _playerData.KarnagePoints -= _playerData.DecreaseKarnagePointSpeed * Time.deltaTime;
            
            if(_playerData.KarnagePoints <= 0)
            {
                _playerData.IsKarnageON = false;
            }
        }
    }
}

// Struct representing an element in the combat queue, used to process attacks.
public struct CombatQueueElement
{
    public AttackType Attack; // Type of attack (Weak/Heavy)
    public bool IsGrounded;   // Whether the player is grounded
    public int Quality;       // Quality of the attack (for scoring)
    public int Index;         // Index in the combo
}

// Enum for different attack types.
public enum AttackType{
    Weak,
    Heavy
}