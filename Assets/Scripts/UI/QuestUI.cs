using System.Collections;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform _questBackground;
    [SerializeField] private TMP_Text _questText;

    [Header("Slide Animation")]
    [SerializeField] private float _slideDuration = 0.65f;
    [SerializeField] private float _offscreenPadding = 40f;

    private Vector2 _visiblePosition;
    private Coroutine _slideCoroutine;

    private void Awake()
    {
        if (_questBackground == null || _questText == null)
        {
            Debug.LogError(
                "QuestUI: Missing UI references!",
                this
            );

            enabled = false;
            return;
        }

        _visiblePosition = _questBackground.anchoredPosition;
        _questBackground.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameState.OnStateChanged += UpdateQuest;
        UpdateQuest();
    }

    private void OnDisable()
    {
        GameState.OnStateChanged -= UpdateQuest;

        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
            _slideCoroutine = null;
        }
    }

    private void UpdateQuest()
    {
        if (_questBackground == null || _questText == null)
            return;

        string description = GameState.GetQuestDescription();

        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
            _slideCoroutine = null;
        }

        if (string.IsNullOrEmpty(description))
        {
            _questBackground.gameObject.SetActive(false);
            return;
        }

        _questText.text = "New Quest:\n" + description;
        _questBackground.gameObject.SetActive(true);

        _slideCoroutine = StartCoroutine(SlideIn());
    }

    private IEnumerator SlideIn()
    {
        float offscreenX =
            _questBackground.rect.width *
            (1f - _questBackground.pivot.x) +
            _offscreenPadding;

        Vector2 startPosition = new Vector2(
            offscreenX,
            _visiblePosition.y
        );

        _questBackground.anchoredPosition = startPosition;

        float elapsed = 0f;

        while (elapsed < _slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / _slideDuration
            );

            t = t * t * (3f - 2f * t);

            _questBackground.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    _visiblePosition,
                    t
                );

            yield return null;
        }

        _questBackground.anchoredPosition =
            _visiblePosition;

        _slideCoroutine = null;
    }
}
