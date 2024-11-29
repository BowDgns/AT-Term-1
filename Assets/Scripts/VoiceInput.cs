using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq; // For LINQ methods like ToArray

public class MultiButtonToggleVoiceControl : MonoBehaviour
{
    public List<Button> buttons;  // List for buttons
    public List<string> buttonPhrases;  // Phrases for button actions

    public List<Toggle> toggles;  // List for toggles
    public List<string> togglePhrases;  // Phrases for toggle actions

    // Volume control sliders and audio sources
    public Slider sfxSlider;        // Slider for SFX volume
    public Slider musicSlider;      // Slider for Music volume
    public AudioSource sfxAudio;    // AudioSource for SFX
    public AudioSource musicAudio;  // AudioSource for Music

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Button> buttonCommands;
    private Dictionary<string, Toggle> toggleCommands;

    void Start()
    {
        // Ensure the phrases and UI elements match in count
        if (buttons.Count != buttonPhrases.Count || toggles.Count != togglePhrases.Count)
        {
            Debug.LogError("The number of UI elements and phrases must match!");
            return;
        }

        // Step 1: Populate dictionaries with phrases and their associated buttons and toggles
        buttonCommands = new Dictionary<string, Button>();
        for (int i = 0; i < buttonPhrases.Count; i++)
        {
            buttonCommands[buttonPhrases[i]] = buttons[i];
        }

        toggleCommands = new Dictionary<string, Toggle>();
        for (int i = 0; i < togglePhrases.Count; i++)
        {
            toggleCommands[togglePhrases[i]] = toggles[i];
        }

        // Step 2: Add volume control phrases
        var volumeControlPhrases = new List<string>
        {
            "set sfx volume to",
            "set music volume to",
            "set volume to zero",
            "set volume to one hundred"
        };

        // Combine button, toggle, and volume control phrases
        var allPhrases = buttonPhrases.Concat(togglePhrases).Concat(volumeControlPhrases).ToArray();
        keywordRecognizer = new KeywordRecognizer(allPhrases);
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;

        // Step 3: Start the recognizer
        keywordRecognizer.Start();
        Debug.Log("Keyword Recognizer started. Say a command to trigger buttons, toggles, or volume control.");
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log($"Recognized phrase: {args.text}");

        // Step 4: Handle button commands
        if (buttonCommands.TryGetValue(args.text, out Button button))
        {
            Debug.Log($"Triggering button: {button.name}");
            button.onClick.Invoke(); // Trigger the button's onClick event
        }
        // Step 5: Handle toggle commands
        else if (toggleCommands.TryGetValue(args.text, out Toggle toggle))
        {
            Debug.Log($"Toggling state of: {toggle.name}");
            toggle.isOn = !toggle.isOn; // Toggle the state of the toggle
        }
        // Step 6: Handle volume control commands
        else if (args.text.Contains("set sound volume to"))
        {
            int volume = ExtractVolume(args.text);
            SetSFXVolume(volume);
        }
        else if (args.text.Contains("set music volume to"))
        {
            int volume = ExtractVolume(args.text);
            SetMusicVolume(volume);
        }
        else if (args.text.Contains("set volume to zero"))
        {
            SetSFXVolume(0); // Set both volumes to 0%
            SetMusicVolume(0);
        }
        else if (args.text.Contains("set volume to one hundred"))
        {
            SetSFXVolume(100); // Set both volumes to 100%
            SetMusicVolume(100);
        }
        else
        {
            Debug.LogWarning($"Phrase '{args.text}' not recognized!");
        }
    }

    // Extract the volume percentage from the recognized command
    private int ExtractVolume(string command)
    {
        string[] words = command.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        int volume = 0;

        // Try to parse the last word of the command as a volume percentage
        if (words.Length >= 4 && int.TryParse(words[words.Length - 1], out volume))
        {
            return Mathf.Clamp(volume, 0, 100);  // Ensure volume is between 0 and 100
        }

        return 50;  // Default to 50% if parsing fails
    }

    // Method to set the SFX volume
    private void SetSFXVolume(int percentage)
    {
        float volumeValue = Mathf.Clamp(percentage / 100f, 0f, 1f);
        sfxSlider.value = volumeValue; // Update SFX slider
        sfxAudio.volume = volumeValue; // Set SFX AudioSource volume
        Debug.Log($"SFX volume set to {percentage}%");
    }

    // Method to set the Music volume
    private void SetMusicVolume(int percentage)
    {
        float volumeValue = Mathf.Clamp(percentage / 100f, 0f, 1f);
        musicSlider.value = volumeValue; // Update Music slider
        musicAudio.volume = volumeValue; // Set Music AudioSource volume
        Debug.Log($"Music volume set to {percentage}%");
    }

    // Clean up the recognizer when the object is destroyed
    void OnDestroy()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}
