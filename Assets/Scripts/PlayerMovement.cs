using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles twin-stick player movement via Unity Input System.
/// Reads left-stick input and applies physics-based movement on the XZ plane.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDeadzone = 0.1f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Constrain Y-axis rotation/physics to prevent tipping or Y drift
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        // Read raw input each frame (applied in FixedUpdate for physics)
        if (moveAction != null)
            moveInput = moveAction.action.ReadValue<Vector2>();
        else
            moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        // Apply deadzone manually (or rely on Input System's built-in deadzone processor)
        Vector2 direction = moveInput;
        if (direction.magnitude < moveDeadzone)
            direction = Vector2.zero;

        // Build a 3D velocity vector on the XZ plane
        Vector3 movement = new Vector3(direction.x, 0f, direction.y) * moveSpeed;

        // Move the rigidbody — preserve existing Y velocity to avoid Y-axis drift,
        // or overwrite entirely if we want full physics-based XZ control.
        // We use rb.velocity to keep it simple and lock Y to 0.
        rb.velocity = new Vector3(movement.x, 0f, movement.z);
    }
}
