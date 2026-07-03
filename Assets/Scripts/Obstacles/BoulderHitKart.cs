using UnityEngine;

public class BoulderHitKart : MonoBehaviour
{
    [SerializeField] private float _catchDistance = 25f;
    [SerializeField] private float _chaseForce = 40f;

    private Transform _kart;
    private Rigidbody _rb;
    private Collider _boulderCollider; 

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _boulderCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (_kart == null || _rb == null) return;

        Vector3 directionToKart = (_kart.position - transform.position).normalized;
        directionToKart.y = 0f;
        _rb.AddForce(directionToKart * _chaseForce, ForceMode.Acceleration);
    }

    private void Update()
    {
        if (_kart == null) return;

        Collider boulderCollider = GetComponent<Collider>();
        if (boulderCollider == null) return;

        Vector3 closestPoint = boulderCollider.ClosestPoint(_kart.position);
        float distanceKartToBoulder = Vector3.Distance(closestPoint, _kart.position);
        if (distanceKartToBoulder <= _catchDistance)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    public void SetKart(Transform kart)
    {
        _kart = kart;
    }
    public void DebugIncreaseSpeed(float amount)
    {
        _chaseForce += amount;
    }

    public void DebugDecreaseSpeed(float amount)
    {
        _chaseForce = Mathf.Max(0f, _chaseForce - amount);
    }

    public float DebugGetSpeed()
    {
        return _chaseForce;
    }
}
