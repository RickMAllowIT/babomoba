using UnityEngine;

/// <summary>
/// Enemy Dummy — a static target dummy for MVP testing.
/// High health pool, tagged as "Enemy". Logs when HP reaches 0 but is not destroyed.
/// </summary>
public class EnemyDummy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 9999f;

    private float currentHealth;

    public float Health => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        gameObject.tag = "Enemy";
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply damage to this dummy. Logs when HP reaches zero.
    /// </summary>
    /// <param name="amount">Amount of damage to deal.</param>
    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Debug.Log($"{name} (EnemyDummy) has been defeated!");
        }
    }
}
