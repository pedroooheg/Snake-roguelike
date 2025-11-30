using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int baseHealth = 3;
    private int currentHealth;

    private void Start()
    {
        if (currentHealth <= 0)
            currentHealth = baseHealth;
    }

    // chamado pelo EnemySpawner
    public void ApplyDifficulty(float diff)
    {
        // aumenta vida conforme dificuldade
        currentHealth = Mathf.RoundToInt(baseHealth * (1f + diff * 0.8f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            TakeDamage(1);
            Destroy(other.gameObject); // projétil do player sempre some
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
