using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int health = 100;

    // Add public fields for minimum and maximum damage range
    public int minDamage = 5;
    public int maxDamage = 15;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{enemyName} takes {damage} damage! Health left: {health}");
    }

    public void AttackPlayer(Player player)
    {
        // Use the minDamage and maxDamage range for the attack
        int damage = Random.Range(minDamage, maxDamage);
        player.health -= damage;
        Debug.Log($"{enemyName} attacks {player.player_name} for {damage} damage! Player health left: {player.health}");
        player.UpdateHealth();
    }
}
