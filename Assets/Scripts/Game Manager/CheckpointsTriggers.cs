using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private int _checkpointIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<CarRecoverySystem>(out var respawn)) return;
        respawn.SetCheckpoint(transform.position, transform.rotation, _checkpointIndex);
    }
}
