using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volume_slider;
    public AudioSource audio_source;

    void Start()
    {
        volume_slider.value = audio_source.volume;
        volume_slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        audio_source.volume = volume;
    }
}
