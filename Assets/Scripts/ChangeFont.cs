using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFont : MonoBehaviour
{
    public Font legacyFont1;              // Font for Unity UI Text
    public Font legacyFont2;              // Alternative Font for Unity UI Text
    public TMP_FontAsset tmpFont1;        // Font for TextMeshProUGUI
    public TMP_FontAsset tmpFont2;        // Alternative Font for TextMeshProUGUI
    public bool useFirstFont = true;      // Toggle to determine which font to use
    public float fontSizeMultiplier = 0.8f; // Multiplier to reduce the size of font2

    void Start()
    {
        ToggleFonts();
    }

    public void ToggleFonts()
    {
        useFirstFont = !useFirstFont;     // Switch between true and false

        // For Unity UI (legacy) Text components
        Text[] legacyTextComponents = FindObjectsOfType<Text>();
        foreach (Text txt in legacyTextComponents)
        {
            txt.font = useFirstFont ? legacyFont1 : legacyFont2;

            // Adjust font size based on which font is active, casting to int
            txt.fontSize = useFirstFont ? (int)(txt.fontSize / fontSizeMultiplier) : (int)(txt.fontSize * fontSizeMultiplier);
        }

        // For TextMeshProUGUI components
        TextMeshProUGUI[] tmpTextComponents = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmp in tmpTextComponents)
        {
            tmp.font = useFirstFont ? tmpFont1 : tmpFont2;

            // Adjust font size based on which font is active
            tmp.fontSize = useFirstFont ? tmp.fontSize / fontSizeMultiplier : tmp.fontSize * fontSizeMultiplier;
        }
    }
}
