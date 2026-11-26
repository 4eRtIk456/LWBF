using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MailboxInteraction : MonoBehaviour
{
    [Header("Mailbox")]
    [SerializeField] private Animator mailboxAnimator;
    [SerializeField] private AnimationClip openingClip;
    [SerializeField] private AnimationClip closingClip;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Letter UI")]
    [SerializeField] private LetterSequenceUI _letterUI;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string _letterMessage =
        "I guess this is a new order. Nice.";

    [TextArea(2, 4)]
    [SerializeField] private string _emptyMessage = "Empty.";

    private PlayerInput _nearbyPlayer;
    private InputAction _interactAction;

    private bool _isBusy;

    private void Start()
    {
        SetPromptVisible(false);

        if (mailboxAnimator == null)
            return;

        if (GameState.HasReadLetter)
        {
            mailboxAnimator.Play("Mailbox_Closed", 0, 0f);
        }
        else
        {
            mailboxAnimator.Play("Mailbox_Message", 0, 0f);
        }
    }

    private void Update()
    {
        if (_nearbyPlayer == null ||
            _interactAction == null ||
            _isBusy)
        {
            return;
        }

        DialogueManager dialogue = DialogueManager.Instance;

        if (dialogue != null && dialogue.IsOpen)
        {
            SetPromptVisible(false);
            return;
        }

        SetPromptVisible(true);

        if (!_interactAction.WasPressedThisFrame())
            return;

        if (GameState.HasReadLetter)
        {
            StartCoroutine(ReadEmptyMailbox());
        }
        else
        {
            StartCoroutine(CollectLetter());
        }
    }

    private IEnumerator CollectLetter()
    {
        _isBusy = true;
        SetPromptVisible(false);

        // Open the mailbox.
        if (mailboxAnimator != null)
        {
            mailboxAnimator.Play(
                "Mailbox_Opening", 0, 0f
            );

            if (openingClip != null)
            {
                yield return new WaitForSeconds(
                    openingClip.length
                );
            }
        }

        // Phoebe speaks before the letter appears.
        yield return ShowMailboxDialogue(_letterMessage);

        // Open the illustrated letter after Continue.
        if (_letterUI != null)
        {
            PhoebeMovement movement = null;

            if (_nearbyPlayer != null)
            {
                movement =
                    _nearbyPlayer.GetComponentInParent<PhoebeMovement>();
            }

            bool wasMovementEnabled =
                movement != null && movement.enabled;

            if (movement != null)
            {
                movement.enabled = false;
            }

            bool letterClosed = false;

            _letterUI.OpenLetter(
                () => letterClosed = true
            );

            yield return new WaitUntil(
                () => letterClosed
            );

            if (movement != null)
            {
                movement.enabled = wasMovementEnabled;
            }
        }
        else
        {
            Debug.LogWarning(
                "MailboxInteraction: Letter UI is not assigned!",
                this
            );
        }

        // Remember that the letter was read.
        GameState.ReadLetter();

        // Close the mailbox.
        if (mailboxAnimator != null)
        {
            mailboxAnimator.Play(
                "Mailbox_Closing", 0, 0f
            );

            if (closingClip != null)
            {
                yield return new WaitForSeconds(
                    closingClip.length
                );
            }

            mailboxAnimator.Play(
                "Mailbox_Closed", 0, 0f
            );
        }

        _isBusy = false;
        RefreshPrompt();
    }

    private IEnumerator ReadEmptyMailbox()
    {
        _isBusy = true;
        SetPromptVisible(false);

        yield return ShowMailboxDialogue(_emptyMessage);

        _isBusy = false;
        RefreshPrompt();
    }

    private IEnumerator ShowMailboxDialogue(string message)
    {
        DialogueManager dialogue = DialogueManager.Instance;

        if (dialogue == null)
        {
            Debug.LogWarning(
                "MailboxInteraction: DialogueManager not found!",
                this
            );

            yield break;
        }

        dialogue.ShowDialogue(
            "Phoebe",
            message,
            message == _letterMessage
                ? PhoebeExpression.Happy
                : PhoebeExpression.Neutral
        );

        // Wait until Continue closes the dialogue.
        yield return new WaitUntil(
            () => dialogue == null || !dialogue.IsOpen
        );
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
        if (interactionPrompt != null &&
            interactionPrompt.activeSelf != visible)
        {
            interactionPrompt.SetActive(visible);
        }
    }
}
