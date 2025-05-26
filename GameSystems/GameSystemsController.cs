// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using UnityEngine;

// Controls initialization and updating of core game systems such as combat.
public class GameSystemsController : MonoBehaviour
{
    [SerializeField] private BoxCollider _leftPunchCollider;
    [SerializeField] private BoxCollider _rightPunchCollider;
    [SerializeField] private Animator _playerAnim;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private CombatSystem _combatSystem;

    // Initialize combat system with references to colliders, animator, and transform.
    void Start()
    {
        _combatSystem.OnStart(_leftPunchCollider, _rightPunchCollider);
        _combatSystem.Anim = _playerAnim;
        _combatSystem.PlayerTransf = _playerTransform;
    }

    // Update the combat system each frame.
    private void Update()
    {
        _combatSystem.OnUpdate();
    }
}