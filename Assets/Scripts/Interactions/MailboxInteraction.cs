
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MailboxInteraction : MonoBehaviour
{
    [Header("Mailbox")]
    [SerializeField] private Animator mailboxAnimator;
    [SerializeField] private AnimationClip openingClip;
    [SerializeField] private AnimationClip closingClip;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float dialogueDuration = 3f;

    private PlayerInput nearbyPlayer;
    private InputAction interactAction;

    private bool hasLetter = true;
    private bool isAnimating = false;
    private Coroutine dialogueCoroutine;

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (mailboxAnimator != null)
            mailboxAnimator.Play("Mailbox_Message", 0, 0f);
    }

    private void Update()
    {
        if (nearbyPlayer == null || interactAction == null)
            return;

        if (isAnimating || dialogueCoroutine != null)
            return;

        if (interactAction.WasPressedThisFrame())
        {
            if (hasLetter)
                StartCoroutine(CollectLetter());
            else
                ShowDialogue("Empty.");
        }
    }

    private IEnumerator CollectLetter()
    {
        if (mailboxAnimator == null)
            yield break;

        isAnimating = true;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        mailboxAnimator.Play("Mailbox_Opening", 0, 0f);

        if (openingClip != null)
            yield return new WaitForSeconds(openingClip.length);

        hasLetter = false;

        ShowDialogue("I guess this is a new order. Nice.");

        if (dialogueCoroutine != null)
            yield return dialogueCoroutine;

        mailboxAnimator.Play("Mailbox_Closing", 0, 0f);

        if (closingClip != null)
            yield return new WaitForSeconds(closingClip.length);

        mailboxAnimator.Play("Mailbox_Closed", 0, 0f);

        isAnimating = false;

        if (nearbyPlayer != null && interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    private void ShowDialogue(string message)
    {
        if (dialoguePanel == null || dialogueText == null)
            return;

        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine = StartCoroutine(DisplayDialogue(message));
    }

    private IEnumerator DisplayDialogue(string message)
    {

        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        yield return new WaitForSeconds(dialogueDuration);

        dialoguePanel.SetActive(false);
        dialogueCoroutine = null;

        if (nearbyPlayer != null && !isAnimating &&
            interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInput player = other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        nearbyPlayer = player;
        interactAction = player.actions.FindAction("Interact");

        if (interactionPrompt != null && !isAnimating &&
            dialogueCoroutine == null)
            interactionPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInput player = other.GetComponentInParent<PlayerInput>();

        if (player == null || player != nearbyPlayer)
            return;

        nearbyPlayer = null;
        interactAction = null;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}
