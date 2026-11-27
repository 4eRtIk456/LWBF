using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeafInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    [Header("Collectible")]
    [SerializeField] private GameObject _leafObject;

    [Header("Dialogue")]
    [SerializeField] private string _speakerName = "Phoebe";

    [TextArea(2, 4)]
    [SerializeField] private string _message =
        "A Lunar Leaf! That was quite easy.";

    private PlayerInput _nearbyPlayer;
    private InputAction _interactAction;
    private bool _isCollecting;

    private void Start()
    {
        SetPromptVisible(false);

        if (GameState.HasFirstIngredient)
            HideLeaf();
    }

    private void Update()
    {
        if (_isCollecting ||
            _nearbyPlayer == null ||
            _interactAction == null)
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
            StartCoroutine(CollectIngredient());
    }

    private IEnumerator CollectIngredient()
    {
        DialogueManager dialogue = DialogueManager.Instance;

        if (dialogue == null)
            yield break;

        _isCollecting = true;
        SetPromptVisible(false);

        dialogue.ShowDialogue(
            "Phoebe",
            "A Lunar Leaf! That was quite easy.",
            PhoebeExpression.Happy
        );
        yield return new WaitUntil(
            () => dialogue == null || !dialogue.IsOpen
        );

        GameState.CollectFirstIngredient();
        Debug.Log("First ingredient collected!");

        HideLeaf();
    }

    private void HideLeaf()
    {
        if (_leafObject != null)
            _leafObject.SetActive(false);
        else
            gameObject.SetActive(false);
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

        if (!_isCollecting &&
            !GameState.HasFirstIngredient &&
            DialogueManager.Instance != null &&
            !DialogueManager.Instance.IsOpen)
        {
            SetPromptVisible(true);
        }
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
