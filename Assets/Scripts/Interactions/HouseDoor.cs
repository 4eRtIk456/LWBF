
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HouseDoor : MonoBehaviour
{
    [Header("Door Animation")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AnimationClip openingClip;
    [SerializeField] private AnimationClip closingClip;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Scene Transition")]
    [SerializeField] private string _destinationScene =
        "PhoebeHouseInterior";

    private PlayerInput nearbyPlayer;
    private InputAction interactAction;

    private bool isOpen;
    private bool isAnimating;

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (doorAnimator != null)
            doorAnimator.Play("Door_Closed");

        isOpen = false;
    }

    private void Update()
    {
        if (nearbyPlayer == null || interactAction == null)
            return;

        if (isAnimating)
            return;

        if (DialogueManager.Instance == null)
        {
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            return;
        }

        if (DialogueManager.Instance.IsOpen)
        {
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            return;
        }

        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);

        if (interactAction.WasPressedThisFrame())
            AskToEnter();
    }

    private void AskToEnter()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning(
                "HouseDoor: DialogueManager not found!",
                this
            );
            return;
        }

        if (DialogueManager.Instance.IsOpen)
            return;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        DialogueManager.Instance.ShowChoice(
            "Phoebe",
            "Enter the house?",
            EnterHouse
        );
    }

    private void EnterHouse()
    {
        if (isAnimating)
            return;

        StartCoroutine(OpenDoorAndEnter());
    }

    private IEnumerator OpenDoorAndEnter()
    {
        isAnimating = true;
        SfxManager.Instance?.PlayDoorEnter();

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (doorAnimator != null)
        {
            doorAnimator.Play("Door_Opening", 0, 0f);

            if (openingClip != null)
                yield return new WaitForSeconds(
                    openingClip.length
                );

            isOpen = true;

            doorAnimator.Play("Door_Opened", 0, 0f);
        }

        SceneEntryState.SetEnteringHouse();

        SceneManager.LoadScene(_destinationScene);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        nearbyPlayer = player;
        interactAction =
            player.actions.FindAction("Interact");

        if (interactionPrompt != null &&
            !isAnimating &&
            DialogueManager.Instance != null &&
            !DialogueManager.Instance.IsOpen)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null || player != nearbyPlayer)
            return;

        nearbyPlayer = null;
        interactAction = null;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}
