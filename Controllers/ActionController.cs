// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using UnityEngine;
using SplineMesh;
using MilkShake;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class ActionController : MonoBehaviour
{
    [SerializeField] private DataContainer_Player _playerData;
    [SerializeField] private DataContainer_Camera _cameraData;

    [SerializeField]
    private Animator _anim;
    public Animator Anim => _anim;

    [SerializeField]
    private PlayerActions[] _actionsUpdated;

    [SerializeField]
    private PlayerActions[] _actionsNoUpdated;

    [SerializeField]
    private LayerMask _groundCheckLayerMask;

    public GameObject DashParticles;
    public GameObject Hand;

    public VisualEffect GrindParticles;

    //Rail stuff
    Spline _currentSpline;
    Spline _ignoringSpline;
    public Spline Spline { get { return _currentSpline; } internal set { _currentSpline = value; } }
    public Spline IgnoringSpline { get { return _ignoringSpline; } internal set { _ignoringSpline = value; } }

    //Player Components
    [ReadOnly] public Transform Transf;
    public Rigidbody Rb { get; private set; }
    private CapsuleCollider _capsuleCollider;

    [SerializeField] private ParticleSystem _landParticle1; 
    [SerializeField] private ParticleSystem _landParticle2; 

    public GameObject SmashEffect;
    //carnage effect

    private CarnageEffectHandler _carnageEffectHandler;

    [SerializeField] private ShakePreset _landShake;

    private void Awake()
    {
        _playerData.ActionController = this;

        Transf = GetComponent<Transform>();
        Rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _carnageEffectHandler = GetComponent<CarnageEffectHandler>();
    }

    void Start()
    {
        _playerData.MovementAllowed = true;
        _playerData.PlayerMovementState = DataContainer_Player.MovementState.Stopped;
    }

    void FixedUpdate()
    {
        IsGrounded();

        foreach (var action in _actionsUpdated)
        {
            action.ActionEffect();
        }
    }

    private void IsGrounded()
    {
        RaycastHit raycastHit;

        bool lastState = _playerData.isGrounded;

        Physics.SphereCast(transform.position, _capsuleCollider.radius - 0.2f, Vector3.down, out raycastHit, (_capsuleCollider.height) * 0.60f, _groundCheckLayerMask);

        _playerData.isGrounded = raycastHit.collider == null ? false : true;
        Anim.SetBool("OnAir", !_playerData.isGrounded);

        if(_playerData.isGrounded == false) 
        {
            Transf.rotation = Quaternion.Euler(0, Transf.rotation.eulerAngles.y, 0);
        }

        //Set the jumped flag
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

    bool IsGoingBackwards(Vector3 segmentFoward)
    {
        var dot = Vector3.Dot(transform.forward, segmentFoward);
        return dot < 0.1f;
    }

    public void IgnoreLastSpline()
    {
        _currentSpline = null;
        StartCoroutine("CoolDownIgnoreSpline");
    }

    IEnumerator CoolDownIgnoreSpline()
    {
        yield return new WaitForSeconds(1f);
        _ignoringSpline = null;
    }

    private void OnCollisionEnter(Collision collision)
    {

        //If we are already in a rail quick return
        if (_currentSpline != null) return;

        //Get the segment we just collided with
        var segment = collision.gameObject.GetComponent<Segment>();
        if (segment)
        {   
            //Get the segment spline
            var spline = segment.GetComponentInParent<Spline>();
            if (spline)
            {
                //If we a spline that should be ignored return
                if (spline == _ignoringSpline) return;

                //Set jumped flag so that we can jump out of the rail
                _playerData.Jumped = false;

                //Play rail Sound
                AkSoundEngine.PostEvent("SFX_Jett_Rail_Play", gameObject);

                //Check  where we are in the rail
                var sample = spline.GetProjectionSample(segment.transform.position);

                //Store the distance in the rail that we currently are
                _playerData.DistanceInPath = segment._distanceInSpline;

                //Check if we are going backwards
                var forward = segment.transform.forward;
                _playerData.GrindingBackwards = IsGoingBackwards(forward);

                //place ourselves in the spline
                _currentSpline = spline;


            }
        }
    }
}
