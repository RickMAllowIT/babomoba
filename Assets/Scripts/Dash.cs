using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the Dash ability for BABOMOBA (Slice 11).
/// Reads L2 (Left Trigger) via Unity Input System.
/// On press, applies a velocity burst in the player's facing direction
/// for a short duration, then enters cooldown.
/// Dash distance is approximately one body length (~2 units for radius-1 sphere).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 2f;        // ~1 body length
    [SerializeField] private float dashDuration = 0.15f;     // burst window (seconds)
    [SerializeField] private float cooldownDuration = 4f;    // cooldown after dash

    [Header("Input")]
    [SerializeField] private InputActionReference dashAction; // L2 / Left Trigger

    private Rigidbody rb;
    private float cooldownRemaining;
    private float dashTimer;
    private bool isDashing;
    private Vector3 dashDirection;

    /// <summary>Whether the player is currently mid-dash. Useful for other scripts to check (e.g. prevent normal movement during dash).</summary>
    public bool IsDashing => isDashing;

    /// <summary>Remaining cooldown time in seconds. Zero means ready to dash.</summary>
    public float CooldownRemaining => cooldownRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (dashAction != null)
        {
            dashAction.action.Enable();
            dashAction.action.performed += OnDashPerformed;
        }
    }

    private void OnDisable()
    {
        if (dashAction != null)
        {
            dashAction.action.performed -= OnDashPerformed;
            dashAction.action.Disable();
        }
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        // Ignore if on cooldown or already dashing
        if (cooldownRemaining > 0f || isDashing)
            return;

        // Burst in the direction the player is currently facing
        dashDirection = transform.forward;
        isDashing = true;
        dashTimer = dashDuration;

        Debug.Log("[Dash] Dashing!");
    }

    private void FixedUpdate()
    {
        if (!isDashing)
            return;

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer > 0f)
        {
            // Override velocity to move in dash direction at calculated speed
            float dashSpeed = dashDistance / dashDuration;
            rb.velocity = new Vector3(
                dashDirection.x * dashSpeed,
                rb.velocity.y,  // preserve Y velocity (gravity / bounce)
                dashDirection.z * dashSpeed
            );
        }
        else
        {
            // End dash — stop horizontal velocity (keep Y for gravity)
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            isDashing = false;
            cooldownRemaining = cooldownDuration;

            Debug.Log($"[Dash] Dash ended. Cooldown {cooldownDuration:F1}s started.");
        }
    }

    private void Update()
    {
        // Tick cooldown timer
        if (cooldownRemaining <= 0f)
            return;

        cooldownRemaining -= Time.deltaTime;

        if (cooldownRemaining <= 0f)
        {
            cooldownRemaining = 0f;
            Debug.Log("[Dash] Cooldown complete — ready to dash!");
        }
        else
        {
            // Log remaining cooldown at whole-second boundaries to reduce spam
            float secondsRounded = Mathf.Ceil(cooldownRemaining);
            if (secondsRounded > 0f && Mathf.Approximately(cooldownRemaining % 1f, 0f))
            {
                Debug.Log($"[Dash] Cooldown: {secondsRounded:F0}s remaining");
            }
        }
    }
}
