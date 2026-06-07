using System;
using System.Collections;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public event Action<string> OnCombatLog;
    public event Action<float, float> OnPlayerHPChanged;   // current, max
    public event Action<float, float> OnEnemyHPChanged;    // current, max

    private bool _combatActive;

    public void StartCombat(RuntimePlayerStats player, EnemyStats enemy, Action<bool> onCombatEnd)
    {
        if (_combatActive) StopAllCoroutines();
        StartCoroutine(CombatLoop(player, enemy, onCombatEnd));
    }

    private IEnumerator CombatLoop(RuntimePlayerStats player, EnemyStats enemy, Action<bool> onCombatEnd)
    {
        _combatActive = true;

        float playerAttackTimer = 0f;
        float enemyAttackTimer  = 0f;

        OnPlayerHPChanged?.Invoke(player.currentHP, player.maxHP);
        OnEnemyHPChanged?.Invoke(enemy.currentHP, enemy.maxHP);

        Log($"Battle starts! You face {enemy.name}.");

        while (player.currentHP > 0 && enemy.currentHP > 0)
        {
            float delta = Time.deltaTime;
            playerAttackTimer += delta;
            enemyAttackTimer  += delta;

            if (playerAttackTimer >= 1f / player.attackSpeed)
            {
                playerAttackTimer = 0f;
                float dmg = player.damage;
                if (UnityEngine.Random.value < player.critChance)
                {
                    dmg *= 2f;
                    Log($"CRIT! You deal {dmg:F0} damage.");
                }
                else
                {
                    Log($"You deal {dmg:F0} damage.");
                }
                enemy.currentHP -= dmg;
                OnEnemyHPChanged?.Invoke(Mathf.Max(enemy.currentHP, 0), enemy.maxHP);
            }

            if (enemy.currentHP > 0 && enemyAttackTimer >= 1f / enemy.attackSpeed)
            {
                enemyAttackTimer = 0f;
                player.currentHP -= enemy.damage;
                Log($"{enemy.name} deals {enemy.damage:F0} damage. Your HP: {Mathf.Max(player.currentHP, 0):F0}");
                OnPlayerHPChanged?.Invoke(Mathf.Max(player.currentHP, 0), player.maxHP);
            }

            yield return null;
        }

        _combatActive = false;
        bool playerWon = enemy.currentHP <= 0;
        Log(playerWon ? "Victory!" : "You have been defeated...");
        onCombatEnd?.Invoke(playerWon);
    }

    private void Log(string msg) => OnCombatLog?.Invoke(msg);
}

// Plain data classes — no MonoBehaviour, easy to create and pass around.

public class RuntimePlayerStats
{
    public float currentHP;
    public float maxHP;
    public float damage;
    public float attackSpeed;
    public float critChance;
    public float goldGainMultiplier;
    public int   gold;
}

public class EnemyStats
{
    public string name;
    public float  maxHP;
    public float  currentHP;
    public float  damage;
    public float  attackSpeed;
    public int    goldReward;
    public int    soulReward;
}
