using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimaClick : MonoBehaviour
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

    //Metodo para colocar las cartas clima
    public void OnClimaClick()
    {
        if (controller.selectedCard != null)
        {
            CardStats card = controller.selectedCard.transform.Find("Stats").GetComponent<CardStats>();
            if(card.row == "Clima"){
                if(card.row == "Clima")
                {
                    controller.selectedCard.transform.GetComponent<CardHover>().cardActive = false;
                    controller.selectedCard.transform.GetComponent<CardSelect>().isSelectable = false;
                    controller.selectedCard.transform.SetParent(transform, false); 
                    controller.selectedCard = null;
                }   
                
                controller.ResetField(1);
                controller.ResetField(2);
                controller.ActiveEffect(transform, card);
                if(controller.gameState == GameState.PLAYER1 || controller.gameState == GameState.PLAYER2)
                        controller.playerMove = true;
                controller.UpdateStats();
            }
        }   
    }
}
