using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform target;   // cabeça da cobra
    public float speed = 5f;   // velocidade do inimigo

    void Update()
    {
        if (target == null) return;

        // direção até o alvo (cobra)
        Vector3 dir = (target.position - transform.position).normalized;

        // movimento em direção ao alvo
        transform.position += dir * speed * Time.deltaTime;

        // gira pra olhar na direção que está andando
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}
