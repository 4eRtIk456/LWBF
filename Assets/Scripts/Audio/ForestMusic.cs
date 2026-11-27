
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class ForestMusic : MonoBehaviour
{
    public static ForestMusic Instance { get; private set; }

    private AudioSource _audioSource;
    private bool _isPausedForGhost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.spatialBlend = 0f;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        _audioSource.Play();
    }

    public void PauseForGhost()
    {
        if (_isPausedForGhost || _audioSource == null)
        {
            return;
        }

        if (!_audioSource.isPlaying)
        {
            return;
        }

        _audioSource.Pause();
        _isPausedForGhost = true;
    }

    public void ResumeAfterGhost()
    {
        if (!_isPausedForGhost || _audioSource == null)
        {
            return;
        }

        _audioSource.UnPause();
        _isPausedForGhost = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "OutsideScene" ||
            scene.name == "Level1")
        {
            return;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }
}
