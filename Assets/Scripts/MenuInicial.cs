using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuInicial : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void CreateCard()
    {   
        SceneManager.LoadScene("DSLCompile");
    }
    public void Exit()
    {   
        Debug.Log("Salir...");
        Application.Quit();
    }
}
