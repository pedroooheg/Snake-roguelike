using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class mov : MonoBehaviour
{
    public Camera cam;
    // This speed is little too high, It'd greate to decrease it
    public float velocidade = 20.0f;
    // Also too high
    public float SteerSpeed = 360.0f;

    public int Gap = 10;
    public GameObject BodyPrefab;

    // tiro
    public GameObject projectilePrefab;
    public float projectileSpeed = 50f;
    public float fireCooldown = 0.3f;
    private float fireTimer = 0f;

    // câmera
    public Vector3 cameraLocalOffset = new Vector3(0f, 15f, -10f);

    // corpo da cobra
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionHistory = new List<Vector3>();

    // ---- UPGRADES ----
    int hydraLevel;      // mais projéteis
    int armorLevel;      // “vidas” de blindado
    int plasmaLevel;     // boost de velocidade ao atirar
    float plasmaBoostTimer = 0f;


    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        // puxa níveis do GameManager
        if (GameManager.Instance != null)
        {
            hydraLevel = GameManager.Instance.hydraLevel;
            armorLevel = GameManager.Instance.armoredBodyLevel;
            plasmaLevel = GameManager.Instance.plasmaEngineLevel;
        }

        // cria segmentos iniciais
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
    }


    void Update()
    {
        // -------- movimento seguindo o mouse --------
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        float hitDist;
        Vector3 targetPos = transform.position;

        if (groundPlane.Raycast(ray, out hitDist))
            targetPos = ray.GetPoint(hitDist);

        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        float dist = toTarget.magnitude;
        Vector3 moveDir = Vector3.zero;

        // velocidade base
        float currentSpeed = velocidade;

        // boost de plasma
        if (plasmaLevel > 0 && plasmaBoostTimer > 0f)
        {
            plasmaBoostTimer -= Time.deltaTime;
            currentSpeed *= 1f + 0.3f * plasmaLevel;   // ajusta o multiplicador se quiser
        }

        if (dist > 0.1f)
        {
            moveDir = toTarget.normalized;

            transform.position += moveDir * currentSpeed * Time.deltaTime;

            if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    SteerSpeed * Time.deltaTime
                );
            }
        }

        // -------- trilha do corpo --------
        PositionHistory.Insert(0, transform.position);

        int index = 0;
        foreach (var body in BodyParts)
        {
            int historyIndex = Mathf.Min(index * Gap, PositionHistory.Count - 1);
            Vector3 point = PositionHistory[historyIndex];
            body.transform.position = point;
            index++;
        }

        // -------- câmera --------
        Vector3 offsetMundo = transform.TransformDirection(cameraLocalOffset);
        cam.transform.position = transform.position + offsetMundo;
        cam.transform.LookAt(transform.position);

        // -------- tiro --------
        HandleShooting();
    }


    // ===================== TIRO / HYDRA / PLASMA =====================

    private void HandleShooting()
    {
        fireTimer -= Time.deltaTime;

        bool holdLeft = Input.GetMouseButton(0);      // tiro contínuo
        bool clickRight = Input.GetMouseButtonDown(1); // tiro único

        bool wantsToShoot = holdLeft || clickRight;

        if (!wantsToShoot || fireTimer > 0f || projectilePrefab == null)
            return;

        fireTimer = fireCooldown;

        Vector3 forward = transform.forward;

        // tiro central
        ShootProjectile(forward);

        // HYDRA: mais projéteis em leque
        if (hydraLevel >= 1)
        {
            float angle = 15f;
            ShootProjectile(Quaternion.AngleAxis(angle, Vector3.up) * forward);
            ShootProjectile(Quaternion.AngleAxis(-angle, Vector3.up) * forward);
        }
        if (hydraLevel >= 2)
        {
            float angle = 30f;
            ShootProjectile(Quaternion.AngleAxis(angle, Vector3.up) * forward);
            ShootProjectile(Quaternion.AngleAxis(-angle, Vector3.up) * forward);
        }

        // PLASMA: dá um boost de velocidade por um tempo
        if (plasmaLevel > 0)
        {
            plasmaBoostTimer = 0.15f + 0.05f * plasmaLevel;
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        Vector3 spawnPos = transform.position + direction.normalized * 1.5f;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rot);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * projectileSpeed;
        }

        Destroy(proj, 5f);
    }


    // ===================== CORPO DA COBRA =====================

    private void GrowSnake()
    {
        if (BodyPrefab == null) return;

        GameObject body = Instantiate(BodyPrefab);

        int index = BodyParts.Count + 1;
        float spacing = 0.5f;

        body.transform.position = transform.position - transform.forward * index * spacing;
        body.transform.rotation = transform.rotation;

        BodyParts.Add(body);
    }


    // ===================== COLISÕES (MOEDA, PAREDE, INIMIGO) =====================

    private void OnTriggerEnter(Collider other)
    {
        // moeda
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            GrowSnake();

            if (GameManager.Instance != null)
                GameManager.Instance.AddCoins(1);
        }
        // parede ou inimigo
        else if (other.CompareTag("Wall") || other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
        {
            // se tiver blindado, gasta um "escudo"
            if (armorLevel > 0 && BodyParts.Count > 0)
            {
                armorLevel--;

                GameObject last = BodyParts[BodyParts.Count - 1];
                BodyParts.RemoveAt(BodyParts.Count - 1);
                Destroy(last);

                // não dá game over ainda
                return;
            }

            // sem blindado -> morre
            SceneManager.LoadScene("gameover");
        }
    }

    // chamado por tiros de inimigo
    public void TakeHitFromProjectile()
    {
        // usa o mesmo esquema do armorLevel e dos segmentos
        if (armorLevel > 0 && BodyParts.Count > 0)
        {
            armorLevel--;

            GameObject last = BodyParts[BodyParts.Count - 1];
            BodyParts.RemoveAt(BodyParts.Count - 1);
            Destroy(last);
        }
        else
        {
            // sem blindado -> morre
            SceneManager.LoadScene("gameover");
        }
    }


}
