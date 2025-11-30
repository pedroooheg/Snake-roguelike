using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform target;

    [Header("Velocidade")]
    public float baseSpeed = 4f;   // velocidade inicial
    private float speed;           // velocidade com dificuldade aplicada

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // se o spawner não chamou ApplyDifficulty a tempo, usa baseSpeed
        if (speed <= 0f)
            speed = baseSpeed;
    }

    // chamado pelo EnemySpawner
    public void ApplyDifficulty(float diff)
    {
        // quanto maior a dificuldade, mais rápido
        speed = baseSpeed * (1f + diff * 0.5f);
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        Vector3 newPos = rb.position + direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(newPos);

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
