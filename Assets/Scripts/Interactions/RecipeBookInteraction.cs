using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecipeBookInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string _beforeLetterMessage =
        "Nothing to look up just yet.";

    [TextArea(2, 4)]
    [SerializeField] private string _afterLetterMessage =
        "A potion to bring back what was lost... Let's see...";

    private PlayerInput _nearbyPlayer;
    private InputAction _interactAction;
    private bool _isBusy;

    private void Start()
    {
        SetPromptVisible(false);
    }

    private void Update()
    {
        if (_nearbyPlayer == null ||
            _interactAction == null ||
            _isBusy)
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

        if (_interactAction.WasPressedThisFrame())
            StartCoroutine(ReadRecipeBook());
    }

    private IEnumerator ReadRecipeBook()
    {
        _isBusy = true;
        SetPromptVisible(false);

        DialogueManager dialogue = DialogueManager.Instance;

        if (dialogue == null)
        {
            _isBusy = false;
            yield break;
        }

        bool hasReadLetter = GameState.HasReadLetter;

        if (hasReadLetter)
        {
            dialogue.ShowDialogue(
                "Phoebe",
                _afterLetterMessage,
                PhoebeExpression.Serious
            );
        }
        else
        {
            dialogue.ShowDialogue(
                "Phoebe",
                _beforeLetterMessage,
                PhoebeExpression.Neutral
            );
        }

        yield return new WaitUntil(
            () => !dialogue.IsOpen
        );

        if (hasReadLetter && !GameState.HasCheckedRecipeBook)
        {
            GameState.CheckRecipeBook();
        }

        _isBusy = false;
        RefreshPrompt();
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
        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        _nearbyPlayer = player;
        _interactAction =
            player.actions.FindAction("Interact");

        RefreshPrompt();
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

    private void RefreshPrompt()
    {
        bool dialogueOpen =
            DialogueManager.Instance != null &&
            DialogueManager.Instance.IsOpen;

        SetPromptVisible(
            _nearbyPlayer != null &&
            !_isBusy &&
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
