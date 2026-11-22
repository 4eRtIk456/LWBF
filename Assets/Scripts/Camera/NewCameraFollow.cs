
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class NewCameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Camera Limits")]
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void LateUpdate()
    {
        if (_target == null)
            return;

        Vector3 targetPosition = new Vector3(
            Mathf.Clamp(
                _target.position.x,
                _minPosition.x,
                _maxPosition.x
            ),
            Mathf.Clamp(
                _target.position.y,
                _minPosition.y,
                _maxPosition.y
            ),
            -10f
        );

        float t = 1f - Mathf.Exp(-_smoothSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            t
        );
    }
}
