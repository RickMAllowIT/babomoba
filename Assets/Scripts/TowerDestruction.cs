using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles tower destruction logic for BABOMOBA.
/// Listens to the Health component's onDeath event, disables the tower,
/// and broadcasts an event so minions can re-path toward the core.
/// </summary>
[RequireComponent(typeof(Health))]
public class TowerDestruction : MonoBehaviour
{
    [Header("Events")]
    /// <summary>
    /// Fired when the tower is destroyed. Minions listen to this to re-path toward the core.
    /// </summary>
    public UnityEvent OnTowerDestroyed = new UnityEvent();

    [Header("Components (auto-assigned if left empty)")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Collider _collider;
    [SerializeField] private MonoBehaviour _targeting;
    [SerializeField] private MonoBehaviour _firing;

    private Health _health;
    private bool _isDestroyed = false;

    private void Awake()
    {
        _health = GetComponent<Health>();

        // Auto-find components if not manually assigned
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_collider == null) _collider = GetComponent<Collider>();

        // Attempt to find TowerTargeting and TowerFiring if present on this GameObject
        if (_targeting == null)
        {
            // Use component name search for loose coupling — these scripts may not exist yet
            System.Type targetingType = System.Type.GetType("TowerTargeting");
            if (targetingType != null)
                _targeting = GetComponent(targetingType) as MonoBehaviour;
        }

        if (_firing == null)
        {
            System.Type firingType = System.Type.GetType("TowerFiring");
            if (firingType != null)
                _firing = GetComponent(firingType) as MonoBehaviour;
        }
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.onDeath.AddListener(OnTowerDeath);
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.onDeath.RemoveListener(OnTowerDeath);
    }

    private void OnTowerDeath()
    {
        // Guard against double-destruction
        if (_isDestroyed) return;
        _isDestroyed = true;

        // Disable visual
        if (_renderer != null)
            _renderer.enabled = false;

        // Disable collision so projectiles/minions pass through
        if (_collider != null)
            _collider.enabled = false;

        // Disable combat systems
        if (_targeting != null)
            _targeting.enabled = false;

        if (_firing != null)
            _firing.enabled = false;

        // Notify listeners (e.g., minions) to re-path toward the core
        OnTowerDestroyed.Invoke();

        // Optionally, the tower could be deactivated entirely after a brief delay
        // to let any VFX play. If no VFX, you can also simply do:
        // gameObject.SetActive(false);
    }
}
