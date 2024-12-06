using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;

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

        if (voice_toggle != null)
        {
            voice_toggle.onValueChanged.AddListener(OnVoiceControlToggleChanged);
        }
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
        if (button_dictionary.TryGetValue(args.text, out Button button))
        {
            button.onClick.Invoke();
        }

        else if (toggle_dictionary.TryGetValue(args.text, out Toggle toggle))
        {
            toggle.isOn = !toggle.isOn;
        }
        else
        {
            Debug.LogWarning($"'{args.text}' not in the list");
        }
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
