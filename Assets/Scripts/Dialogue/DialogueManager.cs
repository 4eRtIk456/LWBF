using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _speakerName;
    [SerializeField] private TMP_Text _dialogueText;

    [Header("Buttons")]
    [SerializeField] private Button _continueButton;
    [SerializeField] private GameObject _choicePanel;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    [Header("Phoebe Portrait")]
    [SerializeField] private GameObject _phoebePortrait;
    [SerializeField] private PhoebePortraitController _portraitController;

    [Header("Player")]
    [SerializeField] private PhoebeMovement _playerMovement;

    private Action _onYes;
    private Action _onNo;
    private bool _wasMovementEnabled;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "Another DialogueManager already exists.",
                this
            );

            enabled = false;
            return;
        }

        Instance = this;

        _continueButton.onClick.AddListener(CloseDialogue);
        _yesButton.onClick.AddListener(ChooseYes);
        _noButton.onClick.AddListener(ChooseNo);

        _dialoguePanel.SetActive(false);

        if (_phoebePortrait != null)
            _phoebePortrait.SetActive(false);
    }

  
    public void ShowDialogue(string speaker, string message)
    {
        ShowDialogue(
            speaker,
            message,
            PhoebeExpression.Neutral
        );
    }

    public void ShowDialogue(
        string speaker,
        string message,
        PhoebeExpression expression)
    {
        OpenDialogue();

        _speakerName.text = speaker;
        _dialogueText.text = message;

        _continueButton.gameObject.SetActive(true);
        _choicePanel.SetActive(false);

        UpdatePortrait(speaker, expression);
    }

    public void ShowChoice(
        string speaker,
        string question,
        Action onYes,
        Action onNo = null)
    {
        ShowChoice(
            speaker,
            question,
            onYes,
            onNo,
            PhoebeExpression.Neutral
        );
    }

    public void ShowChoice(
        string speaker,
        string question,
        Action onYes,
        Action onNo,
        PhoebeExpression expression)
    {
        OpenDialogue();

        _speakerName.text = speaker;
        _dialogueText.text = question;

        _onYes = onYes;
        _onNo = onNo;

        _continueButton.gameObject.SetActive(false);
        _choicePanel.SetActive(true);

        UpdatePortrait(speaker, expression);
    }

    private void UpdatePortrait(
        string speaker,
        PhoebeExpression expression)
    {
        if (_phoebePortrait == null)
            return;

        bool showPhoebe = speaker == "Phoebe";

        _phoebePortrait.SetActive(showPhoebe);

        if (showPhoebe && _portraitController != null)
        {
            _portraitController.SetExpression(expression);
        }
    }

    private void OpenDialogue()
    {
        if (IsOpen)
            return;

        IsOpen = true;

        if (_playerMovement != null)
        {
            _wasMovementEnabled = _playerMovement.enabled;
            _playerMovement.enabled = false;
        }

        _dialoguePanel.SetActive(true);
    }

    public void CloseDialogue()
    {
        if (!IsOpen)
            return;

        IsOpen = false;

        _dialoguePanel.SetActive(false);

        if (_phoebePortrait != null)
            _phoebePortrait.SetActive(false);

        _onYes = null;
        _onNo = null;

        if (_playerMovement != null)
            _playerMovement.enabled = _wasMovementEnabled;
    }

    private void ChooseYes()
    {
        Action callback = _onYes;
        CloseDialogue();
        callback?.Invoke();
    }

    private void ChooseNo()
    {
        Action callback = _onNo;
        CloseDialogue();
        callback?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
