using UnityEngine;
using UnityEngine.UI;

public class TextTime : MonoBehaviour
{
    public Slider timeSlider;
    public float text_time = 2f;

    void Start()
    {
        if (timeSlider != null)
            timeSlider.value = text_time;
    }

    public void OnSliderValueChanged()
    {
        if (timeSlider != null)
            text_time = timeSlider.value;

    }
}


