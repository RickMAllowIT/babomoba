using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Renders a laser beam from the player's weapon position outward in the aim direction.
/// The beam is visible only while the right stick is active (magnitude > deadzone),
/// and hidden when the stick is centered (auto-stream-only firing).
/// Uses a LineRenderer with a distinct color separate from target highlight.
/// Slice 20: Laser beam.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    [Header("Beam Settings")]
    [SerializeField] private Transform beamSpawnPoint;
    [SerializeField] private float beamLength = 10f;
    [SerializeField] private float aimDeadzone = 0.1f;

    [Header("Input")]
    [SerializeField] private InputActionReference aimAction;

    [Header("Appearance")]
    [SerializeField] private Color beamColor = Color.cyan;
    [SerializeField] private Material beamMaterial;
    [SerializeField] private float beamWidth = 0.05f;

    private LineRenderer lineRenderer;
    private Vector2 aimInput;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Default spawn point to this transform if not assigned
        if (beamSpawnPoint == null)
            beamSpawnPoint = transform;

        // Configure LineRenderer for a single beam segment
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.startColor = beamColor;
        lineRenderer.endColor = beamColor;

        if (beamMaterial != null)
            lineRenderer.material = beamMaterial;

        // Start hidden — beam only appears while stick is active
        lineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        if (aimAction != null)
            aimAction.action.Enable();
    }

    private void OnDisable()
    {
        if (aimAction != null)
            aimAction.action.Disable();

        // Ensure beam is hidden when component is disabled
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    private void Update()
    {
        // Read right-stick input each frame (same pattern as PlayerAim.cs)
        if (aimAction != null)
            aimInput = aimAction.action.ReadValue<Vector2>();
        else
            aimInput = Vector2.zero;

        // Apply deadzone — hide beam when stick is centered or below threshold
        Vector2 direction = aimInput;
        if (direction.magnitude < aimDeadzone)
        {
            lineRenderer.enabled = false;
            return;
        }

        // Build a 3D direction on the XZ plane from the 2D stick input
        Vector3 aimDirection = new Vector3(direction.x, 0f, direction.y).normalized;

        // Set beam start (weapon spawn point) and end (aim direction * beam length)
        Vector3 startPos = beamSpawnPoint.position;
        Vector3 endPos = startPos + aimDirection * beamLength;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Show the beam while stick is active
        lineRenderer.enabled = true;
    }
}
