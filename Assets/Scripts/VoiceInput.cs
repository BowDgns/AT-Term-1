using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq; // For LINQ methods like ToArray

public class MultiButtonToggleVoiceControl : MonoBehaviour
{
    public List<Button> buttons;  
    public List<string> button_words;   // list of buttons and corresponding words to activate buttons

    public List<Toggle> toggles;  
    public List<string> toggle_words;  // same lists but for toggles

    private KeywordRecognizer keyword_recognizer;
    private Dictionary<string, Button> button_dictionary;
    private Dictionary<string, Toggle> toggle_dictionary;

    // Volume control sliders and audio sources
    public Slider sfxSlider;        // Slider for SFX volume
    public Slider musicSlider;      // Slider for Music volume
    public AudioSource sfxAudio;    // AudioSource for SFX
    public AudioSource musicAudio;  // AudioSource for Music

    void Start()
    {
        button_dictionary = new Dictionary<string, Button>();   // dictionary with the word and the button it is linked to
        for (int i = 0; i < button_words.Count; i++)
        {
            button_dictionary[button_words[i]] = buttons[i];
        }

        toggle_dictionary = new Dictionary<string, Toggle>();
        for (int i = 0; i < toggle_words.Count; i++)
        {
            toggle_dictionary[toggle_words[i]] = toggles[i];
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
        var allPhrases = button_words.Concat(toggle_words).Concat(volumeControlPhrases).ToArray();
        keyword_recognizer = new KeywordRecognizer(allPhrases);
        keyword_recognizer.OnPhraseRecognized += OnPhraseRecognized;

        // Step 3: Start the recognizer
        keyword_recognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log($"Recognized phrase: {args.text}");

        // button click if voice input matches a phrase in the list
        if (button_dictionary.TryGetValue(args.text, out Button button))
        {
            button.onClick.Invoke(); 
        }
        // activate or deactivate toggle if voice input matches
        else if (toggle_dictionary.TryGetValue(args.text, out Toggle toggle))
        {
            Debug.Log($"Toggling state of: {toggle.name}");
            toggle.isOn = !toggle.isOn;
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
        if (keyword_recognizer != null && keyword_recognizer.IsRunning)
        {
            keyword_recognizer.Stop();
            keyword_recognizer.Dispose();
        }
    }
}
