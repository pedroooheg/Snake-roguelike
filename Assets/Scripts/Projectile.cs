using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // parede
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        // inimigo (se estiver usando tag Enemy)
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
