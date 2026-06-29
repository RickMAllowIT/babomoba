using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls a single minion's NavMesh movement.
/// Minion paths toward its assigned destination and stops when it arrives.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Minion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float stoppingDistance = 0.5f;

    private NavMeshAgent agent;
    private Vector3 destination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Set the destination this minion should path toward.
    /// Call this after instantiation (e.g. from the spawner).
    /// </summary>
    public void SetDestination(Vector3 targetPosition)
    {
        destination = targetPosition;
        agent.SetDestination(destination);
    }

    private void Update()
    {
        // When the minion arrives at its destination, stop it.
        if (!agent.pathPending
            && agent.remainingDistance <= agent.stoppingDistance
            && agent.hasPath)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
}
