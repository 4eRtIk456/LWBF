
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        float smoothing = 1f - Mathf.Exp(
            -smoothSpeed * Time.deltaTime
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothing
        );
    }
}
