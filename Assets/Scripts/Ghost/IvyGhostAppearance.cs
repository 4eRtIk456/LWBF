using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class IvyGhostAppearance : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private AnimationClip _transformationClip;
    [SerializeField] private string _animationStateName = "IvyTransformation";
    [SerializeField] private string _idleStateName = "IvyGhostIdle";

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _hasAppeared;
    private bool _isVisible;

    public float TransformationDuration =>
    _transformationClip != null
        ? _transformationClip.length
        : 0f;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _spriteRenderer.enabled = false;
        _animator.enabled = false;
    }

    public void ShowIdle()
    {
        if (_isVisible || _hasAppeared)
        {
            return;
        }

        _isVisible = true;

        _spriteRenderer.enabled = true;
        _animator.enabled = true;

        _animator.Play(_idleStateName, 0, 0f);
        _animator.Update(0f);
    }

    public void Appear()
    {
        if (_hasAppeared || _transformationClip == null)
        {
            return;
        }

        _hasAppeared = true;
        _isVisible = true;

        _spriteRenderer.enabled = true;
        _animator.enabled = true;

        _animator.Play(_animationStateName, 0, 0f);
        _animator.Update(0f);

        StartCoroutine(HideAfterAnimation());
    }

    private IEnumerator HideAfterAnimation()
    {
        yield return new WaitForSeconds(_transformationClip.length);

        _animator.enabled = false;
        _spriteRenderer.enabled = false;
        _isVisible = false;
    }
}
