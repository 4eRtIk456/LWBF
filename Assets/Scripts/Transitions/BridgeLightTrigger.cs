using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(BoxCollider2D))]
public class BridgeLightTrigger : MonoBehaviour
{
    [Header("Ghost")]
    [SerializeField] private IvyGhostAppearance _ivyGhost;
    [SerializeField] private MonoBehaviour _playerMovementScript;
    [SerializeField] private Rigidbody2D _playerRigidbody;
    [SerializeField] private GameObject _questionMark;
    [SerializeField] private float _freezeDuration = 2f;

    [Header("Phoebe Dialogue")]
    [SerializeField] private string _worriedMessage = "???";
    [SerializeField] private string _shockedMessage = "Huh, what's going on?";

    [Header("Phoebe Jump")]
    [SerializeField] private Transform _playerVisual;
    [SerializeField] private float _jumpHeight = 0.3f;
    [SerializeField] private float _jumpDuration = 0.35f;
    [SerializeField] private float _portraitJumpHeight = 35f;

    [Header("Camera Zoom")]
    [SerializeField] private Camera _zoomCamera;
    [SerializeField] private float _zoomMultiplier = 0.8f;
    [SerializeField] private float _zoomDuration = 0.6f;

    [Header("Lighting")]
    [SerializeField] private Light2D _lightToDim;
    [SerializeField] private Volume _moonVolume;

    [Header("Transition Settings")]
    [SerializeField] private float _darkIntensity = 0.1f;
    [SerializeField] private float _fadeDuration = 2.5f;

    private BoxCollider2D _triggerCollider;
    private float _normalIntensity;
    private bool _isDark;
    private bool _ghostEventPlayed;
    private Coroutine _fadeCoroutine;
    private Coroutine _zoomCoroutine;
    private Camera _activeCamera;
    private float _originalCameraSize;
    private Vector3 _portraitStartPosition;
    private bool _isPortraitJumping;

