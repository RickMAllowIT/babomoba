using UnityEngine;

[RequireComponent(typeof(Health))]
public class DummyPatrol : MonoBehaviour
{
    [Header("Patrol Waypoints")]
    [SerializeField] private Transform waypointA;
    [SerializeField] private Transform waypointB;

    [Header("Settings")]
    [SerializeField] private float patrolSpeed = 1.5f;

    private Vector3 targetPosition;
    private bool movingToB = true;

    private void Start()
    {
        if (waypointA == null || waypointB == null)
        {
            Debug.LogError($"{name}: DummyPatrol requires both waypointA and waypointB to be assigned.", this);
            enabled = false;
            return;
        }

        // Start by moving toward waypointB
        targetPosition = waypointB.position;
    }

    private void Update()
    {
        if (waypointA == null || waypointB == null)
            return;

        // Move toward the current target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            patrolSpeed * Time.deltaTime
        );

        // Check if we've reached the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Ping-pong: swap target
            if (movingToB)
            {
                targetPosition = waypointA.position;
                movingToB = false;
            }
            else
            {
                targetPosition = waypointB.position;
                movingToB = true;
            }
        }
    }

    /// <summary>
    /// Visualize waypoints in the Scene view for easy setup.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (waypointA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(waypointA.position, 0.3f);
            Gizmos.DrawLine(transform.position, waypointA.position);
        }

        if (waypointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(waypointB.position, 0.3f);
            Gizmos.DrawLine(transform.position, waypointB.position);
        }
    }
}
