using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class DebugTools : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private CarController _carController;
    [SerializeField] private BoostSystem _boostSystem;

    [Header("Level Names")]
    [SerializeField] private string _level1Name;
    [SerializeField] private string _level2Name;
    [SerializeField] private string _level3Name;

    private bool _debugEnabled = false;
    private BoulderHitKart _boulderHitKart;
    private NoRoadDownInputs _inputActions;

    private void Awake()
    {
        _debugText.gameObject.SetActive(false);
        _inputActions = new NoRoadDownInputs();
    }

    private void OnEnable()
    {
        _inputActions.DebugTools.Enable();
        _inputActions.DebugTools.DebugOpener.performed += OnToggleDebug;
        _inputActions.DebugTools.IncreaseCarSpeed.performed += OnCarSpeedUp;
        _inputActions.DebugTools.DecreaseCarSpeed.performed += OnCarSpeedDown;
        _inputActions.DebugTools.IncreaseBoulderSpeed.performed += OnBoulderSpeedUp;
        _inputActions.DebugTools.DecreaseBoulderSpeed.performed += OnBoulderSpeedDown;
        _inputActions.DebugTools.HaveBoost.performed += OnGiveBoost;
        _inputActions.DebugTools.Level1.performed += OnLoadLevel1;
        _inputActions.DebugTools.Level2.performed += OnLoadLevel2;
        _inputActions.DebugTools.Level3.performed += OnLoadLevel3;
        _inputActions.DebugTools.ClearPlayerPrefs.performed += OnClearPlayerPrefs;
    }

    private void OnDisable()
    {
        _inputActions.DebugTools.DebugOpener.performed -= OnToggleDebug;
        _inputActions.DebugTools.IncreaseCarSpeed.performed -= OnCarSpeedUp;
        _inputActions.DebugTools.DecreaseCarSpeed.performed -= OnCarSpeedDown;
        _inputActions.DebugTools.IncreaseBoulderSpeed.performed -= OnBoulderSpeedUp;
        _inputActions.DebugTools.DecreaseBoulderSpeed.performed -= OnBoulderSpeedDown;
        _inputActions.DebugTools.HaveBoost.performed -= OnGiveBoost;
        _inputActions.DebugTools.Level1.performed -= OnLoadLevel1;
        _inputActions.DebugTools.Level2.performed -= OnLoadLevel2;
        _inputActions.DebugTools.Level3.performed -= OnLoadLevel3;
        _inputActions.DebugTools.ClearPlayerPrefs.performed -= OnClearPlayerPrefs;
        _inputActions.DebugTools.Disable();
        
    }

    private void Update()
    {
        if (!_debugEnabled) return;

        if (_boulderHitKart == null)
        {
            try
            {
                GameObject boulderObj = GameObject.FindWithTag("Boulder");
                if (boulderObj != null)
                    _boulderHitKart = boulderObj.GetComponent<BoulderHitKart>();
            }
            catch { }
        }

        UpdateDebugText();
    }

    private void OnToggleDebug(InputAction.CallbackContext ctx)
    {
        _debugEnabled = !_debugEnabled;
        _debugText.gameObject.SetActive(_debugEnabled);
    }

    private void OnCarSpeedUp(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled) return;
        _carController._debugCarSpeed += 1000f;
    }

    private void OnCarSpeedDown(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled) return;
        _carController._debugCarSpeed -= 1000f;
    }

    private void OnBoulderSpeedUp(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled) return;
        _boulderHitKart.DebugIncreaseSpeed(1f);
    }

    private void OnBoulderSpeedDown(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled || _boulderHitKart == null) return;
        _boulderHitKart.DebugDecreaseSpeed(1f);
    }

    private void OnGiveBoost(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled) return;
        _boostSystem.DebugGiveBoost();
    }

    private void OnLoadLevel1(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled || string.IsNullOrEmpty(_level1Name)) return;
        SceneManager.LoadScene(_level1Name);
    }

    private void OnLoadLevel2(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled || string.IsNullOrEmpty(_level2Name)) return;
        SceneManager.LoadScene(_level2Name);
    }

    private void OnLoadLevel3(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled || string.IsNullOrEmpty(_level3Name)) return;
        SceneManager.LoadScene(_level3Name);
    }

    private void OnClearPlayerPrefs(InputAction.CallbackContext ctx)
    {
        if (!_debugEnabled) return;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    private void UpdateDebugText()
    {
        float boulderSpeed = _boulderHitKart != null ? _boulderHitKart.DebugGetSpeed() : 0f;

        _debugText.text =
            $"--- DEBUG MODE ---\n" +
            $"Car Speed: {_carController.CurrentSpeed:F1} (I/O)\n" +
            $"Boost: {(_boostSystem._hasBoost ? "FULL" : "EMPTY")} (B)\n" +
            $"Boulder Speed: {boulderSpeed:F1} (N/M)\n" +
            $"1 = {_level1Name} | 2 = {_level2Name} | 3 = {_level3Name}\n" +
            $"Clear Player Prefs = - (Subtract)";
    }
}