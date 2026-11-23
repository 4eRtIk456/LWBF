
using UnityEngine;
using UnityEngine.InputSystem;

public class LeafInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    [Header("Dialogue")]
    [SerializeField] private string _speakerName = "Phoebe";

    [TextArea(2, 4)]
    [SerializeField] private string _message =
        "A Lunar Leaf! That was quite easy.";

    private PlayerInput _nearbyPlayer;
    private InputAction _interactAction;

    private void Start()
    {
        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (_nearbyPlayer == null || _interactAction == null)
            return;

        if (DialogueManager.Instance == null)
            return;

        if (DialogueManager.Instance.IsOpen)
        {
            SetPromptVisible(false);
            return;
        }

        SetPromptVisible(true);

        if (_interactAction.WasPressedThisFrame())
        {
            SetPromptVisible(false);

            DialogueManager.Instance.ShowDialogue(
                _speakerName,
                _message
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        _nearbyPlayer = player;
        _interactAction =
            player.actions.FindAction("Interact");

        SetPromptVisible(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null || player != _nearbyPlayer)
            return;

        _nearbyPlayer = null;
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
