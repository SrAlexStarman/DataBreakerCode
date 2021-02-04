// Copyright (C) 2020 Alejandro Lopez, All Rights Reserved 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PunchCollision : MonoBehaviour
{
    [SerializeField] private DataContainer_Player _playerData;
    [SerializeField] private VisualEffect _particles;

    [SerializeField] private PlayerAttacks[] _attack;
    [SerializeField] float _damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Breakable"))
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -transform.right, out hit, Mathf.Infinity);

            if (_particles != null)
            {
                VisualEffect vfxInstance = Instantiate(_particles);
                vfxInstance.transform.position = hit.point;
                Destroy(vfxInstance, 1);
            }

            var enemy = other.GetComponent<Enemy>();

            if (enemy)
            {
                other.attachedRigidbody.AddForceAtPosition(-transform.right * 10f, hit.point, ForceMode.Impulse);

                enemy.OnHit(_damageAmount);


                //Play the sound
                AkSoundEngine.SetSwitch("Hit", enemy.Stats.enemyName, gameObject);
                AkSoundEngine.PostEvent("SFX_Jett_Punch_Hit_Play", gameObject);

                
                GetComponent<BoxCollider>().enabled = false;
            }

            Breakable breakable = other.GetComponent<Breakable>();
            
            if (breakable != null)
            {
                breakable.Smash();
            }
        }
    }
}
