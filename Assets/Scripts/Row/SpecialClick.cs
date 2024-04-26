using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialClick : MonoBehaviour
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

    public void OnSpecialClick()
    {   
        if (controller.selectedCard != null)
        {
            CardStats card = controller.selectedCard.transform.Find("Stats").GetComponent<CardStats>();
            if(card.row == "Aumento"){
                if((transform.parent.parent.name == "Player1" && card.faction == "Vikings") || (transform.parent.parent.name == "Player2" && card.faction == "Last Kingdom"))
                {   
                    if(card.row == "Aumento" && transform.childCount == 0)
                    {
                        controller.selectedCard.transform.GetComponent<CardHover>().cardActive = false;
                        controller.selectedCard.transform.GetComponent<CardSelect>().isSelectable = false;
                        controller.selectedCard.transform.SetParent(transform, false); 
                        controller.selectedCard = null;
                    }   
                }
                controller.ResetField(1);
                controller.ResetField(2);
                controller.ActiveEffect(transform);
                if(controller.gameState == GameState.PLAYER1 || controller.gameState == GameState.PLAYER2)
                        controller.playerMove = true;
                controller.UpdateStats();
            }
        }   
    }
}
