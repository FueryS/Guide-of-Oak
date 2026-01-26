using System;
using UnityEngine;

public class BarrierFollow : MonoBehaviour
{
    Transform _target;

    float _xPos;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"Block at {name} couldn't find an object with the 'Player' tag!");
        }

        _xPos = transform.position.x;
    }

    private void Update()
    {
        transform.position = new(_xPos,_target.position.y,_target.position.z);
    }

    #region collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("COllided");
        }
    }

    #endregion


}
