using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    public UIManager UI;

    public List<Player> players;
    public List<Enemy> enemies;
    public GameObject enemy_spawn;
    public Enemy current_enemy;

    public Button attack_button;
    public Button special_button;

    // AUDIO NEED TO FIX
    public AudioClip enemyDefeatedSound;
    public AudioClip damageSound;
    public AudioSource audioSource;

    private int current_player = 0;
    private bool attacked = false;

    private List<Enemy> original_enemies; 
    private List<Player> original_players; 

    void Awake()
    {
        UI = GetComponent<UIManager>();     // get the UI manager for references
    }
    private void Start()
    {
        // get original state of player and enemys for resetting
        original_enemies = new List<Enemy>(enemies);
        original_players = new List<Player>(players);

        attack_button.gameObject.SetActive(false);
        special_button.gameObject.SetActive(false);

        attack_button.onClick.AddListener(() => PlayerAttack(1));
        special_button.onClick.AddListener(() => PlayerAttack(2));

        if (enemies.Count > 0)
        {
            SpawnNextEnemy();
        }
        else
        {
            Debug.LogError("No enemies available in the list!");
        }

        StartCoroutine(GameLoop());
    }

    private void SpawnNextEnemy()
    {
        if (enemies.Count > 0)
        {
            current_enemy = Instantiate(enemies[0], enemy_spawn.transform.position, Quaternion.identity);
            enemies.RemoveAt(0);

            UpdateEnemyUI();    // update ui with new name and health
        }
        else
        {
            Debug.Log("All enemies defeated! Players win!");
            EndGame();
        }
    }

    private void UpdateEnemyUI()
    {
        if (current_enemy != null)
        {
            UI.enemy_name.text = current_enemy.enemyName;
            UI.enemy_health.text = $"{current_enemy.health}";
        }
        else
        {
            UI.enemy_name.text = "none";
            UI.enemy_health.text = "0";
        }
    }

    private IEnumerator GameLoop()
    {
        while (current_enemy != null && players.Count > 0)
        {
            for (int i = 0; i < players.Count; i++)
            {
                current_player = i;
                Player currentPlayer = players[current_player];
                yield return StartCoroutine(PlayerTurn(currentPlayer));

                if (current_enemy.health <= 0)
                {
                    Debug.Log($"{current_enemy.enemyName} has been defeated!");

                    if (audioSource != null && enemyDefeatedSound != null)
                    {
                        audioSource.PlayOneShot(enemyDefeatedSound);
                    }

                    Destroy(current_enemy.gameObject);
                    yield return new WaitForSeconds(2);
                    SpawnNextEnemy();

                    if (current_enemy == null)
                    {
                        Debug.Log("Players win!");
                        EndGame();
                        yield break;
                    }
                }
            }

            if (current_enemy != null)
            {
                yield return StartCoroutine(EnemyTurn());

                if (players.Count == 0)
                {
                    Debug.Log("Enemy wins!");
                    EndGame();
                    yield break;
                }
            }
        }
    }

    // logic for looping through player turn
    private IEnumerator PlayerTurn(Player player)
    {
        Debug.Log($"{player.player_name}'s turn!");

        attack_button.gameObject.SetActive(true);
        special_button.gameObject.SetActive(true);

        attacked = false;

        while (!attacked)
        {
            yield return null;
        }

        attack_button.gameObject.SetActive(false);
        special_button.gameObject.SetActive(false);

        Debug.Log($"{player.player_name}'s turn ended.");
        yield return new WaitForSeconds(1);
    }

    // logic for player attack
    private void PlayerAttack(int attackType)
    {
        Player currentPlayer = players[current_player];

        if (attackType == 1)
        {
            currentPlayer.Attack1(current_enemy);
        }
        else if (attackType == 2)
        {
            currentPlayer.Attack2(current_enemy);
        }

        // sounds
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (UI.enable_shake) // shake 
        {
            StartCoroutine(UI.Shake(currentPlayer.transform, 0.15f, 0.1f));
            StartCoroutine(UI.Shake(current_enemy.transform, 0.2f, 0.15f));
        }

        if (UI.enable_particles && UI.damage_effect != null)    // particles
        {
            UI.InstantiateDamageEffect(current_enemy.transform.position);
        }

        UpdateEnemyUI();
        attacked = true;
    }

    // logic for enemy turn and attack
    private IEnumerator EnemyTurn()
    {
        Debug.Log($"{current_enemy.enemyName}'s turn!");

        Player targetPlayer = players[Random.Range(0, players.Count)];
        current_enemy.AttackPlayer(targetPlayer);

        // Play damage sound
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Trigger shake effect if enabled
        if (UI.enable_shake)
        {
            StartCoroutine(UI.Shake(current_enemy.transform, 0.15f, 0.1f));
            StartCoroutine(UI.Shake(targetPlayer.transform, 0.2f, 0.15f));
        }

        if (targetPlayer.health <= 0)
        {
            Debug.Log($"{targetPlayer.player_name} has been defeated!");
            players.Remove(targetPlayer);
        }

        Debug.Log($"{current_enemy.enemyName}'s turn ended.");
        yield return new WaitForSeconds(1);
    }


    // logic for ending and resetting the game
    private void EndGame()
    {
        Debug.Log("Game Over! Resetting game...");
        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(3); // Optional delay before resetting

        // Reset players and enemies
        players = new List<Player>(original_players);
        enemies = new List<Enemy>(original_enemies);

        foreach (Player player in players)
        {
            player.ResetHealth(); // Ensure this method resets player health
        }

        current_enemy = null;
        current_player = 0;

        SpawnNextEnemy();
        StartCoroutine(GameLoop());
    }
}
