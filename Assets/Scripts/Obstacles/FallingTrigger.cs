using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrigger : MonoBehaviour
{
    [SerializeField] private ObstacleManager _obstacleManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _obstacleManager.TriggerFall();
        Debug.Log("Starting TriggerFall coroutine!");
    }
}
