using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuReset : MonoBehaviour
{    
    [HideInInspector]
    public GameObject controllerObject;
  
    [HideInInspector]
    public StartGame controller;
    void Start()
    {
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();    
    }
    public void OnReset()
    {   
        //Reiniciar partida
        SceneManager.LoadScene("Game");
    }
    public void OnMenuInicial()
    {   
        //cargar Menu Inicial
        SceneManager.LoadScene("MenuInicial");
    }

    public void Exit()
    {   
        //Salir
        Debug.Log("Salir...");
        Application.Quit();
    }
    
}
