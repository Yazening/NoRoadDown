using UnityEngine;

public class FallingTrigger : MonoBehaviour
{
    [SerializeField] private ObstacleManager _obstacleManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(_obstacleManager.TriggerFall());
    }
}
