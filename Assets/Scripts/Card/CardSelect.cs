using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelect : MonoBehaviour
{
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;

    public bool isSelectable;

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
        else if(controller.gameState == GameState.PLAYER1)
        {
            controller.ResetField(1);
            ActivatedField(1);
        } else {
            controller.ResetField(2);
            ActivatedField(2);
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
