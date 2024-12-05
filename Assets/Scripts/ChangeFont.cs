using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFont : MonoBehaviour
{
    public TMP_FontAsset font_1;        
    public TMP_FontAsset font_2;        
    public bool use_game_font = true;     
    public float font_multiplyer = 0.8f;

    public void ToggleFonts()
    {
        use_game_font = !use_game_font;     // switch between true and false when toggled

        TextMeshProUGUI[] tmpTextComponents = FindObjectsOfType<TextMeshProUGUI>(true); // find all active and inactive objects

        foreach (TextMeshProUGUI tmp in tmpTextComponents)
        {
            tmp.font = use_game_font ? font_1 : font_2;
            tmp.fontSize = use_game_font ? tmp.fontSize / font_multiplyer : tmp.fontSize * font_multiplyer;
        }
    }
}
