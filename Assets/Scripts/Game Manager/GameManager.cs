using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private Image _boostIcon;
    [SerializeField] private int _goTextSize = 50;

    [Header("Level Settings")]
    [SerializeField] private string _nextLevelName;
    [SerializeField] private string _mainMenuName = "MainMenu";

    [SerializeField] private CarController _carController;
    [SerializeField] private BoostSystem _boostSystem;
    public bool IsGameActive { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _deathPanel.SetActive(false);
        _winPanel.SetActive(false);
        _boostIcon.gameObject.SetActive(false);
        _carController.enabled = false;

        StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if (!IsGameActive) return;
        UpdateBoostUI();
    }

    private IEnumerator StartCountdown()
    {
        string[] steps = { "3", "2", "1", $"<size={_goTextSize}%>GO!</size>"};

        foreach (string step in steps)
        {
            _countdownText.text = step;
            yield return new WaitForSeconds(1f);
        }

        _countdownText.gameObject.SetActive(false);
        _carController.enabled = true;
        IsGameActive = true;
    }

    public void PlayerDied()
    {
        if (!IsGameActive) return;

        IsGameActive = false;
        _carController.enabled = false;
        _deathPanel.SetActive(true);
    }
    public void LevelComplete()
    {
        if (!IsGameActive) return;

        IsGameActive = false;
        _carController.enabled = false;
        _winPanel.SetActive(true);
    }

    private void UpdateBoostUI()
    {
        if (_boostIcon == null) return;

        _boostIcon.gameObject.SetActive(_boostSystem._hasBoost);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(_nextLevelName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(_mainMenuName);
    }
}