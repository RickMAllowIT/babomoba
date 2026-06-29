using UnityEngine;

/// <summary>
/// Smoothly follows a target (the player) from a fixed offset above and behind.
/// Uses Vector3.SmoothDamp for smooth, lag-compensated movement.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -7f);

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Desired position: target position + offset in world space
        Vector3 targetPosition = target.position + offset;

        // Smoothly move the camera toward the target position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // Always look at the target
        transform.LookAt(target);
    }
}
