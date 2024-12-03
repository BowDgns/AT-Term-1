using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public string player_name;
    public int health = 100;
    public TMP_Text health_text;

    public void Attack1(Enemy enemy)
    {
        int damage = 10;
        Debug.Log($"{player_name} used Attack1 on {enemy.name} for {damage} damage!");
        enemy.TakeDamage(damage);
    }

    public void Attack2(Enemy enemy)
    {
        int damage = 20;
        Debug.Log($"{player_name} used Attack2 on {enemy.name} for {damage} damage!");
        enemy.TakeDamage(damage);
    }

    public void ResetHealth()
    {
        health = 100;
    }

    public void UpdateHealth()
    {
        health_text.text = $"{health}";
    }
}
