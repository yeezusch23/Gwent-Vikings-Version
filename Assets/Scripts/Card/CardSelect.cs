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
        if(controller.selectedCard != null)
        {
            controller.selectedCard.transform.Translate(0, -30, 0);
        } 

        if(controller.selectedCard == null)
            controller.selectedCard = gameObject;
        else
        {
            controller.selectedCard = gameObject;
            gameObject.transform.Translate(0, 30, 0);
        }
        
        if(controller.gameState == GameState.PLAYER1)
        {
            controller.ResetField(1);
            ActivatedField(1);
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
        
        CardStats card = gameObject.GetComponent<CardStats>();
        Debug.Log(card);
        Sprite specialSprite = playerField.GetComponent<RowInfo>().special;
        Sprite spriteClose = player1Field.GetComponent<RowInfo>().closeSelected;
        // if(card.row == "close_range") 
        // {
        //     playerField.transform.Find("close").Find("row").GetComponent<Image>().sprite = spriteClose;
        // }

        // if (card.row == "close")
        // {   
            
        // } else if (card.row == "range")
        // {
        //     instantiateCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/range");
        // } else if (card.row == "siege") 
        // {
        //     instantiateCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/siege");
        // } else if (card.row == "close_range")
        // {
        //     instantiateCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/close_range");
        // } else 
        // {
        //     Destroy(instantiateCard.transform.Find("Row").GetComponent<Image>());
        // }
        
        // // Restaurando los special Sprites de los campos, Tambien se puede usar la variante Resources.Load<Sprite>("path");
        // Sprite specialSprite = playerField.GetComponent<RowInfo>().special;
        // playerField.transform.Find("close").Find("special").GetComponent<Image>().sprite = specialSprite;
        // playerField.transform.Find("range").Find("special").GetComponent<Image>().sprite = specialSprite;
        // playerField.transform.Find("siege").Find("special").GetComponent<Image>().sprite = specialSprite;

        // // Restaurando las rows de los campos, Tambien se puede usar la variante Resources.Load<Sprite>("path");
        // Sprite rowSprite = playerField.GetComponent<RowInfo>().close;
        // playerField.transform.Find("close").Find("row").GetComponent<Image>().sprite = rowSprite;
        // rowSprite = playerField.GetComponent<RowInfo>().range;
        // playerField.transform.Find("range").Find("row").GetComponent<Image>().sprite = rowSprite;
        // rowSprite = playerField.GetComponent<RowInfo>().siege;
        // playerField.transform.Find("siege").Find("row").GetComponent<Image>().sprite = rowSprite;

        // // Light off the weather board and deselect weather card
        // // climaField.GetComponent<Image>().sprite = climaField.GetComponent<WeatherManager>().weather;
        // // climaField.GetComponent<WeatherManager>().isWeatherCard = false;
    }

    // Check if the player selected a new card, if yes deselect it else select the appropriate card
    private bool CheckSameCard()
    {
        // if (controller.selectedCard != null)
        // {
        //     if (controller.selectedCard == gameObject)//Same card
        //     {
        //         //gameObject.GetComponent<CardHover>().isCardUp = true; // Deprecated
        //         controller.selectedCard = null;
        //         //sameCard = true;
        //         return true;
        //     }
        //     else
        //     {
        //         controller.selectedCard.gameObject.GetComponent<CardHover>().isHoverable = true;
        //         controller.selectedCard.gameObject.GetComponent<CardHover>().TranslateDown();
        //         controller.selectedCard = null;
        //         return false;
        //         //sameCard = false;
        //     }
        // }
        return false;
    }
}
