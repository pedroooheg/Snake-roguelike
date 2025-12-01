using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;          // cabeça da cobra

    [Header("Prefabs de inimigo")]
    public GameObject tankPrefab;
    public GameObject runnerPrefab;
    public GameObject shooterPrefab;

    public Renderer groundRenderer;   // chão

    [Header("Dificuldade")]
    public float baseSpawnInterval = 5f;
    public int baseMaxEnemies = 5;
    public float difficultyPerSecond = 0.05f;

    float difficulty = 0f;
    float spawnTimer = 0f;

    void Update()
    {
        if (player == null || groundRenderer == null)
            return;

        difficulty += Time.deltaTime * difficultyPerSecond;

        int maxEnemies = baseMaxEnemies + Mathf.FloorToInt(difficulty * 2f);
        float currentInterval = Mathf.Max(0.5f, baseSpawnInterval - difficulty * 0.3f);

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && CountEnemies() < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = currentInterval;
        }
    }

    int CountEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void SpawnEnemy()
    {
        // sorteia tipo de inimigo
        GameObject prefab = EscolherPrefabPorDificuldade();
        if (prefab == null) return;

        Bounds b = groundRenderer.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        Vector3 spawnPos = new Vector3(x, 100f, z);

        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, 200f))
            spawnPos = hit.point + Vector3.up * 0.5f;
        else
            spawnPos.y = b.max.y + 0.5f;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        // seguir o player (se tiver EnemyFollow)
        EnemyFollow follow = enemy.GetComponent<EnemyFollow>();
        if (follow != null)
        {
            follow.target = player;
            follow.ApplyDifficulty(difficulty);
        }

        // vida escalável
        EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.ApplyDifficulty(difficulty);
        }

        // tiro (se tiver EnemyShooter)
        EnemyShooter shooter = enemy.GetComponent<EnemyShooter>();
        if (shooter != null)
        {
            shooter.Init(player, difficulty);
        }
    }
    GameObject EscolherPrefabPorDificuldade()
    {
        // pesos base
        float wRunner = 1f;
        float wTank = 1f;
        float wShooter = 1f;

        // aumenta peso conforme dificuldade
        // (ajusta esses valores como quiser)
        wRunner = Mathf.Max(0.5f, 2.0f - difficulty * 0.3f); // runner forte no início, cai depois
        wTank = 0.5f + difficulty * 0.2f;                  // tank vai ficando mais comum
        wShooter = 0.2f + Mathf.Max(0, difficulty - 2f) * 0.3f; // shooter só começa a pesar depois

        // ignora tipos que não têm prefab
        float total = 0f;
        if (runnerPrefab != null) total += wRunner;
        if (tankPrefab != null) total += wTank;
        if (shooterPrefab != null) total += wShooter;

        if (total <= 0f) return null;

        float r = Random.value * total;

        if (runnerPrefab != null && r < wRunner) return runnerPrefab;
        r -= wRunner;

        if (tankPrefab != null && r < wTank) return tankPrefab;
        r -= wTank;

        if (shooterPrefab != null && r < wShooter) return shooterPrefab;

        // fallback
        return runnerPrefab ?? tankPrefab ?? shooterPrefab;
    }

}
