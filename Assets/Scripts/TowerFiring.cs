using UnityEngine;

/// <summary>
/// Handles tower firing for BABOMOBA (Slice 13).
/// Fires a projectile at the current target on a configurable interval.
/// References TowerTargeting on the same GameObject for target acquisition.
/// The projectile travels toward the target's position at fire time (non-homing).
/// Stops firing when no target is available (idle).
/// </summary>
[RequireComponent(typeof(TowerTargeting))]
public class TowerFiring : MonoBehaviour
{
    [Header("Firing")]
    [SerializeField] private float fireRate = 1.5f;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    private TowerTargeting targeting;
    private float fireCooldownTimer;

    private void Awake()
    {
        targeting = GetComponent<TowerTargeting>();
    }

    private void Update()
    {
        fireCooldownTimer -= Time.deltaTime;

        Transform target = targeting.GetCurrentTarget();

        // Idle — no target to fire at
        if (target == null)
        {
            return;
        }

        if (fireCooldownTimer <= 0f)
        {
            Fire(target);
            fireCooldownTimer = fireRate;
        }
    }

    /// <summary>
    /// Spawns a projectile at the tower position, aimed at the target's
    /// position at the moment of fire. The projectile is non-homing and
    /// will continue in a straight line.
    /// </summary>
    private void Fire(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[TowerFiring] No projectilePrefab assigned.", this);
            return;
        }

        // Instantiate at tower position with default rotation
        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Direction from tower to target at time of fire (non-homing)
        Vector3 direction = (target.position - transform.position).normalized;

        // Orient the projectile so its forward axis points at the target.
        // The Projectile component reads transform.forward in Start() for velocity.
        projectileObj.transform.forward = direction;

        // Set owner so the projectile can ignore collision with the tower
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetOwner(gameObject);
        }
    }
}
