using UnityEngine;

/// <summary>
/// Applies a visual highlight (ring/glow indicator) on the entity the auto-stream
/// is currently locked onto. The highlight is visually distinct from the laser beam
/// (different color, ring/glow style instead of a line).
/// Slice 19: Target highlight.
/// 
/// Usage:
///   - Attach to the same GameObject as AutoStream (the player).
///   - Assign a ring/glow prefab in the inspector (child ring GameObject approach).
///   - If no ring prefab is assigned, falls back to swapping the target's material
///     to an emissive highlight material.
/// 
/// Works on any entity tagged "Enemy" or "Minion" — future-proof for structures
/// tagged with "Enemy".
/// </summary>
[RequireComponent(typeof(AutoStream))]
public class TargetHighlight : MonoBehaviour
{
    [Header("Highlight Style")]
    [SerializeField, Tooltip("Ring/glow prefab instantiated as a child of the target. "
        + "Moves with the target automatically via parent hierarchy. "
        + "If unassigned, falls back to material-swap highlight.")]
    private GameObject ringPrefab;

    [Header("Fallback — Material Swap")]
    [SerializeField, Tooltip("Emissive material applied to the target's Renderer when "
        + "no ringPrefab is assigned. Use a bright, distinct color (yellow/gold).")]
    private Material highlightMaterial;
    [SerializeField, Tooltip("Debug gizmo colour for the targeting radius.")]
    private Color debugGizmoColor = new Color(1f, 0.84f, 0f, 0.5f); // gold

    [Header("Targeting")]
    [SerializeField, Tooltip("Override range radius. If 0, reads from AutoStream.")]
    private float rangeRadiusOverride = 0f;

    private AutoStream autoStream;
    private Transform currentTarget;
    private GameObject activeRing;

    // Material-swap state
    private Renderer targetRenderer;
    private Material originalMaterial;

    private float EffectiveRangeRadius =>
        rangeRadiusOverride > 0f ? rangeRadiusOverride
            : autoStream != null ? autoStream.RangeRadius
            : 15f;

    /// <summary>The transform currently being highlighted, or null.</summary>
    public Transform CurrentHighlightedTarget => currentTarget;

    private void Awake()
    {
        autoStream = GetComponent<AutoStream>();
    }

    private void Update()
    {
        // Highlight only when auto-stream is active and not suspended
        if (autoStream == null || !autoStream.IsActive || autoStream.IsSuspended)
        {
            ClearHighlight();
            return;
        }

        Transform newTarget = FindNearestTarget();

        // If target changed (including from valid to null), swap highlight
        if (newTarget != currentTarget)
        {
            ClearHighlight();
            currentTarget = newTarget;
            ApplyHighlight();
        }
    }

    /// <summary>
    /// Finds the nearest target within range using Physics.OverlapSphere,
    /// matching the same tag filter as AutoStream.
    /// </summary>
    private Transform FindNearestTarget()
    {
        float radius = EffectiveRangeRadius;
        Vector3 origin = transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, radius);

        Transform nearest = null;
        float nearestSqDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.transform == transform)
                continue;

            if (!HasTargetTag(hit.tag))
                continue;

            float sqDist = (hit.transform.position - origin).sqrMagnitude;
            if (sqDist < nearestSqDist)
            {
                nearestSqDist = sqDist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    /// <summary>Returns true if the given tag matches a valid target.</summary>
    private bool HasTargetTag(string tag)
    {
        // Match AutoStream's default target tags
        foreach (string t in _targetTags)
        {
            if (t == tag)
                return true;
        }
        return false;
    }

    private static readonly string[] _targetTags = { "Enemy", "Minion" };

    /// <summary>
    /// Applies the highlight to the current target.
    /// Priority: ring prefab (instantiated as child) > material swap.
    /// </summary>
    private void ApplyHighlight()
    {
        if (currentTarget == null)
            return;

        // --- Ring prefab approach (recommended) ---
        if (ringPrefab != null)
        {
            activeRing = Instantiate(ringPrefab, currentTarget.position,
                Quaternion.identity, currentTarget);
            return;
        }

        // --- Fallback: material-swap approach ---
        if (highlightMaterial == null)
        {
            Debug.LogWarning("[TargetHighlight] No ringPrefab or highlightMaterial assigned. "
                + "Highlight will not appear.", this);
            return;
        }

        Renderer renderer = currentTarget.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            renderer.material = highlightMaterial;
            targetRenderer = renderer;
        }
        else
        {
            Debug.LogWarning($"[TargetHighlight] Target '{currentTarget.name}' has no Renderer "
                + "for material-swap highlight.", currentTarget);
        }
    }

    /// <summary>Removes the highlight from the previous target.</summary>
    private void ClearHighlight()
    {
        // Destroy ring if one exists
        if (activeRing != null)
        {
            // Safe to call even if the target (and thus the ring) was already destroyed
            Destroy(activeRing);
            activeRing = null;
        }

        // Restore original material if swapped
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
            targetRenderer = null;
            originalMaterial = null;
        }

        currentTarget = null;
    }

    private void OnDestroy()
    {
        // Ensure cleanup when this component (or its GameObject) is destroyed
        ClearHighlight();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = debugGizmoColor;
        Gizmos.DrawWireSphere(transform.position, EffectiveRangeRadius);
    }
}
