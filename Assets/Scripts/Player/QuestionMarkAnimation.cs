using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMP_Text))]
public class QuestionMarkAnimation : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float _totalDuration = 2f;
    [SerializeField] private float _fadeInDuration = 0.3f;
    [SerializeField] private float _fadeOutDuration = 0.4f;

    [Header("Bounce")]
    [SerializeField] private float _bounceHeight = 5f;
    [SerializeField] private float _bounceDuration = 0.5f;

    [Header("Floating")]
    [SerializeField] private float _floatHeight = 1.5f;
    [SerializeField] private float _floatSpeed = 2f;

    private RectTransform _rectTransform;
    private TMP_Text _text;
    private Vector2 _startPosition;
    private Color _originalColor;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _text = GetComponent<TMP_Text>();

        _startPosition = _rectTransform.anchoredPosition;
        _originalColor = _text.color;
    }

    private void OnEnable()
    {
        _rectTransform.anchoredPosition = _startPosition;
        SetAlpha(0f);

        _animationCoroutine = StartCoroutine(AnimateQuestionMark());
    }

    private IEnumerator AnimateQuestionMark()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(_totalDuration, 0.01f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float fadeIn = Mathf.Clamp01(
                elapsed / Mathf.Max(_fadeInDuration, 0.01f)
            );

            float fadeOut = Mathf.Clamp01(
                (duration - elapsed) /
                Mathf.Max(_fadeOutDuration, 0.01f)
            );

            float alpha = Mathf.Min(fadeIn, fadeOut);
            SetAlpha(Mathf.SmoothStep(0f, 1f, alpha));

            float bounceProgress = Mathf.Clamp01(
                elapsed / Mathf.Max(_bounceDuration, 0.01f)
            );

            float bounce = Mathf.Sin(
                bounceProgress * Mathf.PI
            ) * _bounceHeight;

            float floating = Mathf.Sin(
                elapsed * _floatSpeed * Mathf.PI * 2f
            ) * _floatHeight;

            _rectTransform.anchoredPosition =
                _startPosition + Vector2.up * (bounce + floating);

            yield return null;
        }

        SetAlpha(0f);
        _rectTransform.anchoredPosition = _startPosition;
        _animationCoroutine = null;
    }

    private void OnDisable()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }

        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition = _startPosition;
        }

        if (_text != null)
        {
            _text.color = _originalColor;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = _originalColor;
        color.a *= alpha;
        _text.color = color;
    }
}
