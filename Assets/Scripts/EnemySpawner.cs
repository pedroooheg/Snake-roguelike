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
        // No começo só Runner, depois libera Tank, depois Shooter
        float r = Random.value;

        if (difficulty < 1.5f)
        {
            // só Runner
            return runnerPrefab;
        }
        else if (difficulty < 3f)
        {
            // Runner mais comum, Tank às vezes
            if (r < 0.7f) return runnerPrefab;
            else return tankPrefab;
        }
        else
        {
            // todos aparecem
            if (r < 0.5f) return runnerPrefab;
            else if (r < 0.8f) return tankPrefab;
            else return shooterPrefab;
        }
    }
}
