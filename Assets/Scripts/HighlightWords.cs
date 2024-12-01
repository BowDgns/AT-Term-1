using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HighlightToggleWithUI : MonoBehaviour
{
    // Reference to the UI Toggle
    public Toggle highlightToggle;

    // List to store references to all objects tagged "highlight"
    private List<GameObject> highlightObjects;

    void Start()
    {
        // Initialize the list
        highlightObjects = new List<GameObject>();

        // Find all objects, including inactive ones, using a custom method
        FindAllObjectsWithTag("highlight");

        if (highlightToggle != null)
        {
            // Initialize objects to match the toggle's current state
            UpdateHighlightObjects(highlightToggle.isOn);

            // Add a listener to respond to the toggle's state changes
            highlightToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        else
        {
            Debug.LogError("Highlight Toggle is not assigned in the Inspector!");
        }
    }

    // Custom method to find all objects with the specified tag, including inactive ones
    void FindAllObjectsWithTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag) && !highlightObjects.Contains(obj))
            {
                highlightObjects.Add(obj);
            }
        }
    }

    // Called whenever the toggle value changes
    void OnToggleValueChanged(bool isOn)
    {
        UpdateHighlightObjects(isOn);
    }

    // Update the active state of all objects
    void UpdateHighlightObjects(bool isActive)
    {
        foreach (GameObject obj in highlightObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }

    void OnDestroy()
    {
        // Remove the listener to avoid potential memory leaks
        if (highlightToggle != null)
        {
            highlightToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}
