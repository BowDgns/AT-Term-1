using UnityEngine;
using System.Collections;
using TMPro;

public class Enemy : MonoBehaviour
{
    public string enemy_name;
    public int health = 100;

    // Add public fields for minimum and maximum damage range
    public int min_damage = 5;
    public int max_damage = 15;

    public TMP_Text damage_text;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{enemy_name} takes {damage} damage! Health left: {health}");
    }

    public void AttackPlayer(Player player)
    {
        // Use the minDamage and maxDamage range for the attack
        int damage = Random.Range(min_damage, max_damage);
        player.health -= damage;
        Debug.Log($"{enemy_name} attacks {player.player_name} for {damage} damage! Player health left: {player.health}");
        player.UpdateHealth();

        player.damage_text.text = $"-{damage}";

        StartCoroutine(ClearDamageText(player));
        
    }
    private IEnumerator ClearDamageText(Player player)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Set the player's damage text to nothing
        player.damage_text.text = "";
    }
}
