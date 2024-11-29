using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleGameObject : MonoBehaviour
{
    // Reference to the primary GameObject you want to toggle
    [SerializeField] private GameObject primaryObject;

    // Optional secondary GameObject to activate when the primary is deactivated
    [Tooltip("Optional: Set a secondary GameObject active when the primary is toggled off")]
    [SerializeField] private GameObject secondaryObject;

    // For toggling using a UI Button
    [Tooltip("Assign the UI Button to toggle the GameObject")]
    [SerializeField] private Button toggleButton;

    // Back button for returning to the previous GameObject
    [Tooltip("Assign the UI Button to go back to the previous GameObject")]
    [SerializeField] private Button backButton;

    // Stack to track active GameObjects for "Back" functionality
    private Stack<GameObject> activeObjectStack = new Stack<GameObject>();

    private void Start()
    {
        // Register the toggle button click event
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(Toggle);
        }

        // Register the back button click event
        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBack);
        }

        // Initialize the stack with the currently active GameObject
        if (primaryObject != null && primaryObject.activeSelf)
        {
            activeObjectStack.Push(primaryObject);
        }
        else if (secondaryObject != null && secondaryObject.activeSelf)
        {
            activeObjectStack.Push(secondaryObject);
        }
    }

    // Method to toggle the active state of the primary object and handle the secondary object
    private void Toggle()
    {
        if (primaryObject != null)
        {
            // Determine the current active object
            GameObject currentActive = primaryObject.activeSelf ? primaryObject : secondaryObject;

            // Toggle the primary object's state
            bool isPrimaryActive = primaryObject.activeSelf;
            primaryObject.SetActive(!isPrimaryActive);

            // If a secondary object is assigned, set it active when the primary is inactive
            if (secondaryObject != null)
            {
                secondaryObject.SetActive(isPrimaryActive);
            }

            // Push the previously active object onto the stack
            if (currentActive != null)
            {
                activeObjectStack.Push(currentActive);
            }
        }
        else
        {
            Debug.LogWarning("Primary GameObject is not assigned.");
        }
    }

    // Method to return to the previously active GameObject
    private void GoBack()
    {
        // Ensure there is a previous state to return to
        if (activeObjectStack.Count > 1)
        {
            // Pop the current active object from the stack
            activeObjectStack.Pop();

            // Activate the previous object and deactivate others
            GameObject previousActive = activeObjectStack.Peek();
            if (primaryObject != null) primaryObject.SetActive(primaryObject == previousActive);
            if (secondaryObject != null) secondaryObject.SetActive(secondaryObject == previousActive);
        }
        else
        {
            Debug.LogWarning("No previous GameObject to go back to.");
        }
    }
}
