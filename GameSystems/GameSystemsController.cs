using UnityEngine;

public class GameSystemsController : MonoBehaviour
{
    [SerializeField] private BoxCollider _leftPunchCollider;
    [SerializeField] private BoxCollider _rightPunchCollider;
    [SerializeField] private Animator _playerAnim;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private CombatSystem _combatSystem;

    void Start()
    {
        _combatSystem.OnStart(_leftPunchCollider, _rightPunchCollider);
        _combatSystem.Anim = _playerAnim;
        _combatSystem.PlayerTransf = _playerTransform;
    }

    private void Update()
    {
        _combatSystem.OnUpdate();
    }
}