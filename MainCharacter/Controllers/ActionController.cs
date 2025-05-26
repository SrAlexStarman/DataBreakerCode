// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using UnityEngine;
using SplineMesh;
using MilkShake;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
/// <summary>
/// Handles player actions, movement, rail grinding, collision, and animation state management for the main character.
/// </summary>
public class ActionController : MonoBehaviour
{
    // Reference to the player data container.
    [SerializeField] private DataContainer_Player _playerData;
    // Reference to the camera data container.
    [SerializeField] private DataContainer_Camera _cameraData;

    // Animator component for handling player animations.
    [SerializeField]
    private Animator _anim;
    // Public accessor for the animator.
    public Animator Anim => _anim;

    // Actions that are updated every frame.
    [SerializeField]
    private PlayerActions[] _actionsUpdated;
    // Actions that are not updated every frame.
    [SerializeField]
    private PlayerActions[] _actionsNoUpdated;

    // Layer mask used for ground checking.
    [SerializeField]
    private LayerMask _groundCheckLayerMask;

    // Particle effects and references for dash and hand objects.
    public GameObject DashParticles;
    public GameObject Hand;
    public VisualEffect GrindParticles;

    // Rail grinding related fields.
    Spline _currentSpline;
    Spline _ignoringSpline;
    public Spline Spline { get { return _currentSpline; } internal set { _currentSpline = value; } }
    public Spline IgnoringSpline { get { return _ignoringSpline; } internal set { _ignoringSpline = value; } }

    // Player component references.
    [ReadOnly] public Transform Transf;
    public Rigidbody Rb { get; private set; }
    private CapsuleCollider _capsuleCollider;

    // Particle systems for landing effects.
    [SerializeField] private ParticleSystem _landParticle1; 
    [SerializeField] private ParticleSystem _landParticle2; 

    // Visual effect for smash action.
    public GameObject SmashEffect;
    // Handler for carnage effects.
    private CarnageEffectHandler _carnageEffectHandler;

    // Camera shake preset for landing.
    [SerializeField] private ShakePreset _landShake;

    /// <summary>
    /// Initializes references to components and sets up the player data container link.
    /// </summary>
    private void Awake()
    {
        _playerData.ActionController = this;

        Transf = GetComponent<Transform>();
        Rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _carnageEffectHandler = GetComponent<CarnageEffectHandler>();
    }

    /// <summary>
    /// Sets initial player movement state and allows movement at the start of the game.
    /// </summary>
    void Start()
    {
        _playerData.MovementAllowed = true;
        _playerData.PlayerMovementState = DataContainer_Player.MovementState.Stopped;
    }

    /// <summary>
    /// Handles physics updates: checks if the player is grounded and updates all relevant actions.
    /// </summary>
    void FixedUpdate()
    {
        IsGrounded();

        foreach (var action in _actionsUpdated)
        {
            action.ActionEffect();
        }
    }

    /// <summary>
    /// Checks if the player is grounded using a sphere cast and updates animation and landing effects.
    /// </summary>
    private void IsGrounded()
    {
        RaycastHit raycastHit;

        bool lastState = _playerData.isGrounded;

        // Perform a sphere cast to check for ground beneath the player.
        Physics.SphereCast(transform.position, _capsuleCollider.radius - 0.2f, Vector3.down, out raycastHit, (_capsuleCollider.height) * 0.60f, _groundCheckLayerMask);

        _playerData.isGrounded = raycastHit.collider == null ? false : true;
        Anim.SetBool("OnAir", !_playerData.isGrounded);

        // If not grounded, reset the Z rotation to prevent tipping over.
        if(_playerData.isGrounded == false) 
        {
            Transf.rotation = Quaternion.Euler(0, Transf.rotation.eulerAngles.y, 0);
        }

        // If just landed, reset jump and play landing effects.
        if(_playerData.isGrounded == true && lastState == false)
        {
            _playerData.Jumped = false;
            _landParticle1.gameObject.transform.position = _playerData.ActionController.Transf.position - Vector3.up * 0.8f;
            _landParticle2.gameObject.transform.position = _playerData.ActionController.Transf.position - Vector3.up * 0.8f;
            _landParticle1.Play();
            _landParticle2.Play();
            Shaker.ShakeAll(_landShake);
        }
    }

    /// <summary>
    /// Determines if the player is moving backwards relative to a given segment's forward vector.
    /// </summary>
    /// <param name="segmentFoward">The forward vector of the segment.</param>
    /// <returns>True if moving backwards, otherwise false.</returns>
    bool IsGoingBackwards(Vector3 segmentFoward)
    {
        var dot = Vector3.Dot(transform.forward, segmentFoward);
        return dot < 0.1f;
    }

    /// <summary>
    /// Ignores the last used rail spline and starts a cooldown before it can be used again.
    /// </summary>
    public void IgnoreLastSpline()
    {
        _currentSpline = null;
        StartCoroutine("CoolDownIgnoreSpline");
    }

    /// <summary>
    /// Coroutine that waits for a short cooldown before re-enabling the previously ignored spline.
    /// </summary>
    IEnumerator CoolDownIgnoreSpline()
    {
        yield return new WaitForSeconds(1f);
        _ignoringSpline = null;
    }

    /// <summary>
    /// Handles collision logic for entering rail segments and starting grind mechanics.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    private void OnCollisionEnter(Collision collision)
    {
        // If already on a rail, do nothing.
        if (_currentSpline != null) return;

        // Try to get the collided segment.
        var segment = collision.gameObject.GetComponent<Segment>();
        if (segment)
        {   
            // Get the parent spline of the segment.
            var spline = segment.GetComponentInParent<Spline>();
            if (spline)
            {
                // Ignore the spline if it is currently being ignored.
                if (spline == _ignoringSpline) return;

                // Set jump flag to false when landing on a rail.
                _playerData.Jumped = false;

                // Play rail sound effect.
                AkSoundEngine.PostEvent("SFX_Jett_Rail_Play", gameObject);

                // Find where on the spline the player landed.
                var sample = spline.GetProjectionSample(segment.transform.position);

                // Store the distance on the rail.
                _playerData.DistanceInPath = segment._distanceInSpline;

                // Check if grinding backwards.
                var forward = segment.transform.forward;
                _playerData.GrindingBackwards = IsGoingBackwards(forward);

                // Set current spline reference.
                _currentSpline = spline;
            }
        }
    }
}
