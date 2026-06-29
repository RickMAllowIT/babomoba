using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// BABOMOBA Slice 16: Game over state.
/// Listens for the core's Health.onDeath event. On core death:
/// - Freezes time (Time.timeScale = 0)
/// - Displays "Game Over" text on screen
/// - Waits for player to press R (or Restart key) to reload the active scene
/// </summary>
public class GameOver : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Text gameOverText;

    [Header("Settings")]
    [SerializeField] private string coreTag = "Core";
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    [Header("Game Over Message")]
    [SerializeField] private string message = "GAME OVER";

    private Health _coreHealth;
    private bool _isGameOver = false;
    private bool _hasSubscribed = false;

    private void Awake()
    {
        // Start with the game over UI hidden
        if (gameOverCanvas != null)
            gameOverCanvas.enabled = false;

        if (gameOverText != null)
            gameOverText.text = message;
    }

    private void Start()
    {
        // Find the core and subscribe to its death event
        GameObject core = GameObject.FindGameObjectWithTag(coreTag);
        if (core != null)
        {
            _coreHealth = core.GetComponent<Health>();
            if (_coreHealth != null)
            {
                _coreHealth.onDeath.AddListener(OnCoreDeath);
                _hasSubscribed = true;
            }
            else
            {
                Debug.LogError("[GameOver] Core GameObject has no Health component.");
            }
        }
        else
        {
            Debug.LogError($"[GameOver] No GameObject found with tag \"{coreTag}\".");
        }
    }

    private void OnDestroy()
    {
        // Clean up subscription to avoid dangling references
        if (_coreHealth != null && _hasSubscribed)
        {
            _coreHealth.onDeath.RemoveListener(OnCoreDeath);
        }
    }

    /// <summary>
    /// Called when the core's Health reaches zero (before the core GameObject is destroyed).
    /// Freezes time and shows the game-over screen.
    /// </summary>
    private void OnCoreDeath()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;

        // Show the game-over UI
        if (gameOverCanvas != null)
            gameOverCanvas.enabled = true;

        // Freeze game time (Update still runs, but everything with Time.deltaTime stops)
        Time.timeScale = 0f;
    }

    private void Update()
    {
        // Only check for restart input when the game is over
        if (!_isGameOver)
            return;

        if (Input.GetKeyDown(restartKey))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Restores time scale and reloads the current scene.
    /// Can be called externally (e.g., from a UI button).
    /// </summary>
    public void RestartGame()
    {
        // Restore time before loading the scene
        Time.timeScale = 1f;

        // Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
