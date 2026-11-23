
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class ForestMusic : MonoBehaviour
{
    private static ForestMusic _instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        // Prevent duplicate music objects
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _audioSource = GetComponent<AudioSource>();

        _audioSource.loop = true;
        _audioSource.spatialBlend = 0f;

        // Keep music playing between scenes
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        _audioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Continue playing in forest scenes
        if (scene.name == "OutsideScene" ||
            scene.name == "Level1")
        {
            return;
        }

        // Stop forest music when leaving the forest
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance != this)
        {
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        _instance = null;
    }
}
