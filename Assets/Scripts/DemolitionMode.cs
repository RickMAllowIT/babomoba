using UnityEngine;

/// <summary>
/// BABOMOBA Slice 21: Demolition targeting filter.
/// Scans for enemy structures (towers and cores) within range via Physics.OverlapSphere.
/// Priority ordering: nearest tower ("Tower" tag) -> nearest core ("Core" tag) -> null.
/// Does NOT target minions or enemy dummies.
/// </summary>
public class DemolitionMode : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField, Tooltip("Maximum range to acquire structural targets.")] private float rangeRadius = 12f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugLine = true;
    [SerializeField] private Color debugLineColor = Color.magenta;

    /// <summary>
    /// The configurable range radius for structure targeting.
    /// </summary>
    public float RangeRadius => rangeRadius;

    /// <summary>
    /// Returns the nearest structure target (tower preferred over core), or null if none in range.
    /// Evaluated fresh each call — callers should not cache the result across frames.
    /// </summary>
    public Transform GetTarget()
    {
        return FindBestStructureTarget();
    }

    private void Update()
    {
        Transform target = GetTarget();

        if (drawDebugLine && target != null)
        {
            Debug.DrawLine(transform.position, target.position, debugLineColor);
        }
    }

    /// <summary>
    /// Scans all colliders within range and selects the best structural target.
    /// Priority: closest "Tower" -> closest "Core" -> null.
    /// This gives front-to-back priority (towers are typically closer than cores).
    /// </summary>
    private Transform FindBestStructureTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, rangeRadius);

        Transform bestTower = null;
        Transform bestCore = null;

        float closestTowerSq = float.MaxValue;
        float closestCoreSq = float.MaxValue;

        Vector3 origin = transform.position;

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.transform == transform)
                continue;

            string tag = hit.tag;
            Vector3 targetPos = hit.transform.position;
            float sqDist = (targetPos - origin).sqrMagnitude;

            if (tag == "Tower" && sqDist < closestTowerSq)
            {
                closestTowerSq = sqDist;
                bestTower = hit.transform;
            }
            else if (tag == "Core" && sqDist < closestCoreSq)
            {
                closestCoreSq = sqDist;
                bestCore = hit.transform;
            }
            // Intentionally ignores "Minion", "Enemy", and any other tags
        }

        // Priority: tower over core (front-to-back structure order)
        return bestTower != null ? bestTower : bestCore;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = debugLineColor;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }
}
