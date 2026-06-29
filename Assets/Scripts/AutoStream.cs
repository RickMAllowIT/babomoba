using UnityEngine;

/// <summary>
/// Auto-stream: Timer-driven projectile emitter on the player (Slice 7).
/// Fires at the nearest enemy in range on a configurable interval.
/// Targets any entity tagged "Enemy" or "Minion".
/// Respects IsActive (toggled by Slice 8) and IsSuspended (manual aim, Slice 10).
/// </summary>
public class AutoStream : MonoBehaviour
{
    [Header("Firing Settings")]
    [SerializeField, Tooltip("Time between automatic shots (seconds).")] private float fireInterval = 0.5f;
    [SerializeField, Tooltip("Maximum range to acquire targets.")] private float rangeRadius = 15f;

    [Header("Projectile")]
    [SerializeField, Tooltip("Prefab to instantiate on each shot.")] private GameObject projectilePrefab;
    [SerializeField, Tooltip("Transform from which projectiles are spawned (e.g. muzzle).")] private Transform firePoint;

    [Header("Tags")]
    [SerializeField, Tooltip("Tags the auto-stream will target.")] private string[] targetTags = new[] { "Enemy", "Minion" };

    [Header("Targeting Filter")]
    [SerializeField, Tooltip("Optional filter (Slice 9 HarvesterFilter). When assigned, overrides default nearest-enemy targeting.")]
    private HarvesterFilter targetFilter;

    [Header("Debug")]
    [SerializeField] private bool drawDebugLine = true;
    [SerializeField] private Color debugLineColor = Color.red;

    private float fireTimer;

    /// <summary>Whether auto-stream is actively firing. Set false to disable (Slice 8 toggle).</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Set true to suspend auto-stream while manually aiming (Slice 10).</summary>
    public bool IsSuspended { get; set; } = false;

    /// <summary>The configured fire interval in seconds.</summary>
    public float FireInterval => fireInterval;

    /// <summary>The configured targeting range.</summary>
    public float RangeRadius => rangeRadius;

    private void Update()
    {
        // Do nothing if disabled or suspended
        if (!IsActive || IsSuspended)
        {
            fireTimer = 0f;
            return;
        }

        // Tick the fire timer
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            Transform target = FindNearestTarget();

            if (target != null)
            {
                FireAtTarget(target);

                if (drawDebugLine)
                {
                    Debug.DrawLine(firePoint != null ? firePoint.position : transform.position,
                                   target.position, debugLineColor, 0.3f);
                }
            }

            // Reset timer (don't subtract to avoid drift; always fire on interval)
            fireTimer = 0f;
        }
    }

    /// <summary>
    /// Finds a valid target within range.
    /// If a HarvesterFilter is assigned (Slice 9), delegates to its
    /// minion-only lowest-HP targeting. Otherwise falls back to the
    /// default nearest-enemy logic.
    /// </summary>
    private Transform FindNearestTarget()
    {
        // Use the harvester filter if configured (Slice 9 integration)
        if (targetFilter != null)
        {
            Transform filtered = targetFilter.GetTarget();
            if (filtered != null)
                return filtered;
            // If the filter found nothing but we still want
            // fallback targeting, remove this guard. Currently
            // we respect the filter's verdict — no minions = no shot.
            return null;
        }
        Vector3 origin = transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, rangeRadius);

        Transform nearest = null;
        float nearestSqDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.transform == transform)
                continue;

            // Check if the hit object has a target tag
            if (!HasTargetTag(hit.tag))
                continue;

            Vector3 toTarget = hit.transform.position - origin;
            float sqDist = toTarget.sqrMagnitude;

            // sqrMagnitude is already within range since OverlapSphere filtered it,
            // but we also use it for nearest-distance comparison
            if (sqDist < nearestSqDist)
            {
                nearestSqDist = sqDist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    /// <summary>Returns true if the given tag is in the targetTags array.</summary>
    private bool HasTargetTag(string tag)
    {
        foreach (string t in targetTags)
        {
            if (t == tag)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Fires a projectile toward the given target.
    /// Instantiates the projectile prefab at the fire point (or this transform),
    /// then orients it toward the target.
    /// </summary>
    private void FireAtTarget(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[AutoStream] No projectilePrefab assigned — cannot fire.");
            return;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Quaternion spawnRot = firePoint != null ? firePoint.rotation : transform.rotation;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, spawnRot);

        // Orient the projectile toward the target
        Vector3 direction = (target.position - spawnPos).normalized;
        projectile.transform.forward = direction;

        Debug.Log($"[AutoStream] Fired projectile at {target.name} (distance: {Vector3.Distance(spawnPos, target.position):F1})");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the auto-stream range in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }
}
