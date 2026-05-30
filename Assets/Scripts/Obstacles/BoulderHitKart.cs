using UnityEngine;

public class BoulderHitKart : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        GameManager.Instance.PlayerDied();
    }
}
