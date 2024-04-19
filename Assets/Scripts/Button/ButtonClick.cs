// using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
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

    public void OnButtonClick()
    {   
        if(controller.selectedCard != null) 
        {   
            controller.selectedCard.transform.GetComponent<CardHover>().cardTop = false;
            controller.selectedCard.transform.Translate(0, -30, 0);
            controller.selectedCard = null;
        }
        // Debug.Log("entra");
        if(transform.name == "Button1" && controller.gameState == GameState.PLAYER1)
        {   
            // Debug.Log(1);
            if (controller.playerMove == false)
            {
                controller.gameState = GameState.PLAYER1PASS;
            } else
            {
                controller.gameState = GameState.PLAYER2;
            }
            controller.playerMove = false;
            
            controller.button1.GetComponent<ButtonHover>().isHoverable = false;
            controller.button2.GetComponent<ButtonHover>().isHoverable = true;
            controller.button1.transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            controller.button1.transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            if(controller.playerMove == false)
            controller.UpdateStats();
        } else if (transform.name == "Button2" && controller.gameState == GameState.PLAYER2)
        {   
            // Debug.Log(2);
            if (controller.playerMove == false)
            {
                controller.gameState = GameState.PLAYER2PASS;
            } else
            {
                controller.gameState = GameState.PLAYER1;
            }
            controller.playerMove = false;

            controller.button1.GetComponent<ButtonHover>().isHoverable = true;
            controller.button2.GetComponent<ButtonHover>().isHoverable = false;
            controller.button2.transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            controller.button2.transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            controller.UpdateStats();
        } else if(transform.name == "Button2" && controller.gameState == GameState.PLAYER1PASS)
        {
            controller.CloseRound();
        }else if(transform.name == "Button1" && controller.gameState == GameState.PLAYER2PASS)
        {
            controller.CloseRound();
        }
    }

}
