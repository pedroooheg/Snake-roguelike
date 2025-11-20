using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;          // cabeça da cobra (objeto com script mov)
    public GameObject enemyPrefab;    // prefab do inimigo
    public Renderer groundRenderer;   // renderer do chão (Ground_03)

    public float spawnInterval = 2f;  // tempo entre spawns (segundos)
    public int maxEnemies = 20;       // limite de inimigos na cena

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (CountEnemies() < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private int CountEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || player == null || groundRenderer == null)
            return;

        Bounds b = groundRenderer.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);

        // começa alto
        Vector3 spawnPos = new Vector3(x, 100f, z);

        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, 200f))
        {
            spawnPos = hit.point + Vector3.up * 0.5f; // meio metro acima do chão
        }
        else
        {
            // fallback
            spawnPos.y = b.max.y + 0.5f;
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        EnemyFollow follow = enemy.GetComponent<EnemyFollow>();
        if (follow != null)
            follow.target = player;
    }

}
