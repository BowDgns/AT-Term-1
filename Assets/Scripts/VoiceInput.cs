using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class MultiButtonToggleVoiceControl : MonoBehaviour
{
    public Toggle voice_toggle;

    public List<Button> buttons;
    public List<string> button_words;

    public List<Toggle> toggles;
    public List<string> toggle_words;

    private KeywordRecognizer keyword_recognizer;
    private Dictionary<string, Button> button_dictionary;
    private Dictionary<string, Toggle> toggle_dictionary;

    public Slider sfxSlider;
    public Slider musicSlider;
    public AudioSource sfxAudio;
    public AudioSource musicAudio;

    void Start()
    {
        button_dictionary = new Dictionary<string, Button>();
        for (int i = 0; i < button_words.Count; i++)
        {
            button_dictionary[button_words[i]] = buttons[i];
        }

        toggle_dictionary = new Dictionary<string, Toggle>();
        for (int i = 0; i < toggle_words.Count; i++)
        {
            toggle_dictionary[toggle_words[i]] = toggles[i];
        }

        var volumeControlPhrases = new List<string>
        {
            "set sfx volume to",
            "set music volume to",
            "set volume to zero",
            "set volume to one hundred"
        };

        var allPhrases = button_words.Concat(toggle_words).Concat(volumeControlPhrases).ToArray();
        keyword_recognizer = new KeywordRecognizer(allPhrases);
        keyword_recognizer.OnPhraseRecognized += OnPhraseRecognized;

        // Subscribe to the toggle's state change event
        if (voice_toggle != null)
        {
            voice_toggle.onValueChanged.AddListener(OnVoiceControlToggleChanged);
        }

        // Initialize based on toggle's starting state
        if (voice_toggle != null && voice_toggle.isOn)
        {
            keyword_recognizer.Start();
        }
    }

    private void OnVoiceControlToggleChanged(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("Voice control enabled");
            if (!keyword_recognizer.IsRunning)
            {
                keyword_recognizer.Start();
            }
        }
        else
        {
            Debug.Log("Voice control disabled");
            if (keyword_recognizer.IsRunning)
            {
                keyword_recognizer.Stop();
            }
        }
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log($"Recognized phrase: {args.text}");

        if (button_dictionary.TryGetValue(args.text, out Button button))
        {
            button.onClick.Invoke();
        }
        else if (toggle_dictionary.TryGetValue(args.text, out Toggle toggle))
        {
            Debug.Log($"Toggling state of: {toggle.name}");
            toggle.isOn = !toggle.isOn;
        }
        else if (args.text.Contains("set sfx volume to"))
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
            SetSFXVolume(0);
            SetMusicVolume(0);
        }
        else if (args.text.Contains("set volume to one hundred"))
        {
            SetSFXVolume(100);
            SetMusicVolume(100);
        }
        else
        {
            Debug.LogWarning($"Phrase '{args.text}' not recognized!");
        }
    }

    private int ExtractVolume(string command)
    {
        string[] words = command.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        int volume = 0;

        if (words.Length >= 4 && int.TryParse(words[words.Length - 1], out volume))
        {
            return Mathf.Clamp(volume, 0, 100);
        }

        return 50;
    }

    private void SetSFXVolume(int percentage)
    {
        float volumeValue = Mathf.Clamp(percentage / 100f, 0f, 1f);
        sfxSlider.value = volumeValue;
        sfxAudio.volume = volumeValue;
        Debug.Log($"SFX volume set to {percentage}%");
    }

    private void SetMusicVolume(int percentage)
    {
        float volumeValue = Mathf.Clamp(percentage / 100f, 0f, 1f);
        musicSlider.value = volumeValue;
        musicAudio.volume = volumeValue;
        Debug.Log($"Music volume set to {percentage}%");
    }

    void OnDestroy()
    {
        if (keyword_recognizer != null)
        {
            keyword_recognizer.OnPhraseRecognized -= OnPhraseRecognized;

            if (keyword_recognizer.IsRunning)
            {
                keyword_recognizer.Stop();
            }

            keyword_recognizer.Dispose();
        }

        if (voice_toggle != null)
        {
            voice_toggle.onValueChanged.RemoveListener(OnVoiceControlToggleChanged);
        }
    }
}
