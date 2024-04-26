using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuReset : MonoBehaviour
{   
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;
    // Start is called before the first frame update
    void Start()
    {
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();   
    }

    public void OnMenuReset()
    {   
        //Reiniciar partida
        controller.ResetGame();
        
        //Restablecer los colores del boton de volver a jugar
        transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
    }
}
