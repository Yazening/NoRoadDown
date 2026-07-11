using UnityEngine.VFX;
using UnityEngine;

public class DriftSmokeController : MonoBehaviour
{
    [Header("VFX References")]
    [SerializeField] private VisualEffect _smokeLeft;
    [SerializeField] private VisualEffect _smokeRight;

    [Header("VFX Tuning")]
    [SerializeField] private float _steerThreshold = 0.5f; // angle needed for VFX to start appearing
    [SerializeField] private float _minSpeedForDrift = 3f; // There won't be smoke if the kart is stationary
    [SerializeField] private float _spawnRate = 100f; // particle spawn rate when smoke appears

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

        // turning right, makes the vfx appears
        if (isMovingFastEnough && isSteeringRight)
        {
            SetSmoke(_smokeLeft, true);
            SetSmoke(_smokeRight, false);
        }
        // turning left, makes the vfx appears
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
