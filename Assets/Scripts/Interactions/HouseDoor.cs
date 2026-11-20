
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HouseDoor : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AnimationClip openingClip;
    [SerializeField] private AnimationClip closingClip;
    [SerializeField] private GameObject interactionPrompt;

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
    }

    private void Update()
    {
        if (nearbyPlayer == null || interactAction == null)
            return;

        if (isAnimating)
            return;

        if (interactAction.WasPressedThisFrame())
            StartCoroutine(ToggleDoor());
    }

    private IEnumerator ToggleDoor()
    {
        if (doorAnimator == null)
            yield break;

        isAnimating = true;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (!isOpen)
        {
            doorAnimator.Play("Door_Opening", 0, 0f);

            if (openingClip != null)
                yield return new WaitForSeconds(openingClip.length);

            isOpen = true;
            doorAnimator.Play("Door_Opened", 0, 0f);
        }
        else
        {
            doorAnimator.Play("Door_Closing", 0, 0f);

            if (closingClip != null)
                yield return new WaitForSeconds(closingClip.length);

            isOpen = false;
            doorAnimator.Play("Door_Closed", 0, 0f);
        }

        isAnimating = false;

        if (nearbyPlayer != null && interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInput player = other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        nearbyPlayer = player;
        interactAction = player.actions.FindAction("Interact");

        if (interactionPrompt != null && !isAnimating)
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

