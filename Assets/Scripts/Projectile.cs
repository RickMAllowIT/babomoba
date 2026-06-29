using UnityEngine;

/// <summary>
/// Simple projectile for BABOMOBA.
/// Moves forward on spawn, destroys itself on collision or after a max lifetime.
/// Applies damage to any entity with a Health component.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private bool useRigidbodyVelocity = true;

    [Header("Lifetime")]
    [SerializeField] private float maxLifetime = 3f;

    [Header("Damage")]
    [SerializeField] private float damageAmount = 25f;

    [Header("Collision")]
    [SerializeField] private LayerMask hitLayers = ~0; // Everything by default

    private Rigidbody rb;
    private float spawnTime;
    private GameObject owner; // The GameObject that fired this projectile

    public float Speed => speed;
    public float DamageAmount => damageAmount;
    public float MaxLifetime => maxLifetime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnTime = Time.time;

        // If using Rigidbody velocity, we can set it in Start/OnEnable
        // If using transform.Translate, we handle it in FixedUpdate
    }

    private void Start()
    {
        if (useRigidbodyVelocity && rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    private void FixedUpdate()
    {
        // Self-destruct if past lifetime
        if (Time.time > spawnTime + maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // If not using Rigidbody velocity, move via transform.Translate
        if (!useRigidbodyVelocity)
        {
            Vector3 movement = transform.forward * (speed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + movement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore self or owner
        if (owner != null && (other.gameObject == owner || other.transform.IsChildOf(owner.transform)))
            return;

        // Check if the collider belongs to a valid hit layer
        if ((hitLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // Apply damage to anything with a Health component
        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
        }

        // Destroy the projectile on impact
        Destroy(gameObject);
    }

    /// <summary>
    /// Set the owning GameObject so the projectile can ignore collision with the shooter.
    /// </summary>
    public void SetOwner(GameObject ownerObject)
    {
        owner = ownerObject;
    }
}
