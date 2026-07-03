using UnityEngine;
public enum KartState
{
    Moving,
    Boosting
}
public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float _carSpeed = 2000f;
    [SerializeField] private float _turnStrength = 100f;
    [SerializeField] private float _maxSpeed = 15f;
    [SerializeField] private float _maxTurnAngle = 70f;

    [Header("Boost Settings")]
    [SerializeField] private float _boostMultiplier = 2f;
    [SerializeField] private float _boostDuration = 1.5f;

    [Header("Ground Settings")]
    [SerializeField] private float _groundRayDistance = 0.6f;
    [SerializeField] private float _gravityForce = 20f;
    [SerializeField] private LayerMask _groundLayer;

    public KartState CurrentState { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsGrounded { get; private set; }

    private Rigidbody _rb;
    private PlayerInputHandler _input;
    private BoostSystem _boostSystem;

    private bool _hasBoost = false;
    private float _boostTimer = 0f; 
    private float _initialYAngle;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInputHandler>();
        _boostSystem = GetComponent<BoostSystem>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationY;
        _initialYAngle = transform.eulerAngles.y;
    }

    private void Update()
    {
        UpdateBoostTimer();
        UpdateState();
    }

    private void FixedUpdate()
    {
        CurrentSpeed = _rb.velocity.magnitude;
        Turn();
        MoveForward();
        ApplyGravity();
        ClampSpeed();
    }
    public void GiveBoost()
    {
        _hasBoost = true;
    }

    private void UpdateBoostTimer()
    {
        if (CurrentState == KartState.Boosting)
        {
            _boostTimer -= Time.deltaTime;

            if (_boostTimer <= 0f)
            {
                _boostTimer = 0f;
                CurrentState = KartState.Moving;
            }
        }
    }

    private void UpdateState()
    {
        if (_input.IsBoostPressed && _hasBoost && CurrentState != KartState.Boosting)
        {
            CurrentState = KartState.Boosting;
            _boostTimer = _boostDuration;
            _boostSystem.UseBoost();
            _hasBoost = false;
        }
        else if (CurrentState != KartState.Boosting)
        {
            CurrentState = KartState.Moving;
        }
    }

    private void Turn()
    {
        if (!IsGrounded) return;

        float steer = _input.TurnInput * _turnStrength * Time.fixedDeltaTime;
        transform.Rotate(0f, steer, 0f, Space.World);

        float currentY = transform.eulerAngles.y;
        float delta = Mathf.DeltaAngle(_initialYAngle, currentY);

        if (Mathf.Abs(delta) > _maxTurnAngle)
        {
            float clampedAngle = _initialYAngle + Mathf.Clamp(delta, -_maxTurnAngle, _maxTurnAngle);
            transform.eulerAngles = new Vector3( transform.eulerAngles.x, clampedAngle, transform.eulerAngles.z);
        }
    }
    private void MoveForward()
    {
        float force = CurrentState == KartState.Boosting ? _carSpeed * _boostMultiplier : _carSpeed;
        _rb.AddForce(transform.forward * force * Time.fixedDeltaTime,  ForceMode.Acceleration);
    }

    private void ApplyGravity()
    {
        RaycastHit hit;
        IsGrounded = Physics.Raycast( transform.position, Vector3.down, out hit, _groundRayDistance,_groundLayer);

        if (!IsGrounded)
        {
            _rb.AddForce(Vector3.down * _gravityForce, ForceMode.Acceleration);
        }
    }

    private void ClampSpeed()
    {
        Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        float maxSpeed = CurrentState == KartState.Boosting ? _maxSpeed * _boostMultiplier : _maxSpeed;

        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedFlat = flatVelocity.normalized * maxSpeed; _rb.velocity = new Vector3( clampedFlat.x, _rb.velocity.y,  clampedFlat.z);
        }
    }
    public float _debugCarSpeed
    {
        get { return _carSpeed; }
        set { _carSpeed = value; }
    }
}