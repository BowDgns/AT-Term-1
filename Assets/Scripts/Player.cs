using UnityEngine;
using TMPro;
using System.Collections;

public class Player : MonoBehaviour
{
    public string player_name;
    public int health = 100;
    public TMP_Text health_text;
    public TMP_Text damage_text;

    public int min_damage = 10;
    public int max_damage = 20;

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

    private IEnumerator ClearDamageText(Enemy enemy)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Set the player's damage text to nothing
        enemy.damage_text.text = "";
    }
}
