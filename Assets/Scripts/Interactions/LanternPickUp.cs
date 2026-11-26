using UnityEngine;
using UnityEngine.InputSystem;

public class LanternPickup : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string _beforeLetterMessage =
        "I don't need my lantern right now.";

    [TextArea(2, 4)]
    [SerializeField] private string _beforeRecipeMessage =
        "I should check my recipe book first.";

    public static bool HasLantern => GameState.HasLantern;

    private PlayerInput _playerInput;
    private InputAction _interactAction;

    private void Start()
    {
        SetPromptVisible(false);

        if (GameState.HasLantern)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameState.HasLantern || _playerInput == null)
            return;

        DialogueManager dialogue = DialogueManager.Instance;

        if (dialogue == null)
            return;

        if (dialogue.IsOpen)
        {
            SetPromptVisible(false);
            return;
        }

        SetPromptVisible(true);

        if (_interactAction != null &&
            _interactAction.WasPressedThisFrame())
        {
            InteractWithLantern();
        }
    }

    private void InteractWithLantern()
    {
        DialogueManager dialogue = DialogueManager.Instance;

        if (dialogue == null || dialogue.IsOpen)
            return;

        SetPromptVisible(false);

        if (!GameState.HasReadLetter)
        {
            dialogue.ShowDialogue(
                "Phoebe",
                _beforeLetterMessage,
                PhoebeExpression.Neutral
            );

            return;
        }

        if (!GameState.HasCheckedRecipeBook)
        {
            dialogue.ShowDialogue(
                "Phoebe",
                _beforeRecipeMessage,
                PhoebeExpression.Neutral
            );

            return;
        }

        dialogue.ShowChoice(
            "Phoebe",
            "Pick up the lantern?",
            PickUpLantern
        );
    }

    private void PickUpLantern()
    {
        if (!GameState.HasCheckedRecipeBook ||
            GameState.HasLantern)
            return;

        GameState.CollectLantern();

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
        if (GameState.HasLantern)
            return;

        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        _playerInput = player;
        _interactAction =
            player.actions.FindAction("Interact");

        RefreshPrompt();
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

    private void RefreshPrompt()
    {
        bool dialogueOpen =
            DialogueManager.Instance != null &&
            DialogueManager.Instance.IsOpen;

        SetPromptVisible(
            _playerInput != null &&
            !GameState.HasLantern &&
            !dialogueOpen
        );
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
