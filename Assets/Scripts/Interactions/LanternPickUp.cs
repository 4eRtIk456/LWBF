
using UnityEngine;
using UnityEngine.InputSystem;

public class LanternPickup : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    public static bool HasLantern { get; private set; }

    private PlayerInput _playerInput;

    private void Start()
    {
        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(false);

        if (HasLantern)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_playerInput == null || HasLantern)
            return;

        InputAction interactAction =
            _playerInput.actions.FindAction("Interact");

        if (interactAction != null && interactAction.WasPressedThisFrame())
            PickUpLantern();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (HasLantern)
            return;

        PlayerInput player = other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        _playerInput = player;

        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInput player = other.GetComponentInParent<PlayerInput>();

        if (player == null || player != _playerInput)
            return;

        _playerInput = null;

        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(false);
    }

    private void PickUpLantern()
    {
        HasLantern = true;

        Debug.Log("Phoebe picked up the lantern!");

        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(false);

        gameObject.SetActive(false);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetLanternState()
    {
        HasLantern = false;
}
}

