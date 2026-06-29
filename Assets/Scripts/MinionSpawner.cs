using UnityEngine;

/// <summary>
/// Spawns minions on a timer at the far end of the lane.
/// Each minion is assigned a destination (e.g. the player's core).
/// </summary>
public class MinionSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private float spawnInterval = 30f;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;

    [Header("Destination")]
    [SerializeField] private Transform destinationTransform;
    [SerializeField] private Vector3 destinationPosition = Vector3.zero;

    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnMinion();
            timer = spawnInterval;
        }
    }

    private void SpawnMinion()
    {
        if (minionPrefab == null)
        {
            Debug.LogWarning("MinionSpawner: No minionPrefab assigned.", this);
            return;
        }

        GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);

        // Determine destination: prefer Transform reference, fall back to raw Vector3
        Vector3 dest = destinationTransform != null
            ? destinationTransform.position
            : destinationPosition;

        Minion minionScript = minion.GetComponent<Minion>();
        if (minionScript != null)
        {
            minionScript.SetDestination(dest);
        }
        else
        {
            Debug.LogWarning("MinionSpawner: Spawned minion has no Minion component.", this);
        }
    }

    /// <summary>
    /// Draw a gizmo for the spawn position and destination in the Scene view.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Spawn position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);

        // Destination
        Vector3 dest = destinationTransform != null
            ? destinationTransform.position
            : destinationPosition;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(dest, 0.5f);
        Gizmos.DrawLine(spawnPosition, dest);
    }
}
