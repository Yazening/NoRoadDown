using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputHandler : MonoBehaviour
{
    private NoRoadDownInputs _inputActions;
    public float TurnInput { get; private set; }
    public bool IsBoostPressed { get; private set; }
    private void Awake()
    {
        _inputActions = new NoRoadDownInputs(); 
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable(); 
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }
    private void Update()
    {
        TurnInput = _inputActions.Player.Steer.ReadValue<float>();
        IsBoostPressed = _inputActions.Player.Boost.IsPressed();

        Debug.Log($"Steer: {TurnInput} Boost: {IsBoostPressed}");
    }
}

