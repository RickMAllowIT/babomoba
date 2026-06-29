using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// BABOMOBA Slice 8: R3 auto-stream toggle.
/// Reads R3 (Right Stick Click) via Unity Input System.
/// On each press, toggles the AutoStream.IsActive flag on/off.
/// Displays current auto-stream state via OnGUI.
/// Default state on Start is ON (auto-stream active).
/// </summary>
public class AutoStreamToggle : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference r3Action; // Right Stick Click

    [Header("Auto-Stream Reference")]
    [SerializeField] private AutoStream autoStream;

    [Header("UI Indicator")]
    [SerializeField] private bool showOnGUI = true;

    // Cached reference to a TextMeshPro text field (optional alternative to OnGUI)
    [SerializeField] private TMPro.TextMeshProUGUI stateText;

    private void OnEnable()
    {
        if (r3Action != null)
        {
            r3Action.action.Enable();
            r3Action.action.performed += OnR3Performed;
        }
    }

    private void OnDisable()
    {
        if (r3Action != null)
        {
            r3Action.action.performed -= OnR3Performed;
            r3Action.action.Disable();
        }
    }

    private void Start()
    {
        // Auto-find AutoStream on the same GameObject if not assigned
        if (autoStream == null)
        {
            autoStream = GetComponent<AutoStream>();
            if (autoStream == null)
            {
                Debug.LogWarning("[AutoStreamToggle] No AutoStream component found on this GameObject. Searching scene...");
                autoStream = FindObjectOfType<AutoStream>();
            }
        }

        // Default state: ON
        if (autoStream != null)
        {
            autoStream.IsActive = true;
            Debug.Log("[AutoStreamToggle] Default state: ON");
        }

        // Update state text if assigned
        UpdateStateText();
    }

    private void OnR3Performed(InputAction.CallbackContext context)
    {
        if (autoStream == null)
        {
            Debug.LogWarning("[AutoStreamToggle] AutoStream reference is null — cannot toggle.");
            return;
        }

        // Toggle the IsActive flag
        autoStream.IsActive = !autoStream.IsActive;

        string state = autoStream.IsActive ? "ON" : "OFF";
        Debug.Log($"[AutoStreamToggle] Auto-stream toggled {state}");

        // Update UI text if assigned
        UpdateStateText();
    }

    /// <summary>
    /// Updates the optional TextMeshPro text field with the current state.
    /// </summary>
    private void UpdateStateText()
    {
        if (stateText != null && autoStream != null)
        {
            stateText.text = autoStream.IsActive
                ? "AUTO-STREAM: ON"
                : "AUTO-STREAM: OFF";
        }
    }

    /// <summary>
    /// Simple OnGUI indicator showing current auto-stream state.
    /// Only draws if showOnGUI is true and no stateText is assigned.
    /// </summary>
    private void OnGUI()
    {
        if (!showOnGUI || autoStream == null || stateText != null)
            return;

        string state = autoStream.IsActive ? "ON" : "OFF";
        Color color = autoStream.IsActive ? Color.green : Color.red;

        // Draw a simple box in the top-center of the screen
        float boxWidth = 220f;
        float boxHeight = 40f;
        float x = (Screen.width - boxWidth) / 2f;
        float y = 80f;

        GUI.backgroundColor = color;
        GUI.Box(new Rect(x, y, boxWidth, boxHeight), $"AUTO-STREAM: {state}");
    }
}
