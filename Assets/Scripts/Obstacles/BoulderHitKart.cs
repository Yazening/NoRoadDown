using UnityEngine;

// This script will be attached to the boulder
// It chases the kart and tells GameManager when boulder is less than _catchDistance
public class BoulderHitKart : MonoBehaviour
{
    [SerializeField] private float _catchDistance = 2f; // Distance needed for boulder to smash player
    [SerializeField] private float _chaseForce = 40f;   // How fast the boulder to get towards the kart
    [SerializeField] private float _minDistanceBehindCheckpoint = 15f; // min distance boulder to stay bhind kart on recovery 

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

        // Get direction between boulder and kart straighten outs the Y so boulder doesn't fly of the course/track
        Vector3 directionToKart = (_kart.position - transform.position).normalized;
        directionToKart.y = 0f;
        _rb.AddForce(directionToKart * _chaseForce, ForceMode.Acceleration);
    }

    //checks if boulder reach kart to kill 
    private void Update()
    {
        if (_isPaused) return;
        if (_kart == null || _boulderCollider == null) return;

        //closest point measures from boulder
        Vector3 closestPoint = _boulderCollider.ClosestPoint(_kart.position);
        float distance = Vector3.Distance(closestPoint, _kart.position);

        if (distance <= _catchDistance)
        {
            GameManager.Instance.PlayerDied();
        }
    }

   // gives boulder a refernce to the kart so it knows what to follow
    public void SetKart(Transform kart)
    {
        _kart = kart;
    }

    //Pauses the boulder when recovery system is triggered 
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

    public void PauseBoulder()
    {
        _isPaused = true;
        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }
    }

    public void ResumeBoulder()
    {
        _isPaused = false;
        if (_rb != null)
            _rb.isKinematic = false;
    }


    //Debug functions
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