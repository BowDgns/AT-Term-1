using UnityEngine;
using UnityEngine.UI;

public class ToggleGameObject : MonoBehaviour
{
    // Enum to choose between key or button input
    public enum ToggleMode { Key, Button }
    [SerializeField] private ToggleMode toggleMode = ToggleMode.Key;

    // Reference to the primary GameObject you want to toggle
    [SerializeField] private GameObject primaryObject;

    // Optional secondary GameObject to activate when the primary one is deactivated
    [Tooltip("Optional: Set a secondary GameObject active when the primary is toggled off")]
    [SerializeField] private GameObject secondaryObject;

    // For toggling using a keyboard key
    [Tooltip("Select the key to toggle the GameObject")]
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    // For toggling using a UI Button
    [Tooltip("Assign the UI Button to toggle the GameObject")]
    [SerializeField] private Button toggleButton;

    private void Start()
    {
        // If using Button mode, register the button click event
        if (toggleMode == ToggleMode.Button && toggleButton != null)
        {
            toggleButton.onClick.AddListener(Toggle);
        }
    }

    private void Update()
    {
        // If using Key mode, check if the specified key is pressed
        if (toggleMode == ToggleMode.Key && Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }
    }

    // Method to toggle the active state of the primary object and handle the secondary object
    private void Toggle()
    {
        if (primaryObject != null)
        {
            // Toggle the active state of the primary object
            bool isPrimaryActive = primaryObject.activeSelf;
            primaryObject.SetActive(!isPrimaryActive);

            // If a secondary object is assigned, set it active when the primary is inactive
            if (secondaryObject != null)
            {
                secondaryObject.SetActive(isPrimaryActive);
            }
        }
        else
        {
            Debug.LogWarning("Primary GameObject is not assigned.");
        }
    }
}
