
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance { get; private set; }

    [Header("Mailbox")]
    [SerializeField] private AudioClip _mailboxOpen;
    [SerializeField] private AudioClip _mailboxClose;

    [Header("Door")]
    [SerializeField] private AudioClip _doorEnter;
    [SerializeField] private AudioClip _doorExit;

    [Header("Ghost")]
    [SerializeField] private AudioClip _ivyGhost;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 0f;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayMailboxOpen()
    {
        Play(_mailboxOpen);
    }

    public void PlayMailboxClose()
    {
        Play(_mailboxClose);
    }

    public void PlayDoorEnter()
    {
        Play(_doorEnter);
    }

    public void PlayDoorExit()
    {
        Play(_doorExit);
    }

    public void PlayIvyGhost()
    {
        Play(_ivyGhost);
    }

    private void Play(AudioClip clip)
    {
        if (clip == null || _audioSource == null)
        {
            return;
        }

        _audioSource.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
