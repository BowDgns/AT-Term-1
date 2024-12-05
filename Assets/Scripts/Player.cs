using UnityEngine;
using TMPro;
using System.Collections;

public class Player : MonoBehaviour
{
    public string player_name;
    public int health = 100;
    public TMP_Text health_text;
    public TMP_Text damage_text;

    public TextTime time;
    private float text_time;

    public int min_damage = 10;
    public int max_damage = 20;

    public GameObject player_highlight;

    public void Attack1(Enemy enemy)
    {
        int damage = Random.Range(min_damage, max_damage +1);
        Debug.Log($"{player_name} used Attack1 on {enemy.name} for {damage} damage!");
        enemy.TakeDamage(damage);

        enemy.damage_text.text = $"-{damage}";
        StartCoroutine(ClearDamageText(enemy));
    }

    public void Attack2(Enemy enemy)
    {
        int damage = Random.Range(min_damage, max_damage + 1);
        Debug.Log($"{player_name} used Attack2 on {enemy.name} for {damage} damage!");
        enemy.TakeDamage(damage);

        enemy.damage_text.text = $"-{damage}";
        StartCoroutine(ClearDamageText(enemy));

    }

    public void ResetHealth()
    {
        health = 100;
    }

    public void UpdateHealth()
    {
        health_text.text = $"{health}";
    }

    private void Start()
    {
        UpdateHealth();
    }

    private void Update()
    {
        text_time = time.text_time;
        //Debug.Log(text_time);
    }

    private IEnumerator ClearDamageText(Enemy enemy)
    {
        yield return new WaitForSeconds(text_time);
        enemy.damage_text.text = "";
    }
}
