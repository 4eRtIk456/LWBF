
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LetterSequenceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _letterRoot;
    [SerializeField] private RectTransform _letterImageTransform;
    [SerializeField] private Image _letterImage;
    [SerializeField] private Animator _letterAnimator;
    [SerializeField] private TMP_Text _letterText;
    [SerializeField] private TMP_Text _clickHint;

    [Header("Letter Sprites")]
    [SerializeField] private Sprite _closedSprite;

    [Header("Zoom Animation")]
    [SerializeField] private float _startScale = 0.65f;
    [SerializeField] private float _endScale = 1f;
    [SerializeField] private float _zoomDuration = 0.5f;

    [Header("Opening Animation")]
    [SerializeField] private float _openingDuration = 0.9f;

    [Header("Text Animation")]
    [SerializeField] private float _secondsPerCharacter = 0.035f;

    [TextArea(5, 12)]
    [SerializeField] private string _message =
        "Dear Phoebe,\n\n" +
        "I wish to request a potion that can bring back what was lost.\n\n" +
        "Some things are not as gone as they seem.\n\n" +
        "Please prepare it before it is too late.\n\n" +
        "— A Mysterious Customer";

    private enum LetterStage
    {
        Hidden,
        Zooming,
        Closed,
        Opening,
        BlankPaper,
        Typing,
        Reading
    }

    private LetterStage _stage = LetterStage.Hidden;

    private Coroutine _zoomCoroutine;
    private Coroutine _typingCoroutine;

    private Action _onClosed;
    private int _visibleCharacters;

    private void Awake()
    {
        _letterRoot.SetActive(false);
    }

    private void Update()
    {
        if (_stage == LetterStage.Hidden)
            return;

        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        switch (_stage)
        {
            case LetterStage.Closed:
                StartCoroutine(PlayOpening());
                break;

            case LetterStage.BlankPaper:
                StartTyping();
                break;

            case LetterStage.Typing:
                FinishTyping();
                break;

            case LetterStage.Reading:
                CloseLetter();
                break;
        }
    }

    public void OpenLetter(Action onClosed = null)
    {
        if (_stage != LetterStage.Hidden)
            return;

        _onClosed = onClosed;
        _stage = LetterStage.Zooming;

        _letterRoot.SetActive(true);

        _letterAnimator.Play("Letter_Closed", 0, 0f);
        _letterImage.sprite = _closedSprite;

        _letterImageTransform.localScale =
            Vector3.one * _startScale;

        _letterText.text = _message;
        _letterText.maxVisibleCharacters = 0;
        _letterText.gameObject.SetActive(false);

        _clickHint.text = "";

        _zoomCoroutine = StartCoroutine(ZoomLetter());
    }

    private IEnumerator ZoomLetter()
    {
        float elapsed = 0f;

        while (elapsed < _zoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / _zoomDuration);
            t = t * t * (3f - 2f * t);

            float scale = Mathf.Lerp(
                _startScale,
                _endScale,
                t
            );

            _letterImageTransform.localScale =
                Vector3.one * scale;

            yield return null;
        }

        _letterImageTransform.localScale =
            Vector3.one * _endScale;

        _stage = LetterStage.Closed;
        _clickHint.text = "Click to open";
        _zoomCoroutine = null;
    }

    private IEnumerator PlayOpening()
    {
        _stage = LetterStage.Opening;
        _clickHint.text = "";

        _letterAnimator.SetTrigger("OpenLetter");

        yield return new WaitForSecondsRealtime(_openingDuration);

        _stage = LetterStage.BlankPaper;
        _clickHint.text = "Click to read";
    }

    private void StartTyping()
    {
        _stage = LetterStage.Typing;
        _visibleCharacters = 0;

        _letterText.gameObject.SetActive(true);
        _letterText.text = _message;
        _letterText.maxVisibleCharacters = 0;
        _letterText.ForceMeshUpdate();

        _clickHint.text = "Click to skip";

        _typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        int totalCharacters = _letterText.textInfo.characterCount;

        while (_visibleCharacters < totalCharacters)
        {
            _visibleCharacters++;

            _letterText.maxVisibleCharacters =
                _visibleCharacters;

            yield return new WaitForSecondsRealtime(
                _secondsPerCharacter
            );
        }

        _typingCoroutine = null;
        _stage = LetterStage.Reading;
        _clickHint.text = "Click to close";
    }

    private void FinishTyping()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        _letterText.maxVisibleCharacters = int.MaxValue;

        _stage = LetterStage.Reading;
        _clickHint.text = "Click to close";
    }

    public void CloseLetter()
    {
        if (_stage == LetterStage.Hidden)
            return;

        StopAllCoroutines();

        _letterRoot.SetActive(false);
        _stage = LetterStage.Hidden;

        Action callback = _onClosed;
        _onClosed = null;

        callback?.Invoke();
    }

    public bool IsOpen()
    {
        return _stage != LetterStage.Hidden;
    }
}

