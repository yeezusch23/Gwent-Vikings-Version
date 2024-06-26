using UnityEngine;
using UnityEngine.Events;

public class RowClick : MonoBehaviour
{   
    [HideInInspector]
    public GameObject controllerObject;
  
    [HideInInspector]
    public StartGame controller;
    // Start is called before the first frame update
    UnityEvent unityEvent;
    void Start()
    {   
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();
    }

    //Metodo para colocar las cartas en las filas
    public void OnRowClick()
    {   
        if (controller.selectedCard != null)
        {     
            CardStats card = controller.selectedCard.transform.Find("Stats").GetComponent<CardStats>();
            if(card.row == "close" || card.row == "close" || card.row == "range" || card.row == "siege" || card.row == "all" || card.row == "close_range"){
                if((transform.parent.parent.name == "Player1" && card.faction == "Vikings") || (transform.parent.parent.name == "Player2" && card.faction == "Last Kingdom"))
                {   
                    bool ok = false;
                    if (transform.parent.name == card.row)
                    {   
                        controller.selectedCard.transform.GetComponent<CardHover>().cardActive = false;
                        controller.selectedCard.transform.GetComponent<CardSelect>().isSelectable = false;
                        controller.selectedCard.transform.SetParent(transform, false); 
                        controller.selectedCard = null;
                        ok = true;
                    } else if (card.row == "close_range" && (transform.parent.name == "close" || transform.parent.name == "range"))
                    {   
                        controller.selectedCard.transform.GetComponent<CardHover>().cardActive = false;
                        controller.selectedCard.transform.GetComponent<CardSelect>().isSelectable = false;
                        controller.selectedCard.transform.SetParent(transform, false);
                        controller.selectedCard = null; 
                        ok = true;
                    } else if (card.row == "all")
                    {
                        controller.selectedCard.transform.GetComponent<CardHover>().cardActive = false;
                        controller.selectedCard.transform.GetComponent<CardSelect>().isSelectable = false;
                        controller.selectedCard.transform.SetParent(transform, false); 
                        controller.selectedCard = null;
                        ok = true;
                    }
                    if(ok){
                        controller.ResetField(1);
                        controller.ResetField(2);
                        controller.ActiveEffect(transform);
                        controller.UpdateClimaEffects(transform);
                        if(controller.gameState == GameState.PLAYER1 || controller.gameState == GameState.PLAYER2)
                            controller.playerMove = true;
                        controller.UpdateStats();
                    }
                } 
            }
        }

    }

   
}
