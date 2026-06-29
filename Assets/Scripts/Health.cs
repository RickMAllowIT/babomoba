using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Reusable health component for BABOMOBA entities.
/// Attach to any GameObject that needs hit points.
/// On death, invokes onDeath UnityEvent then destroys the GameObject.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Events")]
    public UnityEvent onDeath;

    private float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply damage to this entity.
    /// </summary>
    /// <param name="amount">Raw damage amount (must be non-negative).</param>
    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (IsDead)
        {
            onDeath.Invoke();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Restore health (clamped to maxHealth).
    /// </summary>
    public void Heal(float amount)
    {
        if (IsDead)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    /// <summary>
    /// Reset health to full. Useful when pooling objects.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Set a new max health (optionally scaling current health to preserve percentage).
    /// </summary>
    public void SetMaxHealth(float newMax, bool preservePercentage = false)
    {
        float previousPercentage = currentHealth / maxHealth;
        maxHealth = Mathf.Max(1f, newMax);
        currentHealth = preservePercentage
            ? maxHealth * previousPercentage
            : maxHealth;
    }
}
