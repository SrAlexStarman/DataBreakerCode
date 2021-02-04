using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour
{
    private CheckPointSystem _checkPointSystem;

    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void SetCheckPointSystem(CheckPointSystem CheckPointSystem)
    {
        _checkPointSystem = CheckPointSystem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _checkPointSystem.NextCheckPoint();
            _boxCollider.enabled = false;
        }
    }
}
