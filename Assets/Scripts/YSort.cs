using UnityEngine;
using UnityEngine.Rendering;

public class YSort : MonoBehaviour
{
    [SerializeField] private Transform _sortingPoint;

    private SortingGroup _sortingGroup;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _sortingGroup = GetComponent<SortingGroup>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        float y = _sortingPoint != null
            ? _sortingPoint.position.y
            : transform.position.y;

        int order = Mathf.RoundToInt(-y * 100f);

        if (_sortingGroup != null)
            _sortingGroup.sortingOrder = order;
        else if (_spriteRenderer != null)
            _spriteRenderer.sortingOrder = order;
    }
}