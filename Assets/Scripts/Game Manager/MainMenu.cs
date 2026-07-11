using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Handles panel navigation, level locking, settins and scenes loadings
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
    [SerializeField] private Image _brightnessOverlay;

    private void Start()
    {
        ShowMainMenu();
        LoadSettings();
        UpdateLevelButtons();
    }

    // reads playerprefs to check which level has the player unlockded
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

    // manages Main menu settings and levels panels.
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

    // unlocks levels if player completed them and loads them as well
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
        if (_brightnessOverlay != null)
    {
            Color c = _brightnessOverlay.color;
            c.a = 1f - value; 
            _brightnessOverlay.color = c;
        }
        PlayerPrefs.SetFloat("Brightness", value);
    }

    public void OnFullscreenToggled(bool value)
    {
        if (value)
        {
            Resolution nativeRes = Screen.currentResolution;
            Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    // Loads the saved settings and applies them when game opens
    private void LoadSettings()
    {
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);
        if (_brightnessOverlay != null)
        {
            Color c = _brightnessOverlay.color;
            c.a = 1f - savedBrightness;
            _brightnessOverlay.color = c;
        }
        if (_brightnessSlider != null)
            _brightnessSlider.SetValueWithoutNotify(savedBrightness);

        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        if (savedFullscreen)
        {
            Resolution nativeRes = Screen.currentResolution;
            Screen.SetResolution(nativeRes.width, nativeRes.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        if (_fullscreenToggle != null)
            _fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
    }
}
