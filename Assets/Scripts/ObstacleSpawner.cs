using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs dos obstáculos")]
    public List<GameObject> obstaclePrefabs;   // seus 3 prefabs (Branch, Rock, Tree)

    [Header("Configuração")]
    public int quantidade = 20;                 // quantos obstáculos spawnar
    public Renderer groundRenderer;             // chão (Ground_03)
    public Transform player;                    // cabeça da cobra
    public float distanciaMinimaDoPlayer = 5f;  // não nascer grudado no player

    private void Start()
    {
        Debug.Log("[ObstacleSpawner] Start chamado");
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        if (groundRenderer == null)
        {
            Debug.LogError("[ObstacleSpawner] groundRenderer não ligado no Inspector");
            return;
        }

        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0)
        {
            Debug.LogError("[ObstacleSpawner] Nenhum prefab de obstáculo na lista");
            return;
        }

        Bounds b = groundRenderer.bounds;
        Debug.Log($"[ObstacleSpawner] Bounds do chão: {b.min} -> {b.max}");

        int spawnCount = 0;

        for (int i = 0; i < quantidade; i++)
        {
            // posição aleatória em XZ dentro do chão
            float x = Random.Range(b.min.x, b.max.x);
            float z = Random.Range(b.min.z, b.max.z);

            // altura do chão (como é plano, isso basta)
            float y = b.max.y + 0.1f;
            Vector3 pos = new Vector3(x, y, z);

            // não nascer grudado no player
            if (player != null &&
                Vector3.Distance(pos, player.position) < distanciaMinimaDoPlayer)
            {
                i--; // tenta de novo este índice
                continue;
            }

            // escolhe prefab aleatório
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
            Instantiate(prefab, pos, Quaternion.identity);
            spawnCount++;
        }

        Debug.Log($"[ObstacleSpawner] Spawnados de fato: {spawnCount} obstáculos");
    }
}
