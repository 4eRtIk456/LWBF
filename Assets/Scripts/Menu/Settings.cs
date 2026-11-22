
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Settings : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Display")]
    [SerializeField] private Button fullscreenOnButton;
    [SerializeField] private Button fullscreenOffButton;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private List<Vector2Int> resolutions = new List<Vector2Int>();
    private bool isFullscreen;

    private Color selectedColor = new Color(0.965f, 0.780f, 0.541f);
    private Color normalColor = new Color(0.184f, 0.078f, 0.078f, 1f);
    private Color selectedTextColor = new Color32(50, 23, 22, 255); 
    private Color normalTextColor = new Color32(246, 199, 138, 255); 

    private void Start()
    {
        // Load saved audio settings
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.7f);

        _musicSlider.SetValueWithoutNotify(music);
        _sfxSlider.SetValueWithoutNotify(sfx);

        ApplyVolume("MusicVolume", music);
        ApplyVolume("SFXVolume", sfx);

        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Load display settings
        isFullscreen = PlayerPrefs.GetInt(
            "Fullscreen", Screen.fullScreen ? 1 : 0
        ) == 1;

        SetupResolutions();

        fullscreenOnButton.onClick.AddListener(() => SetFullscreen(true));
        fullscreenOffButton.onClick.AddListener(() => SetFullscreen(false));

        UpdateFullscreenButtons();

        // Restore saved display settings in the built game
#if !UNITY_EDITOR
        if (PlayerPrefs.HasKey("ResolutionWidth"))
            ApplyResolution();
#endif
    }

    private void ApplyVolume(string parameter, float value)
    {
        if (_audioMixer == null)
            return;

        // Convert slider value (0-1) to decibels
        float dB = 20f * Mathf.Log10(Mathf.Max(value, 0.0001f));
        _audioMixer.SetFloat(parameter, dB);
    }

    public void SetMusicVolume(float value)
    {
        ApplyVolume("MusicVolume", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        ApplyVolume("SFXVolume", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void SetupResolutions()
    {
        resolutions.Clear();
        List<string> options = new List<string>();

        foreach (Resolution resolution in Screen.resolutions)
        {
            Vector2Int size = new Vector2Int(
                resolution.width, resolution.height
            );

            // Avoid duplicate resolutions with different refresh rates
            if (!resolutions.Contains(size))
            {
                resolutions.Add(size);
                options.Add(size.x + " x " + size.y);
            }
        }

        if (resolutions.Count == 0)
        {
            Vector2Int current = new Vector2Int(
                Screen.width, Screen.height
            );

            resolutions.Add(current);
            options.Add(current.x + " x " + current.y);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        int savedWidth = PlayerPrefs.GetInt(
            "ResolutionWidth", Screen.width
        );
        int savedHeight = PlayerPrefs.GetInt(
            "ResolutionHeight", Screen.height
        );

        int index = resolutions.FindIndex(r =>
            r.x == savedWidth && r.y == savedHeight
        );

        if (index < 0)
            index = resolutions.Count - 1;

        resolutionDropdown.SetValueWithoutNotify(index);
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Count)
            return;

        Vector2Int selected = resolutions[index];

        PlayerPrefs.SetInt("ResolutionWidth", selected.x);
        PlayerPrefs.SetInt("ResolutionHeight", selected.y);

        ApplyResolution();
    }

    public void SetFullscreen(bool value)
    {
        isFullscreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);

        UpdateFullscreenButtons();
        ApplyResolution();
    }

    private void ApplyResolution()
    {
        int index = resolutionDropdown.value;

        if (index < 0 || index >= resolutions.Count)
            return;

        Vector2Int selected = resolutions[index];

        FullScreenMode mode = isFullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        Screen.SetResolution(selected.x, selected.y, mode);
    }

    
    private void UpdateFullscreenButtons()
    {
        // Change button backgrounds
        fullscreenOnButton.image.color =
            isFullscreen ? selectedColor : normalColor;

        fullscreenOffButton.image.color =
            isFullscreen ? normalColor : selectedColor;

        // Get TextMeshPro components
        TMP_Text onText = fullscreenOnButton.GetComponentInChildren<TMP_Text>();
        TMP_Text offText = fullscreenOffButton.GetComponentInChildren<TMP_Text>();

        // Change text colors
        onText.color = isFullscreen ? selectedTextColor : normalTextColor;
        offText.color = isFullscreen ? normalTextColor : selectedTextColor;
    }


    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
