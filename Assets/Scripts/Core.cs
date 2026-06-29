using UnityEngine;

/// <summary>
/// Represents the player's Core (base) in BABOMOBA — the large cube at the
/// player's end of the lane.  Minions re-path here after their lane tower
/// is destroyed.
///
/// Exposes a static <see cref="CorePosition"/> so Minion.cs (or any other
/// script) can find the core without a direct reference.  Listens for the
/// tower-destroyed broadcast via a public method that designers wire in the
/// Inspector.
///
/// Applies damage-over-time to the core while minions are in contact and
/// visualises remaining HP by lerping the material colour from green (full)
/// to red (nearly dead).
/// </summary>
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Renderer))]
public class Core : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private string minionTag = "Minion";

    [Header("HP Bar Colour")]
    [SerializeField] private Color fullHpColor = Color.green;
    [SerializeField] private Color lowHpColor = Color.red;

    // -----------------------------------------------------------------------
    //  Static reference — minions read this to re-path after tower destroyed
    // -----------------------------------------------------------------------
    /// <summary>
    /// World-space position of this core.  Minions should path to this
    /// position after their lane tower is destroyed.
    /// </summary>
    public static Vector3 CorePosition { get; private set; }

    // -----------------------------------------------------------------------
    //  Component references
    // -----------------------------------------------------------------------
    private Health _health;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    // -----------------------------------------------------------------------
    //  Lifecycle
    // -----------------------------------------------------------------------
    private void Awake()
    {
        _health   = GetComponent<Health>();
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        // Expose position so minions can find us at any time
        CorePosition = transform.position;
        ApplyColor();
    }

    private void Update()
    {
        ApplyColor();
    }

    // -----------------------------------------------------------------------
    //  Tower-destruction hook
    // -----------------------------------------------------------------------
    /// <summary>
    /// Public method that designers can wire to
    /// <see cref="TowerDestruction.OnTowerDestroyed"/> in the Unity Inspector.
    /// Re-asserts <see cref="CorePosition"/> so Minion.cs can pick it up
    /// on the frame the tower is destroyed.
    /// </summary>
    public void OnTowerDestroyed()
    {
        CorePosition = transform.position;
        Debug.Log("[Core] Tower destroyed — core position updated for minion re-pathing.");
    }

    // -----------------------------------------------------------------------
    //  Damage from minion contact (OnCollisionStay = damage over time)
    // -----------------------------------------------------------------------
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag(minionTag))
            return;

        _health.TakeDamage(damagePerSecond * Time.deltaTime);
    }

    // -----------------------------------------------------------------------
    //  HP bar visualisation — colour lerp on the material
    // -----------------------------------------------------------------------
    private void ApplyColor()
    {
        if (_renderer == null || _health == null || _health.MaxHealth <= 0f)
            return;

        float t = _health.CurrentHealth / _health.MaxHealth;   // 1 = full, 0 = dead
        Color colour = Color.Lerp(lowHpColor, fullHpColor, t);

        // Use MaterialPropertyBlock so we don't create material instances
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", colour);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
