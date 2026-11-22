
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class ZoneExit : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _blackScreenDuration = 0.8f;

    [Header("Temporary Return")]
    [SerializeField] private Transform _returnPoint;

    private bool _isTransitioning;

    private void Awake()
    {
        if (_fadeCanvasGroup != null)
            _fadeCanvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTransitioning || _fadeCanvasGroup == null ||
            _returnPoint == null)
            return;

        PlayerInput player = other.GetComponentInParent<PlayerInput>();

        if (player == null)
            return;

        StartCoroutine(Transition(player));
    }


private IEnumerator Transition(PlayerInput player)
{
    _isTransitioning = true;

    PhoebeMovement movement = player.GetComponent<PhoebeMovement>();
    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

    // Allow Phoebe to keep walking while the screen fades out.
    yield return StartCoroutine(Fade(0f, 1f));

    // Stop Phoebe once the screen is fully black.
    if (movement != null)
        movement.enabled = false;

    player.DeactivateInput();

    if (rb != null)
        rb.linearVelocity = Vector2.zero;

    yield return new WaitForSeconds(_blackScreenDuration);

    if (rb != null)
        rb.position = new Vector2(
            _returnPoint.position.x,
            _returnPoint.position.y
        );
    else
        player.transform.position = _returnPoint.position;

    Physics2D.SyncTransforms();

    yield return StartCoroutine(Fade(1f, 0f));

    player.ActivateInput();

    if (movement != null)
        movement.enabled = true;

    _isTransitioning = false;
}


    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = _fadeDuration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsed / _fadeDuration);

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

