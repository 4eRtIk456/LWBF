
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteriorExit : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPrompt;

    [Header("Destination")]
    [SerializeField] private string _destinationScene = "OutsideScene";

    private PlayerInput _playerInput;
    private InputAction _interactAction;
    private bool _playerNearby;

    private void Awake()
    {
        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!_playerNearby || _interactAction == null)
            return;

        if (!_interactAction.WasPressedThisFrame())
            return;

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManager not found!");
            return;
        }

        if (DialogueManager.Instance.IsOpen)
            return;

        DialogueManager.Instance.ShowChoice(
            "Phoebe",
            "Leave the house?",
            LeaveHouse
        );
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

        _playerInput = player;
        _interactAction =
            player.actions.FindAction("Interact");

        _playerNearby = true;

        if (_interactionPrompt != null &&
            (DialogueManager.Instance == null ||
             !DialogueManager.Instance.IsOpen))
        {
            _interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null || player != _playerInput)
            return;

        _playerNearby = false;
        _playerInput = null;
        _interactAction = null;

        if (_interactionPrompt != null)
            _interactionPrompt.SetActive(false);
    }

    private void LeaveHouse()
    {
        SceneManager.LoadScene(_destinationScene);
    }
}
