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
    public GameObject enemySpawnPoint;
    public Enemy currentEnemy;

    public Button attack1Button;
    public Button attack2Button;

    public AudioClip enemyDefeatedSound;
    public AudioClip damageSound;
    public AudioSource audioSource;

    private int currentPlayerIndex = 0;
    private bool chosenAttack = false;

    private List<Enemy> originalEnemies; // Stores the initial list of enemies
    private List<Player> originalPlayers; // Stores the initial list of players

    void Awake()
    {
        UI = GetComponent<UIManager>();     // get the UI manager for references
    }
    private void Start()
    {
        // Store original states of players and enemies
        originalEnemies = new List<Enemy>(enemies);
        originalPlayers = new List<Player>(players);

        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        attack1Button.onClick.AddListener(() => PlayerAttack(1));
        attack2Button.onClick.AddListener(() => PlayerAttack(2));

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
            currentEnemy = Instantiate(enemies[0], enemySpawnPoint.transform.position, Quaternion.identity);
            enemies.RemoveAt(0);

            Debug.Log($"A new enemy has appeared: {currentEnemy.enemyName}");

            // Update UI with enemy name and health
            UpdateEnemyUI();
        }
        else
        {
            Debug.Log("All enemies defeated! Players win!");
            EndGame();
        }
    }

    private void UpdateEnemyUI()
    {
        if (currentEnemy != null)
        {
            UI.enemy_name.text = currentEnemy.enemyName;
            UI.enemy_health.text = $"{currentEnemy.health}";
        }
        else
        {
            UI.enemy_name.text = "none";
            UI.enemy_health.text = "0";
        }
    }

    private IEnumerator GameLoop()
    {
        while (currentEnemy != null && players.Count > 0)
        {
            for (int i = 0; i < players.Count; i++)
            {
                currentPlayerIndex = i;
                Player currentPlayer = players[currentPlayerIndex];
                yield return StartCoroutine(PlayerTurn(currentPlayer));

                if (currentEnemy.health <= 0)
                {
                    Debug.Log($"{currentEnemy.enemyName} has been defeated!");

                    if (audioSource != null && enemyDefeatedSound != null)
                    {
                        audioSource.PlayOneShot(enemyDefeatedSound);
                    }

                    Destroy(currentEnemy.gameObject);
                    yield return new WaitForSeconds(2);
                    SpawnNextEnemy();

                    if (currentEnemy == null)
                    {
                        Debug.Log("Players win!");
                        EndGame();
                        yield break;
                    }
                }
            }

            if (currentEnemy != null)
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

        attack1Button.gameObject.SetActive(true);
        attack2Button.gameObject.SetActive(true);

        chosenAttack = false;

        while (!chosenAttack)
        {
            yield return null;
        }

        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        Debug.Log($"{player.player_name}'s turn ended.");
        yield return new WaitForSeconds(1);
    }

    // logic for player attack
    private void PlayerAttack(int attackType)
    {
        Player currentPlayer = players[currentPlayerIndex];

        if (attackType == 1)
        {
            currentPlayer.Attack1(currentEnemy);
        }
        else if (attackType == 2)
        {
            currentPlayer.Attack2(currentEnemy);
        }

        // sounds
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (UI.enable_shake) // shake 
        {
            StartCoroutine(UI.Shake(currentPlayer.transform, 0.15f, 0.1f));
            StartCoroutine(UI.Shake(currentEnemy.transform, 0.2f, 0.15f));
        }

        if (UI.enable_particles && UI.damage_effect != null)    // particles
        {
            UI.InstantiateDamageEffect(currentEnemy.transform.position);
        }

        UpdateEnemyUI();
        chosenAttack = true;
    }

    // logic for enemy turn and attack
    private IEnumerator EnemyTurn()
    {
        Debug.Log($"{currentEnemy.enemyName}'s turn!");

        Player targetPlayer = players[Random.Range(0, players.Count)];
        currentEnemy.AttackPlayer(targetPlayer);

        // Play damage sound
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Trigger shake effect if enabled
        if (UI.enable_shake)
        {
            StartCoroutine(UI.Shake(currentEnemy.transform, 0.15f, 0.1f));
            StartCoroutine(UI.Shake(targetPlayer.transform, 0.2f, 0.15f));
        }

        if (targetPlayer.health <= 0)
        {
            Debug.Log($"{targetPlayer.player_name} has been defeated!");
            players.Remove(targetPlayer);
        }

        Debug.Log($"{currentEnemy.enemyName}'s turn ended.");
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
        players = new List<Player>(originalPlayers);
        enemies = new List<Enemy>(originalEnemies);

        foreach (Player player in players)
        {
            player.ResetHealth(); // Ensure this method resets player health
        }

        currentEnemy = null;
        currentPlayerIndex = 0;

        // Restart game loop
        SpawnNextEnemy();
        StartCoroutine(GameLoop());
    }
}
