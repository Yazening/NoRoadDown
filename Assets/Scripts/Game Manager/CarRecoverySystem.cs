using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRecoverySystem : MonoBehaviour
{ 
[Header("Fall Detection")]
    [SerializeField] private float _fallYThreshold = -10f;

[Header("Flip Detection")]
[SerializeField] private float _flipDotThreshold = 0.3f; 
[SerializeField] private float _flipTimeToRespawn = 2f; 
[SerializeField] private float _stuckSpeedThreshold = 0.5f;

[Header("Boulder")]
[SerializeField] private BoulderHitKart _boulder;
[SerializeField] private float _resumeSpeedThreshold = 1f;

private Rigidbody _rb;
private CarController _carController;

private Vector3 _lastCheckpointPos;
private Quaternion _lastCheckpointRot;
private int _lastCheckpointIndex = -1;

private float _flipTimer;
private bool _waitingForResume;

private void Awake()
{
    _rb = GetComponent<Rigidbody>();
    _carController = GetComponent<CarController>();
    // Fallback checkpoint = spawn position, in case player falls before hitting one
    _lastCheckpointPos = transform.position;
    _lastCheckpointRot = transform.rotation;
}

private void Update()
{
    if (transform.position.y < _fallYThreshold)
    {
        Respawn();
        return;
    }

    CheckFlip();

    if (_waitingForResume && _rb.velocity.magnitude > _resumeSpeedThreshold)
    {
        _boulder.Resume();
        _waitingForResume = false;
    }
}

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

public void SetCheckpoint(Vector3 pos, Quaternion rot, int index)
{
    if (index <= _lastCheckpointIndex) return; // 
    _lastCheckpointPos = pos;
    _lastCheckpointRot = rot;
    _lastCheckpointIndex = index;
}

private void Respawn()
{
    _rb.velocity = Vector3.zero;
    _rb.angularVelocity = Vector3.zero;
    transform.SetPositionAndRotation(_lastCheckpointPos, _lastCheckpointRot);
    _flipTimer = 0f;

    if (_carController != null)
        _carController.ResetTurnReference();

    if (_boulder != null)
    {
        _boulder.PauseAndClamp(_lastCheckpointPos);
        _waitingForResume = true;
    }
}
}