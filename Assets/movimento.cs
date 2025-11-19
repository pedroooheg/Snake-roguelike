using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Movimento : MonoBehaviour
{
    public float velocidade = 0.01f;
    public float rotate = 0.01f;
    public Camera cam;
    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), -rotate);
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), rotate);

        transform.position += transform.forward * velocidade;

        cam.transform.position = transform.position + new Vector3(-0.5f, 3.0f, -3.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            SceneManager.LoadScene("gameover");
        }
    }

}