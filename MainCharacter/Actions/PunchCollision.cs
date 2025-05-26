// Copyright (C) 2025 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Handles punch collision detection, applying damage and effects to enemies and breakable objects.
/// </summary>
public class PunchCollision : MonoBehaviour
{
    // Reference to the player data container.
    [SerializeField] private DataContainer_Player _playerData;
    // Particle effect to spawn upon successful punch.
    [SerializeField] private VisualEffect _particles;

    // Array of possible player attacks (not used directly in this script).
    [SerializeField] private PlayerAttacks[] _attack;
    // Amount of damage dealt by the punch.
    [SerializeField] float _damageAmount;

    /// <summary>
    /// Handles collision with enemies and breakable objects, applying damage, force, effects, and sounds.
    /// </summary>
    /// <param name="other">The collider that was hit by the punch.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Only react to enemies or breakable objects.
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Breakable"))
        {
            RaycastHit hit;
            // Cast a ray to determine the hit point for effects and force application.
            Physics.Raycast(transform.position, -transform.right, out hit, Mathf.Infinity);

            // Spawn punch particle effect at the hit point, if available.
            if (_particles != null)
            {
                VisualEffect vfxInstance = Instantiate(_particles);
                vfxInstance.transform.position = hit.point;
                Destroy(vfxInstance, 1);
            }

            var enemy = other.GetComponent<Enemy>();

            if (enemy)
            {
                // Apply force to the enemy at the hit point.
                other.attachedRigidbody.AddForceAtPosition(-transform.right * 10f, hit.point, ForceMode.Impulse);

                // Apply damage to the enemy.
                enemy.OnHit(_damageAmount);

                // Play punch hit sound with appropriate switch.
                AkSoundEngine.SetSwitch("Hit", enemy.Stats.enemyName, gameObject);
                AkSoundEngine.PostEvent("SFX_Jett_Punch_Hit_Play", gameObject);

                // Disable the collider after a successful punch to prevent multiple triggers.
                GetComponent<BoxCollider>().enabled = false;
            }

            Breakable breakable = other.GetComponent<Breakable>();
            
            if (breakable != null)
            {
                // Trigger the breakable object's smash logic.
                breakable.Smash();
            }
        }
    }
}
