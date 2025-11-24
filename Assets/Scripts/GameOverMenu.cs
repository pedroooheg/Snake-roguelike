using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void Quit()
    {
        Application.Quit();
    }
}
