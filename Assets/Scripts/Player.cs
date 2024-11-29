using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int health = 100;

    public void Attack1(Enemy enemy)
    {
        int damage = 10;
        Debug.Log($"{playerName} used Attack1 on {enemy.name} for {damage} damage!");
        enemy.TakeDamage(damage);
    }

    public void Attack2(Enemy enemy)
    {
        int damage = 20;
        Debug.Log($"{playerName} used Attack2 on {enemy.name} for {damage} damage!");
        enemy.TakeDamage(damage);
    }
}