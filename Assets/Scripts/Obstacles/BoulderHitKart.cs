using UnityEngine;

public class BoulderHitKart : MonoBehaviour
{
    [SerializeField] private float _catchDistance = 2f;

    private Transform _kart;

    private void Update()
    {
        if (_kart == null) return;

        float distance = Vector3.Distance(transform.position, _kart.position);

        if (distance <= _catchDistance)
        {
            GameManager.Instance.PlayerDied();
        }
    }
    public void SetKart(Transform kart)
    {
        _kart = kart;
    }
}
