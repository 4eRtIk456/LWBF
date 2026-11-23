
using UnityEngine;
using UnityEngine.InputSystem;

public class LanternPickup : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    public static bool HasLantern { get; private set; }

    private PlayerInput _playerInput;
    private InputAction _interactAction;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetLanternState()
    {
        HasLantern = false;
    }

    private void Start()
    {
        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(false);

        if (HasLantern)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (HasLantern || _playerInput == null)
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
            AskToPickUp();
        }
    }

    private void AskToPickUp()
    {
        if (DialogueManager.Instance == null ||
            DialogueManager.Instance.IsOpen)
            return;

        SetPromptVisible(false);

        DialogueManager.Instance.ShowChoice(
            "Phoebe",
            "Pick up the lantern?",
            PickUpLantern
        );
    }

    private void PickUpLantern()
    {
        HasLantern = true;

        Debug.Log("Phoebe picked up the lantern!");

        SetPromptVisible(false);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckPlayer(other);
    }

    private void CheckPlayer(Collider2D other)
    {
        if (HasLantern)
            return;

        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        _playerInput = player;
        _interactAction =
            player.actions.FindAction("Interact");

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

        _playerInput = null;
        _interactAction = null;

        SetPromptVisible(false);
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
