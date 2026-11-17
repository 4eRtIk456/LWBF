
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PhoebeMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Lantern")]
    [SerializeField] private AnimationClip takeOutClip;
    [SerializeField] private AnimationClip takeInClip;
    [SerializeField] private Light2D lanternLight;
    [SerializeField] private float maxLightIntensity = 1.4f;
    [SerializeField] private float lightFadeDuration = 0.5f;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lanternAction;
    private Animator animator;

    private Vector2 movement;
    private Vector2 previousInput;
    private bool preferHorizontal = false;

    private string facingDirection = "Down";
    private string currentAnimation = "";

    private bool hasLantern = false;
    private bool isLanternAnimating = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        moveAction = playerInput.actions.FindAction("Move");
        lanternAction = playerInput.actions.FindAction("Lantern");

        if (moveAction == null || lanternAction == null)
        {
            Debug.LogError("Move or Lantern action is missing!", this);
            enabled = false;
            return;
        }

        if (lanternLight != null)
        {
            lanternLight.gameObject.SetActive(true);
            lanternLight.intensity = 0f;
        }
    }

    private void Update()
    {
        if (lanternAction.WasPressedThisFrame())
        {
            ToggleLantern();
        }

        if (isLanternAnimating)
        {
            movement = Vector2.zero;
            previousInput = Vector2.zero;
            return;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input.x != 0 && previousInput.x == 0)
        {
            preferHorizontal = true;
        }

        if (input.y != 0 && previousInput.y == 0)
        {
            preferHorizontal = false;
        }

        movement = input;

        if (input.x != 0 && input.y != 0)
        {
            if (preferHorizontal)
                movement.y = 0;
            else
                movement.x = 0;
        }

        movement = Vector2.ClampMagnitude(movement, 1f);
        previousInput = input;

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            movement * moveSpeed * Time.fixedDeltaTime
        );
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        if (movement.sqrMagnitude > 0.001f)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                facingDirection =
                    movement.x > 0 ? "Right" : "Left";
            }
            else
            {
                facingDirection =
                    movement.y > 0 ? "Up" : "Down";
            }
        }

        string prefix = hasLantern ? "Lantern_" : "";

        string state = movement.sqrMagnitude > 0.001f
            ? "Walk_"
            : "Idle_";

        PlayAnimation(prefix + state + facingDirection);
    }

    private void ToggleLantern()
    {
        if (isLanternAnimating)
            return;

        if (!hasLantern)
        {
            StartCoroutine(TakeOutLantern());
        }
        else
        {
            StartCoroutine(TakeInLantern());
        }
    }

    private IEnumerator TakeOutLantern()
    {
        isLanternAnimating = true;
        movement = Vector2.zero;
        previousInput = Vector2.zero;

        PlayAnimation("TakeOut_Down");

        float duration = takeOutClip != null
            ? takeOutClip.length
            : 0.5f;

        float fadeDuration = Mathf.Min(
            Mathf.Max(lightFadeDuration, 0f),
            duration
        );

        float waitTime = Mathf.Max(0f, duration - fadeDuration);

        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(
            FadeLight(maxLightIntensity, fadeDuration)
        );

        hasLantern = true;
        isLanternAnimating = false;
        currentAnimation = "";

        UpdateAnimation();
    }

    private IEnumerator TakeInLantern()
    {
        isLanternAnimating = true;
        movement = Vector2.zero;
        previousInput = Vector2.zero;

        PlayAnimation("TakeIn_Down");

        float duration = takeInClip != null
            ? takeInClip.length
            : 0.5f;

        float fadeDuration = Mathf.Min(
            Mathf.Max(lightFadeDuration, 0f),
            duration
        );

        yield return StartCoroutine(
            FadeLight(0f, fadeDuration)
        );

        float remainingTime = Mathf.Max(0f, duration - fadeDuration);

        if (remainingTime > 0f)
            yield return new WaitForSeconds(remainingTime);

        hasLantern = false;
        isLanternAnimating = false;
        currentAnimation = "";

        UpdateAnimation();
    }

    private IEnumerator FadeLight(float targetIntensity, float duration)
    {
        if (lanternLight == null)
        {
            if (duration > 0f)
                yield return new WaitForSeconds(duration);

            yield break;
        }

        float startIntensity = lanternLight.intensity;

        if (duration <= 0f)
        {
            lanternLight.intensity = targetIntensity;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            lanternLight.intensity = Mathf.Lerp(
                startIntensity,
                targetIntensity,
                t
            );

            yield return null;
        }

        lanternLight.intensity = targetIntensity;
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null)
            return;

        if (currentAnimation == animationName)
            return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }
}
