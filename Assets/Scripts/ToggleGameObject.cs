using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleGameObject : MonoBehaviour
{
    
    [SerializeField] private GameObject next_gameobject;
    [SerializeField] private GameObject current_gameobject;

    [SerializeField] private Button toggle_button;

    [SerializeField] private Button back_button;

    private Stack<GameObject> gameobject_stack = new Stack<GameObject>();  // stack to remember the previous gameobject of the back button

    private void Start()
    {
        if (toggle_button != null)
        {
            toggle_button.onClick.AddListener(Toggle);
        }

        if (back_button != null)
        {
            back_button.onClick.AddListener(GoBack);
        }

        // stacK
        if (next_gameobject != null && next_gameobject.activeSelf)
        {
            gameobject_stack.Push(next_gameobject);
        }
        else if (current_gameobject != null && current_gameobject.activeSelf)
        {
            gameobject_stack.Push(current_gameobject);
        }
    }

    private void Toggle()
    {
        if (next_gameobject != null)
        {
            GameObject current_active = next_gameobject.activeSelf ? next_gameobject : current_gameobject;

            bool is_active = next_gameobject.activeSelf;
            next_gameobject.SetActive(!is_active);

            if (current_gameobject != null)
            {
                current_gameobject.SetActive(is_active);
            }

            if (current_active != null)
            {
                gameobject_stack.Push(current_active);
            }
        }
    }

    private void GoBack()   // seperate button logic so it can be used with the back voice command
    {
        if (gameobject_stack.Count > 1)
        {
            gameobject_stack.Pop(); // remove from the stack

            GameObject previous = gameobject_stack.Peek();
            if (next_gameobject != null) next_gameobject.SetActive(next_gameobject == previous);
            if (current_gameobject != null) current_gameobject.SetActive(current_gameobject == previous);
        }
    }
}
