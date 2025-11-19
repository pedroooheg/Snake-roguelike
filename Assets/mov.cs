using UnityEngine;
using System.Collections.Generic;

public class mov : MonoBehaviour
{
    public Camera cam;
    public float velocidade = 5.0f;
    public float SteerSpeed = 180.0f;

    public int Gap = 10; // distância entre posições da trilha
    public GameObject BodyPrefab;

    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionHistory = new List<Vector3>();


    void Start()
    {
        cam = Camera.main;

        // Criar 4 segmentos iniciais já posicionados
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
    }


    void Update()
    {
        // Movimento da cabeça
        transform.position += transform.forward * velocidade * Time.deltaTime;

        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);

        // Grava a posição atual no histórico
        PositionHistory.Insert(0, transform.position);

        // Atualiza a posição de cada parte do corpo
        int index = 0;
        foreach (var body in BodyParts)
        {
            int historyIndex = Mathf.Min(index * Gap, PositionHistory.Count - 1);
            Vector3 point = PositionHistory[historyIndex];
            body.transform.position = point;

            index++;
        }

        // Camera look
        Vector3 offset = transform.TransformDirection(new Vector3(-0.5f, 1.0f, -3.5f));
        cam.transform.position = transform.position + offset;
        cam.transform.LookAt(transform);
    }


    private void GrowSnake()
    {
        GameObject body = Instantiate(BodyPrefab);

        // Posiciona o corpo inicial para não nascer empilhado
        int index = BodyParts.Count + 1;
        float spacing = 0.5f; // ajuste se quiser mais distância entre partes

        body.transform.position = transform.position - transform.forward * index * spacing;
        body.transform.rotation = transform.rotation;

        BodyParts.Add(body);
    }
}