    private void Awake()
    {
        _triggerCollider = GetComponent<BoxCollider2D>();

        if (_lightToDim != null)
        {
            _normalIntensity = _lightToDim.intensity;
        }

        if (_moonVolume != null)
        {
            _moonVolume.weight = 0f;
        }

        if (_questionMark != null)
        {
            _questionMark.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckPlayerSide(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckPlayerSide(other);
    }

    private void CheckPlayerSide(Collider2D other)
    {
        Rigidbody2D playerBody = other.attachedRigidbody;

        if (playerBody == null ||
            !playerBody.CompareTag("Player") ||
            _lightToDim == null ||
            _moonVolume == null)
        {
            return;
        }

        bool shouldBeDark =
            playerBody.position.x > _triggerCollider.bounds.center.x;

        if (shouldBeDark && !_ghostEventPlayed && _ivyGhost != null)
        {
            _ghostEventPlayed = true;
            StartCoroutine(PlayGhostEvent());
        }

        if (shouldBeDark == _isDark)
        {
            return;
        }

        _isDark = shouldBeDark;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        float targetLight = _isDark
            ? _darkIntensity
            : _normalIntensity;

        float targetWeight = _isDark ? 1f : 0f;

        _fadeCoroutine = StartCoroutine(
            FadeEnvironment(targetLight, targetWeight)
        );
    }

    private IEnumerator PlayGhostEvent()
    {
        bool movementWasEnabled =
            _playerMovementScript != null &&
            _playerMovementScript.enabled;

        if (_playerMovementScript != null)
        {
            _playerMovementScript.enabled = false;
        }

        if (_playerRigidbody != null)
        {
            _playerRigidbody.linearVelocity = Vector2.zero;
        }

        _activeCamera = _zoomCamera != null
            ? _zoomCamera
            : Camera.main;

        if (_activeCamera != null && _activeCamera.orthographic)
        {
            _originalCameraSize = _activeCamera.orthographicSize;
        }
        else
        {
            _activeCamera = null;
        }

        _ivyGhost.ShowIdle();

        if (_questionMark != null)
        {
            _questionMark.SetActive(true);
        }

        yield return new WaitForSeconds(_freezeDuration);

        if (_questionMark != null)
        {
            _questionMark.SetActive(false);
        }

        DialogueManager dialogue = DialogueManager.Instance;

        if (dialogue != null)
        {
            dialogue.ShowDialogue(
                "Phoebe",
                _worriedMessage,
                PhoebeExpression.Worried
            );

            yield return new WaitUntil(() => !dialogue.IsOpen);

            dialogue.ShowDialogue(
                "Phoebe",
                _shockedMessage,
                PhoebeExpression.Shocked
            );

            if (_activeCamera != null)
            {
                StartCameraZoom(
                    _originalCameraSize * _zoomMultiplier
                );
            }

            StartCoroutine(JumpInSurprise());
            StartCoroutine(JumpPortrait());

            yield return new WaitUntil(() => !dialogue.IsOpen);
        }

        _ivyGhost.Appear();

        yield return new WaitForSeconds(
            _ivyGhost.TransformationDuration
        );

        if (_activeCamera != null)
        {
            StartCameraZoom(_originalCameraSize);
        }

        if (_playerMovementScript != null)
        {
            _playerMovementScript.enabled = true;
        }
    }

    private void StartCameraZoom(float targetSize)
    {
        if (_activeCamera == null)
        {
            return;
        }

        if (_zoomCoroutine != null)
        {
            StopCoroutine(_zoomCoroutine);
        }

        _zoomCoroutine = StartCoroutine(
            ZoomCamera(targetSize)
        );
    }

    private IEnumerator ZoomCamera(float targetSize)
    {
        float startSize = _activeCamera.orthographicSize;
        float elapsed = 0f;
        float duration = Mathf.Max(_zoomDuration, 0.01f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            _activeCamera.orthographicSize =
                Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        _activeCamera.orthographicSize = targetSize;
        _zoomCoroutine = null;
    }

    private IEnumerator JumpInSurprise()
    {
        if (_playerRigidbody == null)
        {
            yield break;
        }

        Vector2 startPosition = _playerRigidbody.position;
        float elapsed = 0f;
        float duration = Mathf.Max(_jumpDuration, 0.01f);

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float height =
                Mathf.Sin(progress * Mathf.PI) * _jumpHeight;

            _playerRigidbody.MovePosition(
                startPosition + Vector2.up * height
            );

            yield return new WaitForFixedUpdate();
        }

        _playerRigidbody.MovePosition(startPosition);
    }

    private IEnumerator JumpPortrait()
    {
        if (_playerVisual == null || _isPortraitJumping)
        {
            yield break;
        }

        _isPortraitJumping = true;
        _portraitStartPosition = _playerVisual.localPosition;

        float elapsed = 0f;
        float duration = Mathf.Max(_jumpDuration, 0.01f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float height =
                Mathf.Sin(progress * Mathf.PI) *
                _portraitJumpHeight;

            _playerVisual.localPosition =
                _portraitStartPosition + Vector3.up * height;

            yield return null;
        }

        _playerVisual.localPosition = _portraitStartPosition;
        _isPortraitJumping = false;
    }

    private IEnumerator FadeEnvironment(
        float targetLight,
        float targetWeight)
    {
        float startLight = _lightToDim.intensity;
        float startWeight = _moonVolume.weight;

        float elapsed = 0f;
        float duration = Mathf.Max(_fadeDuration, 0.01f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            _lightToDim.intensity =
                Mathf.Lerp(startLight, targetLight, t);

            _moonVolume.weight =
                Mathf.Lerp(startWeight, targetWeight, t);

            yield return null;
        }

        _lightToDim.intensity = targetLight;
        _moonVolume.weight = targetWeight;
        _fadeCoroutine = null;
    }

    private void OnDisable()
    {
        if (_isPortraitJumping && _playerVisual != null)
        {
            _playerVisual.localPosition = _portraitStartPosition;
            _isPortraitJumping = false;
        }

        if (_activeCamera != null)
        {
            _activeCamera.orthographicSize = _originalCameraSize;
        }
    }
}
