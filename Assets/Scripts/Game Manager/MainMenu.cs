using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _levelSelectPanel;
    [SerializeField] private GameObject _settingsPanel;

    [Header("Level Buttons")]
    [SerializeField] private Button _level1Button;
    [SerializeField] private Button _level2Button;
    [SerializeField] private Button _level3Button;

    [Header("Lock Icons")]
    [SerializeField] private GameObject _level2LockIcon;
    [SerializeField] private GameObject _level3LockIcon;

    [Header("Scene Names")]
    [SerializeField] private string _level1Name;
    [SerializeField] private string _level2Name;
    [SerializeField] private string _level3Name;

    [Header("Settings")]
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private Toggle _fullscreenToggle;


    private void Start()
    {
        ShowMainMenu();
        LoadSettings();
        UpdateLevelButtons();
    }

    private void UpdateLevelButtons()
    {
        bool level2Unlocked = PlayerPrefs.GetInt("Level2Unlocked", 0) == 1;
        bool level3Unlocked = PlayerPrefs.GetInt("Level3Unlocked", 0) == 1;

        _level1Button.interactable = true;

        _level2Button.interactable = level2Unlocked;
        if (_level2LockIcon != null)
            _level2LockIcon.SetActive(!level2Unlocked);

        _level3Button.interactable = level3Unlocked;
        if (_level3LockIcon != null)
            _level3LockIcon.SetActive(!level3Unlocked);
    }

    public void ShowMainMenu()
    {
        _mainMenuPanel.SetActive(true);
        _levelSelectPanel.SetActive(false);
        _settingsPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        _mainMenuPanel.SetActive(false);
        _levelSelectPanel.SetActive(true);
        _settingsPanel.SetActive(false);
        UpdateLevelButtons();
    }

    public void ShowSettings()
    {
        _mainMenuPanel.SetActive(false);
        _levelSelectPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(_level1Name);
    }

    public void LoadLevel2()
    {
        if (PlayerPrefs.GetInt("Level2Unlocked", 0) == 1)
            SceneManager.LoadScene(_level2Name);
    }

    public void LoadLevel3()
    {
        if (PlayerPrefs.GetInt("Level3Unlocked", 0) == 1)
            SceneManager.LoadScene(_level3Name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnBrightnessChanged(float value)
    {
        RenderSettings.ambientIntensity = value;
        PlayerPrefs.SetFloat("Brightness", value);
    }

    public void OnFullscreenToggled(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    private void LoadSettings()
    { 
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);
        RenderSettings.ambientIntensity = savedBrightness;
        if (_brightnessSlider != null)
            _brightnessSlider.value = savedBrightness;

        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = savedFullscreen;
        if (_fullscreenToggle != null)
            _fullscreenToggle.isOn = savedFullscreen;
    }
}
