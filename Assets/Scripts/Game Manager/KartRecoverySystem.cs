using UnityEngine;

public class KartRecoverySystem : MonoBehaviour
    { 

[Header("Flip Detection")]
[SerializeField] private float _flipDotThreshold = 0.3f; // how much does the kart needs to rotate to be considred flipped 
[SerializeField] private float _flipTimeToRespawn = 2f; // time needed to be stuck to respawn
[SerializeField] private float _stuckSpeedThreshold = 0.5f; // below this speed, it will respawn as well

[Header("Boulder")]
[SerializeField] private BoulderHitKart _boulder;
[SerializeField] private float _resumeSpeedThreshold = 1f; // boulde start moving again when kart is faster than this variable

private Rigidbody _rb;
private CarController _carController;

    // last  checkpoint the kart passed
private Vector3 _lastCheckpointPos;
private Quaternion _lastCheckpointRot;
private int _lastCheckpointIndex = -1;

private float _flipTimer;
private bool _waitingForResume;

private void Awake()
{
    _rb = GetComponent<Rigidbody>();
    _carController = GetComponent<CarController>();

        // checks the last fall point whereever the kart fell
    _lastCheckpointPos = transform.position;
    _lastCheckpointRot = transform.rotation;
}

private void Update()
{

    CheckFlip();

        // when kart starts moving at certain speed, the boulder starts moving
    if (_waitingForResume && _rb.velocity.magnitude > _resumeSpeedThreshold)
    {
        _boulder.Resume();
        _waitingForResume = false;
    }
}

// check if kart is flipped
private void CheckFlip()
{
    float upAlignment = Vector3.Dot(transform.up, Vector3.up);
    bool isFlipped = upAlignment < _flipDotThreshold;
    bool isStuck = _rb.velocity.magnitude < _stuckSpeedThreshold;

    if (isFlipped && isStuck)
    {
        _flipTimer += Time.deltaTime;
        if (_flipTimer >= _flipTimeToRespawn)
            Respawn();
    }
    else
    {
        _flipTimer = 0f;
    }
}

    // called everytime the kart passes a new checkpoint
public void SetCheckpoint(Vector3 pos, Quaternion rot, int index)
{
    if (index <= _lastCheckpointIndex) return; // 
    _lastCheckpointPos = pos;
    _lastCheckpointRot = rot;
    _lastCheckpointIndex = index;
}

private void Respawn()
{
        // stops all movement
    _rb.velocity = Vector3.zero;
    _rb.angularVelocity = Vector3.zero;

        // goes bacck to last check point
        transform.SetPositionAndRotation(_lastCheckpointPos, _lastCheckpointRot);
    _flipTimer = 0f;

        // fixes kart rotation
    if (_carController != null)
        _carController.ResetTurnReference();

    // stops boulder
    if (_boulder != null)
    {
        _boulder.PauseAndClamp(_lastCheckpointPos);
        _waitingForResume = true;
    }
}
}