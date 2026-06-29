using UnityEngine;

/// <summary>
/// Harvester targeting filter (Slice 9).
/// Replaces AutoStream's generic nearest-enemy targeting with a
/// minion-only filter that prefers the lowest-HP minion in range.
/// 
/// Does NOT target:
///   - Enemy dummies (tag "Enemy")
///   - Towers or core (not tagged "Minion")
/// 
/// Integration with AutoStream:
///   Assign this component to the same GameObject as AutoStream.
///   Set AutoStream.targetFilter in the inspector — AutoStream will
///   call GetTarget() instead of its default FindNearestTarget().
/// </summary>
public class HarvesterFilter : MonoBehaviour
{
    [Header("Filter Settings")]
    [SerializeField, Tooltip("Maximum range to acquire minion targets.")]
    private float rangeRadius = 15f;

    [Header("Debug")]
    [SerializeField, Tooltip("Draw the targeting range in the Scene view.")]
    private bool drawDebugGizmos = true;

    // Cached array reused each frame to reduce allocations
    private Collider[] overlapResults;
    private const int MaxResults = 64;

    /// <summary>The configured targeting range.</summary>
    public float RangeRadius => rangeRadius;

    private void Awake()
    {
        overlapResults = new Collider[MaxResults];
    }

    /// <summary>
    /// Returns the Transform of the lowest-HP live minion within range,
    /// or null if no valid minion is found.
    /// Only targets GameObjects tagged "Minion" that have a Health component.
    /// </summary>
    public Transform GetTarget()
    {
        Vector3 origin = transform.position;
        int hitCount = Physics.OverlapSphereNonAlloc(origin, rangeRadius, overlapResults);

        Transform bestTarget = null;
        float lowestHP = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = overlapResults[i];

            if (hit == null || hit.transform == transform)
                continue;

            // Strict minion-only filter — ignores enemies, towers, core
            if (!hit.CompareTag("Minion"))
                continue;

            // Must have a Health component to evaluate HP
            Health health = hit.GetComponent<Health>();
            if (health == null || health.IsDead)
                continue;

            float hp = health.CurrentHealth;

            // Prefer lowest current HP (switches immediately when a
            // lower-HP minion is found, per Slice 9 AC #4)
            if (hp < lowestHP)
            {
                lowestHP = hp;
                bestTarget = hit.transform;
            }
        }

        return bestTarget;
    }

    /// <summary>
    /// Returns true if the given target is still a valid harvester target
    /// (live minion within range). Useful for checking persistence
    /// between fire intervals.
    /// </summary>
    public bool IsTargetValid(Transform target)
    {
        if (target == null)
            return false;

        Health health = target.GetComponent<Health>();
        if (health == null || health.IsDead)
            return false;

        float sqrDist = (target.position - transform.position).sqrMagnitude;
        return sqrDist <= rangeRadius * rangeRadius;
    }

    private void OnDrawGizmosSelected()
    {
        if (drawDebugGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangeRadius);
        }
    }
}
