using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq; // For LINQ methods like ToArray

public class MultiButtonVoiceControl : MonoBehaviour
{
    public List<Button> buttons; // Assign your buttons in the Inspector
    public List<string> phrases; // Match phrases with the buttons in the Inspector

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Button> voiceCommands;

    void Start()
    {
        // Ensure the phrases and buttons match in count
        if (buttons.Count != phrases.Count)
        {
            Debug.LogError("The number of buttons and phrases must match!");
            return;
        }

        // Step 1: Populate the dictionary with phrases and their associated buttons
        voiceCommands = new Dictionary<string, Button>();
        for (int i = 0; i < phrases.Count; i++)
        {
            voiceCommands[phrases[i]] = buttons[i];
        }

        // Step 2: Initialize the KeywordRecognizer with the dictionary keys
        keywordRecognizer = new KeywordRecognizer(voiceCommands.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;

        // Step 3: Start the recognizer
        keywordRecognizer.Start();
        Debug.Log("Keyword Recognizer started. Say a command to trigger the buttons.");
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log($"Recognized phrase: {args.text}");

        // Step 4: Find the button associated with the recognized phrase and invoke its action
        if (voiceCommands.TryGetValue(args.text, out Button button))
        {
            Debug.Log($"Triggering button: {button.name}");
            button.onClick.Invoke(); // Trigger the button's onClick event
        }
        else
        {
            Debug.LogWarning($"Phrase '{args.text}' not recognized!");
        }
    }

    void OnDestroy()
    {
        // Clean up the recognizer when the object is destroyed
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}
