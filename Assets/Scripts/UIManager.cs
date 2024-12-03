using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public TMP_Text enemy_name;
    public TMP_Text enemy_health;

    public bool enable_shake = true;
    public bool enable_particles = true;

    public Toggle shake_toggle;
    public Toggle particle_toggle;

    public GameObject damage_effect;

    public GameObject win_screen;
    public GameObject game_ui;
    public GameObject title_screen;

    // Start is called before the first frame update
    void Start()
    {
        if(shake_toggle != null)
        {
            shake_toggle.isOn = enable_shake;
            shake_toggle.onValueChanged.AddListener(ToggleShakeEffect);
        }

        if (particle_toggle != null)
        {
            particle_toggle.isOn = enable_particles;
            particle_toggle.onValueChanged.AddListener(ToggleParticleEffect);
        }
    }

    public void ToggleShakeEffect(bool is_enabled)
    {
        enable_shake = is_enabled;
    }

    public void ToggleParticleEffect(bool is_enabled)
    {
        enable_particles = is_enabled;
    }

    public IEnumerator Shake(Transform target, float duration, float magnitude)
    {
        if (!enable_shake) yield break;

        Vector3 orignal_position = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;

            target.localPosition = new Vector3(orignal_position.x + xOffset, orignal_position.y + yOffset, orignal_position.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localPosition = orignal_position;
    }

    public void InstantiateDamageEffect(Vector3 position)
    {
        if (!enable_particles || damage_effect == null) return;

        GameObject effect = Instantiate(damage_effect, position, Quaternion.identity);
        ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            Destroy(effect, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(effect, 0.5f);
        }
    }


}
