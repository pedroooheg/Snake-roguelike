using UnityEngine;
using System.Collections;

public class CoinSpawner : MonoBehaviour
{
    public Renderer groundRenderer;   //  Ground_03 aqui
    public GameObject coinPrefab;     // Coin prefab 

    public float spawnInterval = 2f;  // tempo entre tentativas de spawn
    public int maxCoins = 10;         // máximo de moedas simultâneas

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (CountCoins() < maxCoins)
            {
                SpawnCoin();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private int CountCoins()
    {
        return GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    private void SpawnCoin()
    {
        if (coinPrefab == null || groundRenderer == null)
            return;

        // Pega os limites do chão
        Bounds b = groundRenderer.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);

        // começa bem alto e faz um raycast pra achar o chão
        Vector3 spawnPos = new Vector3(x, 100f, z);

        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, 200f))
        {
            spawnPos = hit.point + Vector3.up * 0.3f; // um pouco acima do chão
        }
        else
        {
            spawnPos.y = b.max.y + 0.3f;
        }

        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }
}
