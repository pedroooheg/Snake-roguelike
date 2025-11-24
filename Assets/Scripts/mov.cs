using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class mov : MonoBehaviour
{
    public Camera cam;
    public float velocidade = 20.0f;

    public float SteerSpeed = 360.0f;

    public int Gap = 10;
    public GameObject BodyPrefab;

    public GameObject projectilePrefab;
    public float projectileSpeed = 50f;
    public float fireCooldown = 0.3f;
    private float fireTimer = 0f;

    public Vector3 cameraLocalOffset = new Vector3(0f, 15f, -10f);

    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionHistory = new List<Vector3>();


    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
    }


    void Update()
    {
        // pega posição do mouse
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

        if (dist > 0.1f)
        {
            moveDir = toTarget.normalized;

            transform.position += moveDir * velocidade * Time.deltaTime;

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

        PositionHistory.Insert(0, transform.position);

        int index = 0;
        foreach (var body in BodyParts)
        {
            int historyIndex = Mathf.Min(index * Gap, PositionHistory.Count - 1);
            Vector3 point = PositionHistory[historyIndex];
            body.transform.position = point;
            index++;
        }

        Vector3 offsetMundo = transform.TransformDirection(cameraLocalOffset);
        cam.transform.position = transform.position + offsetMundo;
        cam.transform.LookAt(transform.position);

        HandleShooting();
    }



    private void HandleShooting()
    {
        fireTimer -= Time.deltaTime;

        bool holdLeft = Input.GetMouseButton(0);
        bool clickRight = Input.GetMouseButtonDown(1);

        bool wantsToShoot = holdLeft || clickRight;

        if (wantsToShoot && fireTimer <= 0f && projectilePrefab != null)
        {
            fireTimer = fireCooldown;

            Vector3 spawnPos = transform.position + transform.forward * 1.5f;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, transform.rotation);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = transform.forward * projectileSpeed;

            Destroy(proj, 5f);
        }
    }



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



    private void OnTriggerEnter(Collider other)
    {
        // coleta moeda
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            GrowSnake();
        }

        // bateu na parede
        if (other.CompareTag("Wall"))
        {
            SceneManager.LoadScene("gameover");
        }
    }
}
