using UnityEngine.VFX;
using UnityEngine;

public class DriftSmokeController : MonoBehaviour
{
    [Header("VFX References")]
    [SerializeField] private VisualEffect _smokeLeft;
    [SerializeField] private VisualEffect _smokeRight;

    [Header("VFX Tuning")]
    [SerializeField] private float _steerThreshold = 0.5f;
    [SerializeField] private float _minSpeedForDrift = 3f;
    [SerializeField] private float _spawnRate = 100f;

    private PlayerInputHandler _input;
    private CarController _carController;

    private void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        _carController = GetComponent<CarController>();
    }

    private void Update()
    {
        HandleDriftSmoke();
    }
    private void HandleDriftSmoke()
    {
        bool isMovingFastEnough = _carController.CurrentSpeed > _minSpeedForDrift;
        bool isSteeringRight = _input.TurnInput > _steerThreshold;
        bool isSteeringLeft = _input.TurnInput < -_steerThreshold;

        if (isMovingFastEnough && isSteeringRight)
        {
            SetSmoke(_smokeLeft, true);
            SetSmoke(_smokeRight, false);
        }
        else if (isMovingFastEnough && isSteeringLeft)
        {
            SetSmoke(_smokeRight, true);
            SetSmoke(_smokeLeft, false);
        }
        else
        {
            SetSmoke(_smokeLeft, false);
            SetSmoke(_smokeRight, false);
        }
    }

    private void SetSmoke(VisualEffect smoke, bool active)
    {
        if (smoke == null) return;

        float rate = active ? _spawnRate : 0f;
        smoke.SetFloat("SpawnRate", rate);
    }
}
