using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float baseCooldown = 3f;
    public float projectileSpeed = 20f;

    private Transform target;
    private float cooldown;
    private float timer;

    public void Init(Transform player, float difficulty)
    {
        target = player;

        // só começa a atirar depois de certa dificuldade
        if (difficulty < 0.5f)
        {
            enabled = false;
            return;
        }

        enabled = true;
        cooldown = Mathf.Max(0.5f, baseCooldown - difficulty * 0.2f);
    }

    private void Update()
    {
        if (target == null || projectilePrefab == null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = cooldown;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        dir.Normalize();

        Vector3 spawnPos = transform.position + dir * 1.2f;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rot);
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
            ep.Init(dir, projectileSpeed);
    }
}
