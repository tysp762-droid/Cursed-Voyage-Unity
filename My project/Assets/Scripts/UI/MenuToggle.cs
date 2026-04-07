using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MenuToggle : MonoBehaviour
{
    private const string VolumeKey = "GameVolume";
    private const string SensitivityKey = "GameSensitivity";
    private const string DifficultyKey = "GameDifficulty";

    [Header("Menu")]
    public GameObject menuPanel;

    [Header("UI Sliders")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Difficulty Buttons")]
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;

    [Header("Difficulty Display")]
    public TMP_Text difficultyText; // TextMeshPro tekstcomponent voor moeilijkheid

    [Header("References")]
    public AudioMixer audioMixer;
    public string exposedVolumeParam = "MyExposedParam"; // Vul hier je geexposeerde volume parameter naam in
    public PlayerCam playerCam;
    public WaveController waveController;

    private CanvasGroup menuCanvasGroup;
    private bool useCanvasGroupToggle;

    private float sensitivity = 1f;

    private DifficultyLevel currentDifficulty;

    void Start()
    {
        if (menuPanel == null)
        {
            Debug.LogError("MenuToggle: menuPanel is not assigned!");
            enabled = false;
            return;
        }

        if (menuPanel == gameObject)
        {
            useCanvasGroupToggle = true;
            menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
            if (menuCanvasGroup == null)
            {
                menuCanvasGroup = menuPanel.AddComponent<CanvasGroup>();
            }
            SetMenuState(false);
        }
        else
        {
            menuPanel.SetActive(false);
        }

        // Laad opgeslagen volume en zet slider
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        SetVolume(savedVolume);

        // Laad opgeslagen sensitivity en zet slider
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1f);
        sensitivity = savedSensitivity;
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
        SetSensitivity(savedSensitivity);

        // Buttons koppelen
        if (easyButton != null)
            easyButton.onClick.AddListener(() => SetDifficulty(DifficultyLevel.Easy));
        if (normalButton != null)
            normalButton.onClick.AddListener(() => SetDifficulty(DifficultyLevel.Normal));
        if (hardButton != null)
            hardButton.onClick.AddListener(() => SetDifficulty(DifficultyLevel.Hard));

        // Laad opgeslagen moeilijkheid en stel in
        int savedDifficulty = PlayerPrefs.GetInt(DifficultyKey, (int)DifficultyLevel.Easy);
        SetDifficulty((DifficultyLevel)savedDifficulty);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        bool isOpen;

        if (useCanvasGroupToggle)
        {
            isOpen = menuCanvasGroup.alpha > 0f && menuCanvasGroup.interactable;
            SetMenuState(!isOpen);
        }
        else
        {
            isOpen = menuPanel.activeSelf;
            menuPanel.SetActive(!isOpen);
        }

        if (!isOpen)
        {
            // Menu openen
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;

            if (playerCam != null)
                playerCam.updatingRotation = false;
        }
        else
        {
            // Menu sluiten
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;

            if (playerCam != null)
                playerCam.updatingRotation = true;
        }
    }

    private void SetMenuState(bool open)
    {
        menuPanel.SetActive(true);
        menuCanvasGroup.alpha = open ? 1f : 0f;
        menuCanvasGroup.interactable = open;
        menuCanvasGroup.blocksRaycasts = open;
    }

    private void OnVolumeChanged(float value)
    {
        SetVolume(value);
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    private void SetVolume(float value)
    {
        if (audioMixer == null) return;

        float sliderValue = Mathf.Clamp(value, 0.0001f, 1f);
        sliderValue = sliderValue * sliderValue; // curve voor beter volumegevoel
        float dB = Mathf.Lerp(-80f, 0f, sliderValue);

        audioMixer.SetFloat(exposedVolumeParam, dB);
    }

    private void OnSensitivityChanged(float value)
    {
        SetSensitivity(value);
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();
    }

    private void SetSensitivity(float value)
    {
        sensitivity = value;
        if (playerCam != null)
            playerCam.SetSensitivity(sensitivity);
    }

    private void SetDifficulty(DifficultyLevel difficulty)
    {
        if (waveController != null)
        {
            waveController.SetDifficulty(difficulty);
        }

        currentDifficulty = difficulty;

        if (difficultyText != null)
        {
            string displayText = difficulty switch
            {
                DifficultyLevel.Easy => "Soft",
                DifficultyLevel.Normal => "Hard",
                DifficultyLevel.Hard => "Holy Shit",
                _ => difficulty.ToString()
            };

            difficultyText.text = $"Difficulty: {displayText}";
        }

        PlayerPrefs.SetInt(DifficultyKey, (int)difficulty);
        PlayerPrefs.Save();

        Debug.Log($"[MenuToggle] Difficulty set to {difficulty}");
    }
}
