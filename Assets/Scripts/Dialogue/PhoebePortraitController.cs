
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum PhoebeExpression
{
    Neutral,
    Serious,
    Worried,
    Happy,
    Angry,
    Shocked,
    Yawn
}

public class PhoebePortraitController : MonoBehaviour
{
    [Header("Portrait Image")]
    [SerializeField] private Image _portraitImage;

    [Header("Neutral")]
    [SerializeField] private Sprite _neutral;
    [SerializeField] private Sprite _neutralClosed;

    [Header("Serious")]
    [SerializeField] private Sprite _serious;
    [SerializeField] private Sprite _seriousClosed;

    [Header("Worried")]
    [SerializeField] private Sprite _worried;
    [SerializeField] private Sprite _worriedClosed;

    [Header("Happy / Confident")]
    [SerializeField] private Sprite _happy;
    [SerializeField] private Sprite _happyClosed;

    [Header("Angry")]
    [SerializeField] private Sprite _angry;
    [SerializeField] private Sprite _angryClosed;

    [Header("Shocked")]
    [SerializeField] private Sprite _shocked;
    [SerializeField] private Sprite _shockedClosed;

    [Header("Yawn")]
    [SerializeField] private Sprite _yawn;

    [Header("Blink Settings")]
    [SerializeField] private float _minimumBlinkInterval = 3f;
    [SerializeField] private float _maximumBlinkInterval = 4f;
    [SerializeField] private float _blinkDuration = 0.15f;

    private PhoebeExpression _currentExpression =
        PhoebeExpression.Neutral;

    private Coroutine _blinkCoroutine;

    private void Awake()
    {
        if (_portraitImage == null)
        {
            _portraitImage = GetComponent<Image>();
        }

        SetExpression(PhoebeExpression.Neutral);
    }

    private void OnEnable()
    {
        if (_portraitImage != null)
        {
            ShowCurrentExpression();
            _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }
    }

    private void OnDisable()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    public void SetExpression(PhoebeExpression expression)
    {
        _currentExpression = expression;

        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        ShowCurrentExpression();

        if (isActiveAndEnabled)
        {
            _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float delay = Random.Range(
                _minimumBlinkInterval,
                _maximumBlinkInterval
            );

            yield return new WaitForSecondsRealtime(delay);

            // Yawning uses a single sprite.
            if (_currentExpression == PhoebeExpression.Yawn)
                continue;

            Sprite closedSprite = GetClosedSprite();

            if (closedSprite == null)
                continue;

            _portraitImage.sprite = closedSprite;

            yield return new WaitForSecondsRealtime(
                _blinkDuration
            );

            ShowCurrentExpression();
        }
    }

    private void ShowCurrentExpression()
    {
        if (_portraitImage == null)
            return;

        switch (_currentExpression)
        {
            case PhoebeExpression.Neutral:
                _portraitImage.sprite = _neutral;
                break;

            case PhoebeExpression.Serious:
                _portraitImage.sprite = _serious;
                break;

            case PhoebeExpression.Worried:
                _portraitImage.sprite = _worried;
                break;

            case PhoebeExpression.Happy:
                _portraitImage.sprite = _happy;
                break;

            case PhoebeExpression.Angry:
                _portraitImage.sprite = _angry;
                break;

            case PhoebeExpression.Shocked:
                _portraitImage.sprite = _shocked;
                break;

            case PhoebeExpression.Yawn:
                _portraitImage.sprite = _yawn;
                break;
        }
    }

    private Sprite GetClosedSprite()
    {
        switch (_currentExpression)
        {
            case PhoebeExpression.Neutral:
                return _neutralClosed;

            case PhoebeExpression.Serious:
                return _seriousClosed;

            case PhoebeExpression.Worried:
                return _worriedClosed;

            case PhoebeExpression.Happy:
                return _happyClosed;

            case PhoebeExpression.Angry:
                return _angryClosed;

            case PhoebeExpression.Shocked:
                return _shockedClosed;

            default:
                return null;
        }
    }
}
