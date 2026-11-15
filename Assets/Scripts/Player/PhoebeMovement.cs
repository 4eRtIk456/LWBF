using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PhoebeMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Animator animator;

    private Vector2 movement;
    private string facingDirection = "Down";
    private string currentAnimation = "";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        moveAction = playerInput.actions.FindAction("Move");
    }

    private void Update()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            movement.y = 0f;
        }
        else if (movement.y != 0f)
        {
            movement.x = 0f;
        }

        movement = Vector2.ClampMagnitude(movement, 1f);

        if (animator != null)
        {
            UpdateAnimation();
        }
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

            PlayAnimation("Walk_" + facingDirection);
        }
        else
        {
            PlayAnimation("Idle_" + facingDirection);
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName)
            return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }

}