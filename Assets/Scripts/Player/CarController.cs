using UnityEngine;

// states the state of the kart 
// Mainly for other scripts like BoostSystem, VFX, read it to act according to the state of the kart
public enum KartState
{
    Moving,
    Boosting
}

// controls kart movement
// reads input from playerinputhandler
// kart always moves forward, player can only steer right and left and boost
public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float _carSpeed = 2000f;
    [SerializeField] private float _turnStrength = 100f;
    [SerializeField] private float _maxSpeed = 15f;
    [SerializeField] private float _maxTurnAngle = 70f; // Maximum degrees the kart can turn/steer

    [Header("Boost Settings")]
    [SerializeField] private float _boostMultiplier = 2f; // multiplies by _carspeed when boosted
    [SerializeField] private float _boostDuration = 1.5f; // duration of boost

    [Header("Ground Settings")]
    [SerializeField] private float _groundRayDistance = 0.6f;
    [SerializeField] private float _gravityForce = 20f;
    [SerializeField] private LayerMask _groundLayer;

    // public properties, other scirpts can read them but can't change them
    public KartState CurrentState { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsGrounded { get; private set; }

    // private references, in awake so we don't have to call them every frame lose preformance
    private Rigidbody _rb;
    private PlayerInputHandler _input;
    private BoostSystem _boostSystem;

    // tracks boost
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

    public void ResetTurnReference()
    {
        _initialYAngle = transform.eulerAngles.y;
    }

    // counts down the boost variable, and then returns to Moving state when it finsihes 
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

    // checks input and if player has boost, which state the state of the kart
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
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, clampedAngle, transform.eulerAngles.z);
        }
    }
    private void MoveForward()
    {
        float force = CurrentState == KartState.Boosting ? _carSpeed * _boostMultiplier : _carSpeed;
        _rb.AddForce(transform.forward * force * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    // uses raycast to check if kart is on slope and if not applies move force so kart is attached to slope
    private void ApplyGravity()
    {
        RaycastHit hit;
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, _groundRayDistance, _groundLayer);

        if (!IsGrounded)
        {
            _rb.AddForce(Vector3.down * _gravityForce, ForceMode.Acceleration);
        }
    }


    // maximizes the kart speed
    // it clamps horizontal velocity so the slope's downward pull still work properly
    private void ClampSpeed()
    {
        Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        float maxSpeed = CurrentState == KartState.Boosting ? _maxSpeed * _boostMultiplier : _maxSpeed;

        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedFlat = flatVelocity.normalized * maxSpeed; _rb.velocity = new Vector3(clampedFlat.x, _rb.velocity.y, clampedFlat.z);
        }
    }

    // Debug Tools
    public float _debugCarSpeed
    {
        get { return _carSpeed; }
        set { _carSpeed = value; }
    }
}