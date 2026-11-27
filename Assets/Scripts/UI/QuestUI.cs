using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestUI : MonoBehaviour
{
    [Header("Notification")]
    [SerializeField] private RectTransform _questNotification;
    [SerializeField] private float _slideDuration = 0.6f;
    [SerializeField] private float _offscreenPadding = 50f;

    [Header("Journal")]
    [SerializeField] private GameObject _journalPanel;
    [SerializeField] private TMP_Text _journalText;

    private static QuestStage? _acknowledgedQuestStage;

    private Vector2 _notificationPosition;
    private Coroutine _slideCoroutine;
    private QuestStage _lastQuestStage = QuestStage.Intro;
    private PhoebeMovement _playerMovement;
    private bool _wasMovementEnabled;
    private bool _journalOpen;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetNotificationState()
    {
        _acknowledgedQuestStage = null;
    }

    private void Awake()
    {
        if (_questNotification == null ||
            _journalPanel == null ||
            _journalText == null)
        {
            Debug.LogError(
                "QuestUI: Missing references!",
                this
            );

            enabled = false;
            return;
        }

        _notificationPosition =
            _questNotification.anchoredPosition;

        _questNotification.gameObject.SetActive(false);
        _journalPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameState.OnStateChanged += RefreshQuest;
        RefreshQuest();
    }

    private void OnDisable()
    {
        GameState.OnStateChanged -= RefreshQuest;

        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
            _slideCoroutine = null;
        }

        RestorePlayerMovement();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        bool qPressed = Keyboard.current.qKey.wasPressedThisFrame;
        bool escapePressed =
            Keyboard.current.escapeKey.wasPressedThisFrame;

        if (_journalOpen && (qPressed || escapePressed))
        {
            CloseJournal();
            return;
        }

        if (!qPressed || _journalOpen)
            return;

        if (!GameState.HasFinishedMorningIntro)
            return;

        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsOpen)
            return;

        OpenJournal();
    }

    private void RefreshQuest()
    {
        if (_journalText == null)
            return;

        UpdateJournalText();

        QuestStage currentStage = GameState.CurrentQuest;

        if (currentStage == QuestStage.Intro)
        {
            HideNotification();
            _lastQuestStage = currentStage;
            return;
        }

        if (currentStage == _lastQuestStage)
            return;

        _lastQuestStage = currentStage;

        if (_acknowledgedQuestStage == currentStage)
        {
            HideNotification();
            return;
        }

        ShowNotification();
    }

    private void ShowNotification()
    {
        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
            _slideCoroutine = null;
        }

        _questNotification.gameObject.SetActive(true);
        _slideCoroutine = StartCoroutine(SlideNotification());
    }

    private void HideNotification()
    {
        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
            _slideCoroutine = null;
        }

        _questNotification.gameObject.SetActive(false);
    }

    private IEnumerator SlideNotification()
    {
        float offscreenX =
            _questNotification.rect.width *
            _questNotification.pivot.x +
            _offscreenPadding;

        Vector2 startPosition = new Vector2(
            offscreenX,
            _notificationPosition.y
        );

        _questNotification.anchoredPosition = startPosition;

        float elapsed = 0f;

        while (elapsed < _slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(_slideDuration, 0.01f)
            );

            t = t * t * (3f - 2f * t);

            _questNotification.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    _notificationPosition,
                    t
                );

            yield return null;
        }

        _questNotification.anchoredPosition =
            _notificationPosition;

        _slideCoroutine = null;
    }

    private void OpenJournal()
    {
        if (_journalOpen)
            return;

        _acknowledgedQuestStage = GameState.CurrentQuest;
        HideNotification();

        UpdateJournalText();

        _playerMovement =
            FindFirstObjectByType<PhoebeMovement>();

        if (_playerMovement != null)
        {
            _wasMovementEnabled = _playerMovement.enabled;
            _playerMovement.enabled = false;
        }

        _journalPanel.SetActive(true);
        _journalOpen = true;
    }

    private void CloseJournal()
    {
        if (!_journalOpen)
            return;

        _journalOpen = false;
        _journalPanel.SetActive(false);

        RestorePlayerMovement();
    }

    private void RestorePlayerMovement()
    {
        if (_playerMovement != null)
        {
            _playerMovement.enabled = _wasMovementEnabled;
            _playerMovement = null;
        }
    }

    private void UpdateJournalText()
    {
        StringBuilder text = new StringBuilder();

        if (GameState.HasFinishedMorningIntro)
        {
            AddQuest(
                text,
                "Check the mailbox",
                GameState.HasReadLetter
            );
        }

        if (GameState.HasReadLetter)
        {
            AddQuest(
                text,
                "Check the recipe book",
                GameState.HasCheckedRecipeBook
            );
        }

        if (GameState.HasCheckedRecipeBook)
        {
            AddQuest(
                text,
                "Pick up the lantern",
                GameState.HasLantern
            );
        }

        if (GameState.HasLantern)
        {
            text.AppendLine();
            text.AppendLine("Ingredients:");

            AddQuest(
                text,
                "First ingredient",
                GameState.HasFirstIngredient
            );

            AddQuest(
                text,
                "Second ingredient",
                GameState.HasSecondIngredient
            );

            AddQuest(
                text,
                "Third ingredient",
                GameState.HasThirdIngredient
            );
        }

        if (GameState.IngredientCount == 3)
        {
            text.AppendLine();

            AddQuest(
                text,
                "Brew the potion",
                false
            );
        }

        _journalText.text = text.ToString();
    }

    private void AddQuest(
        StringBuilder text,
        string description,
        bool completed)
    {
        if (completed)
        {
            text.AppendLine(
                "<color=#967F70>[x] <s>" +
                description +
                "</s></color>"
            );
        }
        else
        {
            text.AppendLine(
                "[ ] " + description
            );
        }
    }
}
