
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class ZoneExit : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _blackScreenDuration = 0.8f;

    [Header("Destination")]
    [SerializeField] private string _destinationScene = "Level1";

    private bool _isTransitioning;

    private void Awake()
    {
        if (_fadeCanvasGroup != null)
            _fadeCanvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTransitioning || _fadeCanvasGroup == null)
            return;

        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        StartCoroutine(Transition(player));
    }

    private IEnumerator Transition(PlayerInput player)
    {
        _isTransitioning = true;

        PhoebeMovement movement =
            player.GetComponent<PhoebeMovement>();

        Rigidbody2D rb =
            player.GetComponent<Rigidbody2D>();

        if (movement != null)
            movement.enabled = false;

        player.DeactivateInput();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return StartCoroutine(Fade(0f, 1f));

        yield return new WaitForSeconds(
            _blackScreenDuration
        );

        SceneManager.LoadScene(_destinationScene);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (_fadeDuration <= 0f)
        {
            _fadeCanvasGroup.alpha = endAlpha;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / _fadeDuration
            );

            _fadeCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                t
            );

            yield return null;
        }

        _fadeCanvasGroup.alpha = endAlpha;
    }
}
