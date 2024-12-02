using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFont : MonoBehaviour
{
    public TMP_FontAsset font_1;        
    public TMP_FontAsset font_2;        
    public bool useFirstFont = true;     
    public float fontSizeMultiplier = 0.8f;

    public void ToggleFonts()
    {
        useFirstFont = !useFirstFont;     // switch between true and false when toggled

        TextMeshProUGUI[] tmpTextComponents = FindObjectsOfType<TextMeshProUGUI>(true); // find all active and inactive objects
        foreach (TextMeshProUGUI tmp in tmpTextComponents)
        {
            tmp.font = useFirstFont ? font_1 : font_2;
            tmp.fontSize = useFirstFont ? tmp.fontSize / fontSizeMultiplier : tmp.fontSize * fontSizeMultiplier;
        }
    }
}
