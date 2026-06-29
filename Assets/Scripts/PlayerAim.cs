using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles twin-stick player aiming via Unity Input System.
/// Reads right-stick input and rotates the player on the Y axis.
/// Draws a debug aim indicator in the Scene view.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAim : MonoBehaviour
{
    [Header("Aiming Settings")]
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float aimDeadzone = 0.1f;

    [Header("Input")]
    [SerializeField] private InputActionReference aimAction;

    [Header("Debug")]
    [SerializeField] private bool showAimIndicator = true;
    [SerializeField] private float indicatorLength = 2f;

    private Vector2 aimInput;
    private float targetAngle;

    private void OnEnable()
    {
        if (aimAction != null)
            aimAction.action.Enable();
    }

    private void OnDisable()
    {
        if (aimAction != null)
            aimAction.action.Disable();
    }

    private void Update()
    {
        // Read raw input each frame
        if (aimAction != null)
            aimInput = aimAction.action.ReadValue<Vector2>();
        else
            aimInput = Vector2.zero;

        // Apply deadzone — no rotation when stick is centered
        Vector2 direction = aimInput;
        if (direction.magnitude < aimDeadzone)
        {
            direction = Vector2.zero;
            // Still draw the last-known direction indicator if we had one
            // but don't rotate further.
        }

        if (direction != Vector2.zero)
        {
            // Convert stick input to an angle on the XZ plane
            // Atan2(x, y) gives angle from forward (0,0,1) direction
            targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        }

        // Smoothly rotate toward the target angle
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void LateUpdate()
    {
        if (!showAimIndicator)
            return;

        // Draw debug ray in Scene view showing current facing direction
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 forward = transform.forward;
        Debug.DrawRay(origin, forward * indicatorLength, Color.green);
    }
}
