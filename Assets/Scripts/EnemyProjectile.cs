using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifeTime = 5f;

    private Vector3 direction;
    private float speed;

    public void Init(Vector3 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // atinge o jogador
        if (other.CompareTag("Player"))
        {
            mov player = other.GetComponent<mov>();
            if (player != null)
            {
                player.TakeHitFromProjectile();
            }
            Destroy(gameObject);
        }

        // bateu na parede, some
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
