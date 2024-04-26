using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;

public class CardSelect : MonoBehaviour
{
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;

    public bool isSelectable = true;

    public GameObject player1Field;
    public GameObject player2Field;

    void Start()
    {   
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();
        player1Field = GameObject.Find("Player1");
        player2Field = GameObject.Find("Player2");
    }

    public void OnCardSelect()
    {   
        if(isSelectable && !controller.playerMove)
        {
            if(controller.selectedCard == null)
            {
                controller.selectedCard = gameObject;
                if(!controller.selectedCard.transform.GetComponent<CardHover>().cardTop)
                {
                    controller.selectedCard.transform.Translate(0, 30, 0);
                }
                controller.selectedCard.transform.GetComponent<CardHover>().cardTop = true;
            } else if (controller.selectedCard == gameObject)
            {   
                controller.selectedCard.transform.GetComponent<CardHover>().cardTop = false;
                controller.selectedCard.transform.Translate(0, -30, 0);
                controller.selectedCard = null;
            } else 
            {
                controller.selectedCard.transform.GetComponent<CardHover>().cardTop = false;
                controller.selectedCard.transform.Translate(0, -30, 0);
                controller.selectedCard = gameObject;
            }

            if(controller.selectedCard == null)
            {
                controller.ResetField(1);
                controller.ResetField(2);
            } 
            else if(controller.gameState == GameState.PLAYER1 || controller.gameState == GameState.PLAYER2PASS)
            {
                controller.ResetField(1);
                ActivatedField(1);
            } else if(controller.gameState == GameState.PLAYER2 || controller.gameState == GameState.PLAYER1PASS) {
                controller.ResetField(2);
                ActivatedField(2);
            }
        }
        if(controller.selectedCard != null){
            CardStats card = controller.selectedCard.transform.Find("Stats").GetComponent<CardStats>();
            string row = transform.parent.parent.name;
            GameObject hand;
            if (controller.gameState == GameState.PLAYER1 || controller.gameState == GameState.PLAYER2PASS)
                hand = controller.handVikings;
            else
                hand = controller.handLastKingdom;
            // Debug.Log(row);
            // Debug.Log(card.row);
            if (card.row == "Señuelo" && (row == "close" || row == "range" || row == "siege"))
            {
                // controller.selectedCard.transform.SetParent(transform, false);
                controller.selectedCard.transform.SetParent(transform.parent, false);
                transform.SetParent(hand.transform, false);
                
                controller.selectedCard.transform.GetComponent<CardHover>().cardActive = false;
                controller.selectedCard.transform.GetComponent<CardSelect>().isSelectable = false;

                transform.GetComponent<CardHover>().cardActive = true;
                transform.GetComponent<CardHover>().cardTop = false;
                transform.GetComponent<CardHover>().isHoverable = true;
                transform.GetComponent<CardSelect>().isSelectable = true;

                transform.Find("Frame").GetComponent<Image>().color = new Color(0, 255, 0, 0);
                transform.Find("Frame").GetComponent<Image>().color = new Color(0, 255, 0, 0);

                // transform.Translate(0, 30, 0);
                // cardTop = true;

                controller.selectedCard = null;
                controller.ResetField(1);
                controller.ResetField(2);
                controller.playerMove = true;
                // Debug.Log("CAMBIAR");
            }
            // Debug.Log("ENTRA");
        }
    }

    public void ActivatedField(int player) 
    {
        GameObject playerField;
        if (player == 1)
        {
            playerField = controller.player1;
        } else
        {
            playerField = controller.player2;
        }
        
        CardStats card = controller.selectedCard.transform.Find("Stats").GetComponent<CardStats>();
        
        if(card.type == "Señuelo")
        {   
            int childs = playerField.transform.Find("close").transform.Find("row").childCount;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = playerField.transform.Find("close").transform.Find("row").GetChild(i);
                child.transform.Find("Frame").transform.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
            childs = playerField.transform.Find("range").transform.Find("row").childCount;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = playerField.transform.Find("range").transform.Find("row").GetChild(i);
                child.transform.Find("Frame").transform.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
            childs = playerField.transform.Find("siege").transform.Find("row").childCount;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = playerField.transform.Find("siege").transform.Find("row").GetChild(i);
                child.transform.Find("Frame").transform.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
        }else{
            // Debug.Log(card.row);
            Sprite spriteClose = player1Field.GetComponent<RowInfo>().closeSelected;
            Sprite spriteRange = player1Field.GetComponent<RowInfo>().rangeSelected;
            Sprite spriteSiege = player1Field.GetComponent<RowInfo>().siegeSelected;
            Sprite spriteClima = player1Field.GetComponent<RowInfo>().climaSelected;
            Sprite spriteSpecial = player1Field.GetComponent<RowInfo>().specialSelected;
            if(card.row == "close_range") 
            {
                playerField.transform.Find("close").Find("row").GetComponent<Image>().sprite = spriteClose;
                playerField.transform.Find("range").Find("row").GetComponent<Image>().sprite = spriteRange;
            } else if (card.row == "Aumento") 
            {
                playerField.transform.Find("close").Find("special").GetComponent<Image>().sprite = spriteSpecial;
                playerField.transform.Find("range").Find("special").GetComponent<Image>().sprite = spriteSpecial;
                playerField.transform.Find("siege").Find("special").GetComponent<Image>().sprite = spriteSpecial;
            } else if (card.row == "all")
            {   
                playerField.transform.Find("close").Find("row").GetComponent<Image>().sprite = spriteClose;
                playerField.transform.Find("range").Find("row").GetComponent<Image>().sprite = spriteRange;   
                playerField.transform.Find("siege").Find("row").GetComponent<Image>().sprite = spriteSiege;   
            } else if (card.row == "Clima")
            {
                controller.climaField.GetComponent<Image>().sprite = spriteClima;
            } else if (card.row == "close")
            {
                playerField.transform.Find("close").Find("row").GetComponent<Image>().sprite = spriteClose;   
            } else if (card.row == "range")
            {
                playerField.transform.Find("range").Find("row").GetComponent<Image>().sprite = spriteRange;
            } else
            {
                playerField.transform.Find("siege").Find("row").GetComponent<Image>().sprite = spriteSiege;   
            }
        }
        
        
    }

}
