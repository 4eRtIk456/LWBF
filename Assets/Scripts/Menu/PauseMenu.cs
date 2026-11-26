using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _pauseRoot;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _optionsPanel;

    private bool _isPaused;
    private float _previousTimeScale = 1f;

    private void Awake()
    {
        _pauseRoot.SetActive(false);
        _pausePanel.SetActive(true);
        _optionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (!_isPaused)
        {
            PauseGame();
        }
        else if (_optionsPanel.activeSelf)
        {
            BackToPause();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (_isPaused)
        {
            return;
        }

        _isPaused = true;

        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        _pauseRoot.SetActive(true);
        _pausePanel.SetActive(true);
        _optionsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        if (!_isPaused)
        {
            return;
        }

        _pauseRoot.SetActive(false);
        _optionsPanel.SetActive(false);

        Time.timeScale = _previousTimeScale;
        _isPaused = false;
    }

    public void OpenOptions()
    {
        _pausePanel.SetActive(false);
        _optionsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        _optionsPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        _isPaused = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        if (_isPaused)
        {
            Time.timeScale = _previousTimeScale;
            _isPaused = false;
        }
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }

}
