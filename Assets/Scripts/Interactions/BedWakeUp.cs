
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BedWakeUp : MonoBehaviour
{
    [Header("Sleeping Phoebe")]
    [SerializeField] private GameObject _sleepingPhoebe;
    [SerializeField] private GameObject _zzz;
    [SerializeField] private SpriteRenderer _sleepingRenderer;
    [SerializeField] private Sprite _eyesClosedSprite;
    [SerializeField] private Sprite _eyesOpenSprite;

    [Header("Player")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _wakePoint;
    [SerializeField] private Transform _entranceSpawnPoint;

    [Header("Camera")]
    [SerializeField] private NewCameraFollow _cameraFollow;
    [SerializeField] private Transform _sleepingCameraPoint;

    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    [Header("Timing")]
    [SerializeField] private float _sleepDuration = 3f;
    [SerializeField] private float _eyesOpenDuration = 1f;

    private PhoebeMovement _movement;
    private PlayerInput _playerInput;
    private SpriteRenderer _playerRenderer;
    private Rigidbody2D _playerRigidbody;
    private InputAction _interactAction;

    private bool _isSleeping;
    private bool _playerNearby;
    private bool _returnedFromOutside;

    private void Awake()
    {
        if (_player == null ||
            _sleepingPhoebe == null ||
            _sleepingRenderer == null ||
            _eyesClosedSprite == null ||
            _eyesOpenSprite == null ||
            _wakePoint == null)
        {
            Debug.LogError(
                "BedWakeUp: Missing references!",
                this
            );

            enabled = false;
            return;
        }

        _movement =
            _player.GetComponent<PhoebeMovement>();

        _playerInput =
            _player.GetComponent<PlayerInput>();

        _playerRenderer =
            _player.GetComponent<SpriteRenderer>();

        _playerRigidbody =
            _player.GetComponent<Rigidbody2D>();

        if (_playerInput != null)
        {
            _interactAction =
                _playerInput.actions.FindAction("Interact");
        }

        SetPromptVisible(false);

        _returnedFromOutside =
            SceneEntryState.ConsumeEnteringHouse();

        if (_returnedFromOutside)
            EnterFromOutside();
        else
            BeginSleeping();
    }

    private void Start()
    {
        if (!_returnedFromOutside)
            StartCoroutine(SleepSequence());
    }

    private void Update()
    {
        if (_isSleeping || !_playerNearby)
            return;

        if (DialogueManager.Instance == null)
            return;

        if (DialogueManager.Instance.IsOpen)
        {
            SetPromptVisible(false);
            return;
        }

        SetPromptVisible(true);

        if (_interactAction != null &&
            _interactAction.WasPressedThisFrame())
        {
            AskToSleep();
        }
    }

    private void AskToSleep()
    {
        if (DialogueManager.Instance == null ||
            DialogueManager.Instance.IsOpen)
            return;

        SetPromptVisible(false);

        DialogueManager.Instance.ShowChoice(
            "Phoebe",
            "Go back to sleep?",
            StartSleeping
        );
    }

    private void StartSleeping()
    {
        if (_isSleeping)
            return;

        StartCoroutine(SleepSequence());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckPlayerNearby(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckPlayerNearby(other);
    }

    private void CheckPlayerNearby(Collider2D other)
    {
        if (_isSleeping)
            return;

        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null || player != _playerInput)
            return;

        _playerNearby = true;

        if (DialogueManager.Instance != null &&
            !DialogueManager.Instance.IsOpen)
        {
            SetPromptVisible(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null || player != _playerInput)
            return;

        _playerNearby = false;
        SetPromptVisible(false);
    }

    private void EnterFromOutside()
    {
        _isSleeping = false;
        _playerNearby = false;

        _sleepingPhoebe.SetActive(false);

        if (_zzz != null)
            _zzz.SetActive(false);

        if (_entranceSpawnPoint == null)
        {
            Debug.LogError(
                "BedWakeUp: Entrance Spawn Point is missing!",
                this
            );
            return;
        }

        _player.transform.position =
            _entranceSpawnPoint.position;

        Physics2D.SyncTransforms();

        if (_playerRigidbody != null)
        {
            _playerRigidbody.linearVelocity = Vector2.zero;
            _playerRigidbody.simulated = true;
        }

        if (_playerRenderer != null)
            _playerRenderer.enabled = true;

        if (_movement != null)
            _movement.enabled = true;

        if (_playerInput != null)
            _playerInput.ActivateInput();

        if (_cameraFollow != null)
            _cameraFollow.SetTarget(_player.transform);
    }

    private void BeginSleeping()
    {
        _isSleeping = true;
        _playerNearby = false;

        SetPromptVisible(false);

        if (_movement != null)
            _movement.enabled = false;

        if (_playerInput != null)
            _playerInput.DeactivateInput();

        if (_playerRenderer != null)
            _playerRenderer.enabled = false;

        if (_playerRigidbody != null)
        {
            _playerRigidbody.linearVelocity = Vector2.zero;
            _playerRigidbody.simulated = false;
        }

        _sleepingRenderer.sprite = _eyesClosedSprite;
        _sleepingPhoebe.SetActive(true);

        if (_zzz != null)
            _zzz.SetActive(true);

        if (_cameraFollow != null &&
            _sleepingCameraPoint != null)
        {
            _cameraFollow.SetTarget(
                _sleepingCameraPoint
            );
        }
    }

    private IEnumerator SleepSequence()
    {
        BeginSleeping();

        // Sleeping animation.
        yield return new WaitForSeconds(_sleepDuration);

        // Phoebe opens her eyes.
        if (_zzz != null)
            _zzz.SetActive(false);

        _sleepingRenderer.sprite = _eyesOpenSprite;

        yield return new WaitForSeconds(
            _eyesOpenDuration
        );

        // Move Phoebe beside the bed.
        _player.transform.position =
            _wakePoint.position;

        Physics2D.SyncTransforms();

        _sleepingPhoebe.SetActive(false);

        if (_playerRigidbody != null)
        {
            _playerRigidbody.linearVelocity = Vector2.zero;
            _playerRigidbody.simulated = true;
        }

        if (_playerRenderer != null)
            _playerRenderer.enabled = true;

        if (_movement != null)
            _movement.enabled = true;

        if (_playerInput != null)
            _playerInput.ActivateInput();

        if (_cameraFollow != null)
            _cameraFollow.SetTarget(_player.transform);

        _isSleeping = false;
    }

    private void SetPromptVisible(bool visible)
    {
        if (_interactionPrompt != null &&
            _interactionPrompt.activeSelf != visible)
        {
            _interactionPrompt.SetActive(visible);
        }
    }
}
