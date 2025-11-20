using UnityEngine;
using System.Collections.Generic;

public class mov : MonoBehaviour
{
    public Camera cam;
    public float velocidade = 20.0f;
    public float SteerSpeed = 180.0f;

    public int Gap = 10; // dist�ncia entre posi��es da trilha
    public GameObject BodyPrefab;

    //  Configura��o de tiro
    public GameObject projectilePrefab;   // arraste o prefab aqui
    public float projectileSpeed = 50f;   // velocidade do proj�til
    public float fireCooldown = 0.3f;     // tempo entre tiros (segundos)
    private float fireTimer = 0f;

    //  Config da c�mera a�rea
    public Vector3 cameraLocalOffset = new Vector3(0f, 15f, -10f);

    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionHistory = new List<Vector3>();


    void Start()
    {
        cam = Camera.main;

        // Criar 5 segmentos iniciais j� posicionados
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
    }


    void Update()
    {
        // Movimento da cabe�a
        transform.position += transform.forward * velocidade * Time.deltaTime;

        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);

        // Grava a posi��o atual no hist�rico
        PositionHistory.Insert(0, transform.position);

        // Atualiza a posi��o de cada parte do corpo
        int index = 0;
        foreach (var body in BodyParts)
        {
            int historyIndex = Mathf.Min(index * Gap, PositionHistory.Count - 1);
            Vector3 point = PositionHistory[historyIndex];
            body.transform.position = point;

            index++;
        }

        //  C�mera a�rea seguindo a cobra
        Vector3 offsetMundo = transform.TransformDirection(cameraLocalOffset);
        cam.transform.position = transform.position + offsetMundo;
        cam.transform.LookAt(transform.position);

        // L�gica de tiro
        HandleShooting();
    }


    private void HandleShooting()
    {
        // Atualiza timer do cooldown
        fireTimer -= Time.deltaTime;

        // Atira quando apertar espa�o E cooldown acabou
        if (Input.GetKeyDown(KeyCode.Space) && fireTimer <= 0f)
        {
            fireTimer = fireCooldown;

            // Posi��o de spawn: um pouco na frente da cabe�a
            Vector3 spawnPos = transform.position + transform.forward * 1.5f;

            // Instancia o proj�til
            GameObject proj = Instantiate(projectilePrefab, spawnPos, transform.rotation);

            // D� velocidade pro proj�til se tiver Rigidbody
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * projectileSpeed;
            }

            // (Opcional) destruir depois de um tempo pra n�o acumular
            Destroy(proj, 5f);
        }
    }


    private void GrowSnake()
    {
        GameObject body = Instantiate(BodyPrefab);

        // Posiciona o corpo inicial para n�o nascer empilhado
        int index = BodyParts.Count + 1;
        float spacing = 0.5f; // ajuste se quiser mais dist�ncia entre partes

        body.transform.position = transform.position - transform.forward * index * spacing;
        body.transform.rotation = transform.rotation;

        BodyParts.Add(body);
    }
}
