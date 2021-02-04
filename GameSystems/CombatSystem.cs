// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

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

    //private Queue<CombatQueueElement> _combatQueue;
    [ReadOnly] public int _comboIndex = 0;

    internal Transform PlayerTransf;
    internal Animator Anim;
    private float _animTime = 0;
    [ReadOnly] [SerializeField] private float _resetComboTime = 0;
    [ReadOnly] [SerializeField] private float _marginComboTime = 0;

    private bool _chainedAnimation = false;

    [SerializeField] private ShakePreset _smallSmashShake;
    [SerializeField] private ShakePreset _bigSmashShake;

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

    public void OnUpdate()
    {
        if(_resetCombo == true)
        {
            _resetComboTime -= Time.deltaTime;

            if(_resetComboTime <= 0)
            {
                _attackAllowed = true;
                _resetCombo = false;
            }
        }

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

        KarnageTimer();
        _playerData.AnimForwardMovement = Anim.GetFloat("ForwardMovement");
    }

    public void CombatInput(CombatQueueElement element)
    {
        Debug.Log($"ComboIndex: {_comboIndex} || MaxCombo: {_maxCombo}");

        if(_attackAllowed == true)
        {
            _comboIndex++;
            Anim.SetInteger("AttackIndex", _comboIndex);

            _playerData.ActionController.Rb.useGravity = false;
            _playerData.ActionController.Rb.velocity = Vector3.zero;

            _marginCombo = false;
            _marginComboTime = _playerData.TimeToResetCombo;

            if(element.Attack == AttackType.Weak)
                _weakAttack.TryAttack(_cameraData.EnemyTargeted, PlayerTransf, Anim);

            else if(element.Attack == AttackType.Heavy)
                _heavyAttack.TryAttack(_cameraData.EnemyTargeted, PlayerTransf, Anim); 

            _attackAllowed = false;
            _chainedAnimation = true;
        }
    }

    public void BeginAttack()
    {
        Debug.Log("BEGIN EVENT");
        _playerData.MovementAllowed = false;
        _playerData.AnimOverrideMovement = true;
    }

    public void NextAttack()
    {
        if(_comboIndex < _maxCombo)
        {
            _attackAllowed = true;
            _chainedAnimation = false;
        }
    }

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
            _marginCombo = true;
        }
    }

    public void ResetCombat()
    {
        _comboIndex = 0;
        _attackAllowed = false;
        _resetCombo = false;
        _marginCombo = false;
        _playerData.AnimOverrideMovement = false;
    }

    public void AllowAttack()
    {
        _attackAllowed = true;
        _playerData.ActionController.Rb.useGravity = true;
        _playerData.MovementAllowed = true;
    }

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

    public void SmashGround(bool isSmall)
    {
        float range = isSmall? _playerData.SmallSmashRange : _playerData.BigSmashRange;
        ShakePreset currentShake = isSmall? _smallSmashShake : _bigSmashShake;
        Shaker.ShakeAll(currentShake);
        _playerData.ActionController.SmashEffect.transform.position = _playerData.ActionController.Transf.position - Vector3.up * 0.8f;
        _playerData.ActionController.SmashEffect.SetActive(true);
        _playerData.ActionController.SmashEffect.GetComponent<Renderer>().sharedMaterial.SetFloat("Timer", - 0.44f);
        Vector3 playerPos = _playerData.ActionController.Transf.position;

        //check all the enemies in the area
        foreach(KillEnemy enemy in KillEnemy.EnemiesList)
        {
            if((enemy.transform.position.y > playerPos.y - 1f && enemy.transform.position.y < playerPos.y + 1f) && Vector3.Distance(playerPos, enemy.transform.position) <= range)
            {
                RaycastHit hit;

                if(Physics.Raycast(playerPos, (enemy.transform.position - playerPos).normalized, out hit, Mathf.Infinity) && hit.transform.gameObject == enemy.gameObject)
                {
                    enemy.GetComponent<Enemy>().OnHit(10);
                }
            }
        }
    }

    public void KarnageTimer()
    {
        if(_playerData.IsKarnageON == false)
        {
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
            _playerData.KarnagePoints -= _playerData.DecreaseKarnagePointSpeed * Time.deltaTime;
            
            if(_playerData.KarnagePoints <= 0)
            {
                _playerData.IsKarnageON = false;
            }
        }
    }
}

public struct CombatQueueElement
{
    public AttackType Attack;
    public bool IsGrounded;
    public int Quality;
    public int Index;
}

public enum AttackType{
    Weak,
    Heavy
}