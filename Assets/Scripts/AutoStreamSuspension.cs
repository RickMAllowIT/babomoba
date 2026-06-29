using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// BABOMOBA Slice 10: Auto-stream suspension.
/// Monitors right-stick activity. When the right stick is moved past the deadzone
/// (manual aiming), sets AutoStream.IsSuspended = true so the auto-stream pauses.
/// When the right stick returns to center, sets IsSuspended = false so auto-stream resumes.
///
/// Only touches IsSuspended — never modifies IsActive (the toggle state).
/// If auto-stream was toggled OFF, manual aim does NOT turn it ON.
/// </summary>
[RequireComponent(typeof(PlayerAim))]
public class AutoStreamSuspension : MonoBehaviour
{
    [Header("Auto-Stream Reference")]
    [SerializeField] private AutoStream autoStream;

    [Header("Input")]
    [SerializeField] private InputActionReference aimAction;

    [Header("Thresholds")]
    [SerializeField] private float deadzone = 0.1f;

    private bool wasSuspended;

    private void OnEnable()
    {
        if (aimAction != null)
            aimAction.action.Enable();
    }

    private void OnDisable()
    {
        if (aimAction != null)
            aimAction.action.Disable();

        // On disable, always resume the auto-stream so it doesn't
        // stay suspended if this component is deactivated
        if (autoStream != null)
            autoStream.IsSuspended = false;
    }

    private void Start()
    {
        // Auto-find AutoStream on the same GameObject or a parent if not assigned
        if (autoStream == null)
            autoStream = GetComponentInParent<AutoStream>();

        if (autoStream == null)
            Debug.LogWarning("AutoStreamSuspension: No AutoStream component found. " +
                             "Suspension will be a no-op.", this);

        // Auto-find aim action from PlayerAim if not assigned
        if (aimAction == null)
        {
            var playerAim = GetComponent<PlayerAim>();
            if (playerAim != null)
            {
                // Use reflection or a serialized field approach — we just
                // try to find the aimAction via the component's own field
                var aimField = typeof(PlayerAim).GetField("aimAction",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);
                if (aimField != null)
                    aimAction = aimField.GetValue(playerAim) as InputActionReference;
            }
        }
    }

    private void Update()
    {
        if (autoStream == null)
            return;

        // Read right-stick input
        Vector2 aimInput = Vector2.zero;
        if (aimAction != null)
            aimInput = aimAction.action.ReadValue<Vector2>();

        bool stickActive = aimInput.magnitude > deadzone;

        // --- Suspension logic ---
        // When the right stick is active, suspend auto-stream.
        // When centered, resume.
        if (stickActive)
        {
            // Only suspend if not already suspended — avoids unnecessary setter calls
            if (!autoStream.IsSuspended)
            {
                autoStream.IsSuspended = true;
                wasSuspended = true;
            }
        }
        else
        {
            // Only resume if we were the ones who suspended it (via wasSuspended flag)
            // This prevents us from interfering with other suspension sources
            if (wasSuspended && autoStream.IsSuspended)
            {
                autoStream.IsSuspended = false;
                wasSuspended = false;
            }
        }
    }
}
