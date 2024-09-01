using UnityEngine;

public class LongPress : MonoBehaviour
{   
    GameObject controllerObject;
    StartGame controller;
    void Start()
    {
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();
    }

    //Cambiar cartas al incio del juego
    public void OnLongPress()
    {
        
        CardStats card = transform.Find("Stats").GetComponent<CardStats>();

        if(controller.roundCount < 2 && controller.changedCard < 2){
            if(controller.gameState == GameState.PLAYER1)
            {   
                controller.InstantiateCard(controller.vikingsDeck.GetCard(), "Vikings");
                controller.vikingsDeck.AddCard(new CardGame(card.name, card.id, card.faction, card.type, card.power, card.row, card.effect));
                controller.vikingsDeck.Shuffle();
                transform.parent = null;
                controller.changedCard++;
            }else{
                controller.InstantiateCard(controller.lastKingdomDeck.GetCard(), "Last Kingdom");
                controller.lastKingdomDeck.AddCard(new CardGame(card.name, card.id, card.faction, card.type, card.power, card.row, card.effect));
                controller.lastKingdomDeck.Shuffle();
                transform.parent = null;
                controller.changedCard++;
            }
            controller.UpdateStats();
        }
    }
}
