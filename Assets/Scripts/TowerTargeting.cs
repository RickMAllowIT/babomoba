using UnityEngine;

/// <summary>
/// Handles target acquisition for a BABOMOBA tower.
/// Scans for targets within a configurable radius using Physics.OverlapSphere.
/// Priority ordering: closest minion ("Minion" tag) -> closest enemy ("Enemy" tag) -> idle (null).
/// Draws a debug line from the tower to its current target.
/// </summary>
public class TowerTargeting : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float rangeRadius = 10f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugLine = true;
    [SerializeField] private Color debugLineColor = Color.yellow;

    private Transform currentTarget;

    /// <summary>
    /// Returns the current target Transform, or null if no valid target exists.
    /// Evaluated fresh each frame — callers should not cache the result across frames.
    /// </summary>
    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    /// <summary>
    /// The configurable range radius of this tower.
    /// </summary>
    public float RangeRadius => rangeRadius;

    private void Update()
    {
        currentTarget = FindBestTarget();

        if (drawDebugLine && currentTarget != null)
        {
            Debug.DrawLine(transform.position, currentTarget.position, debugLineColor);
        }
    }

    /// <summary>
    /// Scans for all colliders within range, then selects the best target
    /// using priority: closest "Minion" -> closest "Enemy" -> null.
    /// </summary>
    private Transform FindBestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, rangeRadius);

        Transform bestMinion = null;
        Transform bestEnemy = null;

        float closestMinionSq = float.MaxValue;
        float closestEnemySq = float.MaxValue;

        Vector3 towerPos = transform.position;

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.transform == transform)
                continue;

            string tag = hit.tag;
            Vector3 targetPos = hit.transform.position;
            float sqDist = (targetPos - towerPos).sqrMagnitude;

            if (tag == "Minion" && sqDist < closestMinionSq)
            {
                closestMinionSq = sqDist;
                bestMinion = hit.transform;
            }
            else if (tag == "Enemy" && sqDist < closestEnemySq)
            {
                closestEnemySq = sqDist;
                bestEnemy = hit.transform;
            }
        }

        // Priority: minion over enemy
        return bestMinion != null ? bestMinion : bestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }
}
