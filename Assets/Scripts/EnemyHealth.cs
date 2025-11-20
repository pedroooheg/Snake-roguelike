using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(gameObject);      // mata inimigo
            Destroy(other.gameObject); // remove projétil
        }
    }
}
