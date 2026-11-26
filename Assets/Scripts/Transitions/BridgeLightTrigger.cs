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

        bool shouldBeDark = playerBody.position.x > _triggerCollider.bounds.center.x;

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

        float targetLight = _isDark ? _darkIntensity : _normalIntensity;
        float targetWeight = _isDark ? 1f : 0f;

        _fadeCoroutine = StartCoroutine(FadeEnvironment(targetLight, targetWeight));
    }

    private IEnumerator PlayGhostEvent()
    {
        if (_playerMovementScript != null)
        {
            _playerMovementScript.enabled = false;
        }

        if (_playerRigidbody != null)
        {
            _playerRigidbody.linearVelocity = Vector2.zero;
        }

        if (_questionMark != null)
        {
            _questionMark.SetActive(true);
        }

        _ivyGhost.Appear();

        yield return new WaitForSeconds(_freezeDuration);

        if (_questionMark != null)
        {
            _questionMark.SetActive(false);
        }

        if (_playerMovementScript != null)
        {
            _playerMovementScript.enabled = true;
        }
    }

    private IEnumerator FadeEnvironment(float targetLight, float targetWeight)
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

            _lightToDim.intensity = Mathf.Lerp(startLight, targetLight, t);
            _moonVolume.weight = Mathf.Lerp(startWeight, targetWeight, t);

            yield return null;
        }

        _lightToDim.intensity = targetLight;
        _moonVolume.weight = targetWeight;
        _fadeCoroutine = null;
    }
}