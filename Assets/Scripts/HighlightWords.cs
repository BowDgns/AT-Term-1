using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HighlightToggleWithUI : MonoBehaviour
{
    public Toggle highlight_toggle;
    private List<GameObject> highlight_tags;

    void Start()
    {
        highlight_tags = new List<GameObject>();

        FindAllObjectsWithTag("highlight");     // select everything tagged with highlight (canvas panels)

        if (highlight_toggle != null)
        {
            UpdateHighlightObjects(highlight_toggle.isOn);
            highlight_toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }
    void FindAllObjectsWithTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)      // add inactive objects with the highlight tag (by default they are all toggled of)
        {
            if (obj.CompareTag(tag) && !highlight_tags.Contains(obj))
            {
                highlight_tags.Add(obj);
            }
        }
    }
    void OnToggleValueChanged(bool isOn)
    {
        UpdateHighlightObjects(isOn);
    }

    void UpdateHighlightObjects(bool isActive)
    {
        foreach (GameObject obj in highlight_tags)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);    // set hihglight to be on or off
            }
        }
    }

    void OnDestroy()
    {
        if (highlight_toggle != null)
        {
            highlight_toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}