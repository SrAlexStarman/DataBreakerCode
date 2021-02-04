using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSystem : MonoBehaviour
{
    [SerializeField] private CheckPoint[] _checkPoints;
    [SerializeField] private DataContainer_Player _playerData;

    [SerializeField] private Action_Dash _dashAction;

    private int _checkPointIndex = 0;

    private void Awake()
    {
        foreach(var item in _checkPoints)
        {
            item.SetCheckPointSystem(this);
        }
    }

    private void Start()
    {
        Respawn();
    }

    public void NextCheckPoint()
    {
        _checkPointIndex++;
    }

    public void Respawn()
    {
        _playerData.ActionController.Transf.position = _checkPoints[_checkPointIndex].transform.position;

        if(_playerData.IsInvinsible)
        {
            _dashAction.EndDash();
        }
    }
}
