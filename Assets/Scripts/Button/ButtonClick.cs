using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonClick : MonoBehaviour
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
    //Botones de Pasar Turno
    public void OnButtonClick()
    {   
        //Colocar la carta en su posicion del mazo si pasa el turno y esta seleccionada  
        if(controller.selectedCard != null) 
        {   
            controller.selectedCard.transform.GetComponent<CardHover>().cardTop = false;
            controller.selectedCard.transform.Translate(0, -30, 0);
            controller.selectedCard = null;
        }
        
        //Si es el turno del jugador 1 y pasa el turno
        if(transform.name == "Button1" && controller.gameState == GameState.PLAYER1)
        {   
            if (controller.playerMove == false)
            {
                controller.gameState = GameState.PLAYER1PASS;
            } else
            {
                controller.gameState = GameState.PLAYER2;
            }
            controller.playerMove = false;

            //Restablecer los colores al boton            
            controller.button1.transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            controller.button1.transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            if(controller.playerMove == false)
            controller.UpdateStats();
            controller.roundCount++;
            controller.changedCard = 0;
        } else if (transform.name == "Button2" && controller.gameState == GameState.PLAYER2)
        {   
            //Si es el turno del jugador 2 y pasa el turno
            if (controller.playerMove == false)
            {
                controller.gameState = GameState.PLAYER2PASS;
            } else
            {
                controller.gameState = GameState.PLAYER1;
            }
            controller.playerMove = false;
            
            //Restablecer los colores al boton            
            controller.button2.transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            controller.button2.transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            controller.UpdateStats();
            controller.roundCount++;
            controller.changedCard = 0;
        } else if(transform.name == "Button2" && controller.gameState == GameState.PLAYER1PASS)
        {   
            //Restablecer los colores al boton            
            controller.button2.transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            controller.button2.transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            controller.CloseRound();
            controller.roundCount++;
            controller.changedCard = 0;
        }else if(transform.name == "Button1" && controller.gameState == GameState.PLAYER2PASS)
        {   
            //Restablecer los colores al boton            
            controller.button1.transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            controller.button1.transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            controller.CloseRound();
            controller.roundCount++;
            controller.changedCard = 0;
        }
    }

}
