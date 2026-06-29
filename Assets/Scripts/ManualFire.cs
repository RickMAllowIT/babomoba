using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles manual fire input (R2 trigger) for BABOMOBA.
/// Reads the R2 float action and aim direction from Input System,
/// spawns a projectile prefab at the player's position pointing in the aim direction,
/// with a fire rate cooldown to prevent spamming.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ManualFire : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Input")]
    [SerializeField] private InputActionReference fireAction;        // R2 trigger (float 0-1)
    [SerializeField] private InputActionReference aimAction;         // Right stick (Vector2)

    [Header("Fire Rate")]
    [SerializeField] private float fireCooldown = 0.3f;             // Seconds between shots
    [SerializeField] private float fireThreshold = 0.5f;             // R2 must exceed this value

    private float lastFireTime = -Mathf.Infinity;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Default spawn point to this transform if not assigned
        if (projectileSpawnPoint == null)
            projectileSpawnPoint = transform;
    }

    private void OnEnable()
    {
        if (fireAction != null)
            fireAction.action.Enable();
        if (aimAction != null)
            aimAction.action.Enable();
    }

    private void OnDisable()
    {
        if (fireAction != null)
            fireAction.action.Disable();
        if (aimAction != null)
            aimAction.action.Disable();
    }

    private void Update()
    {
        if (projectilePrefab == null)
            return;

        // Read R2 float value
        float fireValue = fireAction != null
            ? fireAction.action.ReadValue<float>()
            : 0f;

        if (fireValue > fireThreshold && Time.time >= lastFireTime + fireCooldown)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    /// <summary>
    /// Spawn a projectile in the current aim direction.
    /// </summary>
    private void Fire()
    {
        // Determine aim direction from right stick input
        Vector2 aimInput = aimAction != null
            ? aimAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        // Build a 3D direction on the XZ plane
        Vector3 aimDirection = new Vector3(aimInput.x, 0f, aimInput.y).normalized;

        // If there's no meaningful input, fire in the direction the player is facing
        if (aimDirection.magnitude < 0.01f)
        {
            aimDirection = transform.forward;
        }

        // Spawn projectile at the spawn point
        GameObject projectileObj = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.LookRotation(aimDirection)
        );

        // Ignore collision with the shooter so the player doesn't self-hit
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetOwner(gameObject);
        }

        // Also prevent physics collision between shooter and projectile
        Collider projectileCollider = projectileObj.GetComponent<Collider>();
        Collider ownerCollider = GetComponent<Collider>();
        if (projectileCollider != null && ownerCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, ownerCollider, true);
        }
    }
}
