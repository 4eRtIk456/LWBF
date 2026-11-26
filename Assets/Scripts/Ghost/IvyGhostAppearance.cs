
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class IvyGhostAppearance : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private AnimationClip _transformationClip;
    [SerializeField] private string _animationStateName = "IvyTransformation";

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _hasAppeared;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _spriteRenderer.enabled = false;
        _animator.enabled = false;
    }

    public void Appear()
    {
        if (_hasAppeared || _transformationClip == null)
        {
            return;
        }

        _hasAppeared = true;

        _spriteRenderer.enabled = true;
        _animator.enabled = true;

        _animator.Play(_animationStateName, 0, 0f);

        StartCoroutine(HideAfterAnimation());
    }

    private IEnumerator HideAfterAnimation()
    {
        yield return new WaitForSeconds(_transformationClip.length);

        _animator.enabled = false;
        _spriteRenderer.enabled = false;
    }
}
