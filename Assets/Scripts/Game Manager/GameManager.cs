using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;


// Handles many things: game state, countdown, death , level completion, and boost UI
public class GameManager : MonoBehaviour
{

    //singleton Instance, only one gameManager at a time
    public static GameManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private Image _boostIcon;
    [SerializeField] private int _goTextSize = 50; // speical font text for the text "GO"

    [Header("Level Settings")]
    [SerializeField] private string _nextLevelName;
    [SerializeField] private string _mainMenuName = "MainMenu";

    [SerializeField] private CarController _carController;
    [SerializeField] private BoostSystem _boostSystem;

    // read only 
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

        // hide all panels at start
        _deathPanel.SetActive(false);
        _winPanel.SetActive(false);
        _boostIcon.gameObject.SetActive(false);

        // Kart can't move until countdown finishes
        _carController.enabled = false;
        StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if (!IsGameActive) return;
        UpdateBoostUI();
    }

    // counts down 3-2-1 GO and then kart can start moving
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

    //called when the boulder cathces the kart
    public void PlayerDied()
    {
        if (!IsGameActive) return;

        IsGameActive = false;
        _carController.enabled = false;
        _deathPanel.SetActive(true);
    }

    //calle when the kart reaches end of level/ enters win trigger at the end of the level 
    // Opens next level saves in playerprefs so player doesn't to redo everyting again
    public void LevelComplete()
    {
        if(!IsGameActive) return;

        IsGameActive = false;
        _carController.enabled = false;

        string currentScene = SceneManager.GetActiveScene().name;

        // checks what level is finishedand opens the next one
        if (currentScene == "Level 1")
            PlayerPrefs.SetInt("Level2Unlocked", 1);
        else if (currentScene == "Level 2")
            PlayerPrefs.SetInt("Level3Unlocked", 1);

        PlayerPrefs.Save(); 

        _winPanel.SetActive(true);
    }

    // shows or hides boost icon
    private void UpdateBoostUI()
    {
        if (_boostIcon == null) return;

        _boostIcon.gameObject.SetActive(_boostSystem._hasBoost);
    }


    // functions wired up in Unity in the inspector for scenes managment
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