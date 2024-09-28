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

    public Card GetCardHand(CardStats cardStats, string player)
    {
        if(player == "Player2")
        {
            foreach(Card card in controller.lastKingdomHand.cards)
                if(cardStats.id == card.id)
                    return card;
        } else 
        {
            foreach(Card card in controller.vikingsHand.cards)
                if(cardStats.id == card.id)
                    return card;
        }
        return null;
    }

    //Metodo para colocar las cartas en las filas
    public void OnRowClick()
    {   
        // Debug.Log(controller.selectedCard);
        if (controller.selectedCard != null)
        {     

            CardStats card = controller.selectedCard.transform.Find("Stats").GetComponent<CardStats>();
            if(card.row == "close" || card.row == "range" || card.row == "siege" || card.row == "all" || card.row == "close_range"
            || card.row == "close_siege" || card.row == "range_siege" ){
                
       
                if((transform.parent.parent.name == "Player1") || (transform.parent.parent.name == "Player2"))
                {   
                    
                    // Debug.Log("Entra");
                    bool ok = false;
                    if (transform.parent.name == card.row)
                    {   
                        ok = true;
                    } else if (card.row == "close_range" && (transform.parent.name == "close" || transform.parent.name == "range"))
                    {   
                        ok = true;
                    } else if (card.row == "close_siege" && (transform.parent.name == "close" || transform.parent.name == "siege"))
                    {   
                        controller.selectedCard = null; 
                        ok = true;
                    } else if (card.row == "range_siege" && (transform.parent.name == "siege" || transform.parent.name == "range"))
                    {   
                        controller.selectedCard = null; 
                        ok = true;
                    } else if (card.row == "all")
                    {
                        ok = true;
                    }
                    if (ok){
                        controller.ActiveEffect(transform, card);
                    }

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
                    } else if (card.row == "close_siege" && (transform.parent.name == "close" || transform.parent.name == "siege"))
                    {   
                        controller.selectedCard.transform.GetComponent<CardHover>().cardActive = false;
                        controller.selectedCard.transform.GetComponent<CardSelect>().isSelectable = false;
                        controller.selectedCard.transform.SetParent(transform, false);
                        controller.selectedCard = null; 
                        ok = true;
                    } else if (card.row == "range_siege" && (transform.parent.name == "siege" || transform.parent.name == "range"))
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
