
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
    [SerializeField] private string _destinationSpawnPointName = "";

    private bool _isTransitioning;
    private static string _pendingSpawnPointName;

    private void Awake()
    {
        if (_fadeCanvasGroup != null)
        {
            _fadeCanvasGroup.alpha = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTransitioning || _fadeCanvasGroup == null)
        {
            return;
        }

        PlayerInput player =
            other.GetComponentInParent<PlayerInput>();

        if (player == null)
        {
            return;
        }

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
        {
            movement.enabled = false;
        }

        player.DeactivateInput();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        yield return StartCoroutine(Fade(0f, 1f));

        yield return new WaitForSeconds(_blackScreenDuration);

        _pendingSpawnPointName = _destinationSpawnPointName;

        SceneManager.sceneLoaded -= PlacePlayerAtSpawn;
        SceneManager.sceneLoaded += PlacePlayerAtSpawn;

        SceneManager.LoadScene(_destinationScene);
    }

    private static void PlacePlayerAtSpawn(
        Scene scene,
        LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= PlacePlayerAtSpawn;

        string spawnPointName = _pendingSpawnPointName;
        _pendingSpawnPointName = null;

        if (string.IsNullOrEmpty(spawnPointName))
        {
            return;
        }

        GameObject spawnPoint = GameObject.Find(spawnPointName);
        PlayerInput player = Object.FindFirstObjectByType<PlayerInput>();

        if (spawnPoint == null || player == null)
        {
            Debug.LogWarning(
                "ZoneExit: Player or spawn point not found."
            );
            return;
        }

        Vector3 targetPosition = spawnPoint.transform.position;

        player.transform.position = targetPosition;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.position = new Vector2(
                targetPosition.x,
                targetPosition.y
            );

            rb.linearVelocity = Vector2.zero;
        }
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
