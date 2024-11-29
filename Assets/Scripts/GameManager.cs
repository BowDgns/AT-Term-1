using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Player> players;
    public List<Enemy> enemies;
    public GameObject enemySpawnPoint;
    public Enemy currentEnemy;

    public Button attack1Button;
    public Button attack2Button;

    public AudioClip enemyDefeatedSound;
    public AudioClip damageSound;
    public AudioSource audioSource;

    // Toggles for effects
    public bool enableShakeEffect = true;
    public bool enableDamageParticles = true;

    public Toggle shakeEffectToggle; // UI Toggle for shake effect
    public Toggle damageParticlesToggle; // UI Toggle for damage particles

    // Particle effect for when the enemy takes damage
    public GameObject damageEffectPrefab;

    private int currentPlayerIndex = 0;
    private bool chosenAttack = false;

    private void Start()
    {
        // Initialize shake effect toggle
        if (shakeEffectToggle != null)
        {
            shakeEffectToggle.isOn = enableShakeEffect;
            shakeEffectToggle.onValueChanged.AddListener(ToggleShakeEffect);
        }

        // Initialize damage particles toggle
        if (damageParticlesToggle != null)
        {
            damageParticlesToggle.isOn = enableDamageParticles;
            damageParticlesToggle.onValueChanged.AddListener(ToggleDamageParticles);
        }

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

    public void ToggleShakeEffect(bool isEnabled)
    {
        enableShakeEffect = isEnabled;
        Debug.Log($"Shake effect enabled: {enableShakeEffect}");
    }

    public void ToggleDamageParticles(bool isEnabled)
    {
        enableDamageParticles = isEnabled;
        Debug.Log($"Damage particles enabled: {enableDamageParticles}");
    }

    private void SpawnNextEnemy()
    {
        if (enemies.Count > 0)
        {
            currentEnemy = Instantiate(enemies[0], enemySpawnPoint.transform.position, Quaternion.identity);
            enemies.RemoveAt(0);

            Debug.Log($"A new enemy has appeared: {currentEnemy.enemyName}");
        }
        else
        {
            Debug.Log("All enemies defeated! Players win!");
            StopAllCoroutines();
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
                    yield break;
                }
            }
        }
    }

    private IEnumerator PlayerTurn(Player player)
    {
        Debug.Log($"{player.playerName}'s turn!");

        attack1Button.gameObject.SetActive(true);
        attack2Button.gameObject.SetActive(true);

        chosenAttack = false;

        while (!chosenAttack)
        {
            yield return null;
        }

        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        Debug.Log($"{player.playerName}'s turn ended.");
        yield return new WaitForSeconds(1);
    }

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

        // Play damage sound
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Trigger shake effect if enabled
        if (enableShakeEffect)
        {
            StartCoroutine(Shake(currentPlayer.transform, 0.15f, 0.1f));
            StartCoroutine(Shake(currentEnemy.transform, 0.2f, 0.15f));
        }

        // Trigger damage particles if enabled
        if (enableDamageParticles && damageEffectPrefab != null)
        {
            InstantiateDamageEffect(currentEnemy.transform.position);
        }

        chosenAttack = true;
    }

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
        if (enableShakeEffect)
        {
            StartCoroutine(Shake(currentEnemy.transform, 0.15f, 0.1f));
            StartCoroutine(Shake(targetPlayer.transform, 0.2f, 0.15f));
        }

        if (targetPlayer.health <= 0)
        {
            Debug.Log($"{targetPlayer.playerName} has been defeated!");
            players.Remove(targetPlayer);
        }

        Debug.Log($"{currentEnemy.enemyName}'s turn ended.");
        yield return new WaitForSeconds(1);
    }

    private void InstantiateDamageEffect(Vector3 position)
    {
        if (!enableDamageParticles || damageEffectPrefab == null) return;

        GameObject effect = Instantiate(damageEffectPrefab, position, Quaternion.identity);
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

    private IEnumerator Shake(Transform target, float duration, float magnitude)
    {
        if (!enableShakeEffect) yield break;

        Vector3 originalPosition = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;

            target.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localPosition = originalPosition;
    }
}
