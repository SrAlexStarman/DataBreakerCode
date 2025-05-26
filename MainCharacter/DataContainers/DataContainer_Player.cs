// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all player-related parameters, references, and state for the main character.
/// </summary>
[CreateAssetMenu(fileName = "DataContainer_Player", menuName = "ScriptableObjects/DataContainer/DataContainer_Player", order = 1)]
public class DataContainer_Player : ScriptableObject
{
    /// <summary>
    /// Player movement state enumeration.
    /// </summary>
    public enum MovementState{
        Stopped,
        Moving,
        Running,
        Grinding
    }

    [Header("Current Player Reference")]
    /// <summary>
    /// Reference to the player's action controller script.
    /// </summary>
    public ActionController ActionController;

    [Header("Player Status")]
    /// <summary>
    /// Maximum health value for the player.
    /// </summary>
    public float MaxHealth;
    /// <summary>
    /// Current health value (runtime).
    /// </summary>
    [ReadOnly] public float CurrentHealth;
    /// <summary>
    /// Whether the player is currently invincible.
    /// </summary>
    public bool IsInvinsible;

    [Header("Action Controller Parameters")]
    [ReadOnly] public MovementState PlayerMovementState;
    [ReadOnly] public bool GrindingBackwards;
    [ReadOnly] public float DistanceInPath;
    [ReadOnly] public bool IsSprinting;
    [ReadOnly] public bool isGrounded;

    [ReadOnly] public bool Jumped;

    [Header("Movement Parameters")]
    public float Speed;
    public float SprintSpeed;
    public float AccelerationLerp;
    public float RotationSpeed;
    public float AirSpeed;
    [ReadOnly] public float AnimForwardMovement;
    [ReadOnly] public bool AnimOverrideMovement;
    [ReadOnly] public bool MovementAllowed;
    [ReadOnly] public Vector3 DirectionVector;

    [Header("Jump Parameters")]
    public float JumpForce;

    [Header("Dash Parameters")]
    public float DashSpeed;
    public float DashDistance;
    public float TimeBetweenDashes;
    public CurveType DashCurveType;

    [Header("Grappling Parameters")]
    public float TargetRadius;
    public float GrapplingSpeed;
    public float GrapRange;

    [Header("Grind Parameters")]
    [Range(1f, 20f)]
    public float GrindSpeed;

    /// <summary>
    /// Reduces player health by the specified damage amount unless invincible.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public void OnHit(float damage)
    {
        if(IsInvinsible==false)
        {
            CurrentHealth -= damage;
        }
    }

    [Header("Attack Parameters")]
    public float TimeBetweenCombos;
    public float TimeToResetCombo;
    public float SmallSmashRange;
    public float BigSmashRange;

    [Header("Karnage Parameters")]
    [ReadOnly] public int KarnageCombo;
    [ReadOnly] public float KarnageTime;
    [ReadOnly] public bool IsRestTime;
    [ReadOnly] public float RestTime;
    [ReadOnly] public bool IsKarnageON;
    [ReadOnly] public float KarnagePoints;
    public float DecreaseKarnagePointSpeed;
    public int MaxKarnagePoints;
    public int MaxKarnageCombo;
    public int MaxKarnageTimer;
    public int TimeBeforeDecrease;
    public float TimerAcceleration;
}
