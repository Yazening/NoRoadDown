using UnityEngine;

public class BoulderHitKart : MonoBehaviour
{
    [SerializeField] private float _catchDistance = 25f;
    [SerializeField] private float _chaseForce = 40f;
    [SerializeField] private float _minDistanceBehindCheckpoint = 15f;

    private Transform _kart;
    private Rigidbody _rb;
    private Collider _boulderCollider;
    private bool _isPaused;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _boulderCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (_isPaused) return;
        if (_kart == null || _rb == null) return;
        Vector3 directionToKart = (_kart.position - transform.position).normalized;
        directionToKart.y = 0f;
        _rb.AddForce(directionToKart * _chaseForce, ForceMode.Acceleration); ;
    }

    public void SetKart(Transform kart)
    {
        _kart = kart;
    }

    public void PauseAndClamp(Vector3 checkpointPos)
    {
        _isPaused = true;

        float distanceAhead = Vector3.Distance(transform.position, checkpointPos);
        if (distanceAhead < _minDistanceBehindCheckpoint)
        {
            Vector3 dirFromCheckpointToBoulder = (transform.position - checkpointPos).normalized;
            if (dirFromCheckpointToBoulder == Vector3.zero)
                dirFromCheckpointToBoulder = -transform.forward;

            transform.position = checkpointPos - dirFromCheckpointToBoulder * _minDistanceBehindCheckpoint;
        }

        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
    public void Resume()
    {
        _isPaused = false;
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
