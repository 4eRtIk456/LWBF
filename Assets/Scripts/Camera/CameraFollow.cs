
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [FormerlySerializedAs("target")]
    [SerializeField] private Transform _target;

    [FormerlySerializedAs("smoothSpeed")]
    [SerializeField] private float _smoothSpeed = 3f;

    [Header("Camera Bounds")]
    [SerializeField] private TilemapRenderer _groundRenderer;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (_target == null || _groundRenderer == null)
            return;

        Bounds bounds = _groundRenderer.bounds;

        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;

        float x = _target.position.x;
        float y = _target.position.y;

        if (bounds.size.x > halfWidth * 2f)
            x = Mathf.Clamp(
                x,
                bounds.min.x + halfWidth,
                bounds.max.x - halfWidth
            );
        else
            x = bounds.center.x;

        if (bounds.size.y > halfHeight * 2f)
            y = Mathf.Clamp(
                y,
                bounds.min.y + halfHeight,
                bounds.max.y - halfHeight
            );
        else
            y = bounds.center.y;

        Vector3 targetPosition = new Vector3(x, y, -10f);

        float t = 1f - Mathf.Exp(-_smoothSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            t
        );
    }
}
