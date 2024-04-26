using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CardHover : MonoBehaviour
{   
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;
    public bool cardActive = true;
    public bool cardTop = false;
    public bool isHoverable = true;
    // public bool isLeader = false;
    // public GameObject showCard;

    public EffectList effectList;
    
    // public GameObject bigCardPrefab;
    // public StartGame showInfo;
    // public GameObject cardStats;
    public void Start()
    {      
        
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();

        InitEffectList();
        // controller.showCard.SetActive(true);
        // controller.showCard = GameObject.Find("showCard");
        // controller.showCardText = GameObject.Find("Effect");
        // controller.showCard.GetComponent<Image> ().enabled = false;
        // controller.showCardText.GetComponent<TextMeshProUGUI> ().enabled = false;
        // controller.showCard.SetActive(false);
    }

        void InitEffectList()
    {
        effectList.setEffect("Aumenta en 2 el poder de las cartas de la fila donde es colocada, luego es enviada al cementerio");
        effectList.setEffect("Aumenta en 1 el poder de las cartas de la fila donde es colocada, luego es enviada al cementerio");
        effectList.setEffect("Reduce en 1 la fuerza de todas las unidades cuerpo a cuerpo en el campo de batalla");
        effectList.setEffect("Reduce en 1 la potencia de los ataques a distancia de todas las unidades en el campo de batalla");
        effectList.setEffect("Despeja el clima eliminando todas las cartas de tipo clima");
        effectList.setEffect("Robar una carta");
        effectList.setEffect("Robar una carta");
        effectList.setEffect("Eliminar la carta con menos poder del rival");
        effectList.setEffect("Eliminar la carta con menos poder del rival");
        effectList.setEffect("Eliminar la carta con mas poder del campo rival");
        effectList.setEffect("Aumenta en 1 el poder las cartas adyacentes");
        effectList.setEffect("Caclula el promedio de poder entre todas las cartas del campo del rival ([parte entera]). Luego iguala el poder de todas las cartas a ese promedio");
        effectList.setEffect("Limpia la fila del campo (no vacia) del rival con menos unidades");
        effectList.setEffect("Multiplica por n su ataque, siendo n la cantidad de cartas iguales a ella en el campo");
        effectList.setEffect("Permite colocar una carta con poder 0 en el lugar de una carta del campo para regresar esta a la mano");
    }
      public void OnHoverEnter()
    {   
        if(isHoverable)
            ShowInfo();
        // Debug.Log("ENTER    cardActive: " + cardActive + " cardTop:" + cardTop);
        if(gameObject != controller.selectedCard)
        {
            if (cardActive && !cardTop)
            {   
                TranslateUp();
            }
        } 
    }

    public void OnHoverExit()
    {   
        if(isHoverable)
            HideInfo();
        if(gameObject != controller.selectedCard)
        {
            if (cardActive && cardTop)
            {
                TranslateDown();
            }
        }
        
        // Debug.Log("EXIT   cardActive: " + cardActive + " cardTop:" + cardTop);
        //Debug.Log("Hover Exit");
    }

    public void TranslateUp()
    {
        transform.Translate(0, 30, 0);
        cardTop = true;
    }
    public void TranslateDown()
    {
        transform.Translate(0, -30, 0);
        cardTop = false;
    }

    void ShowInfo()
    {   
        
        // Debug.Log(controller.showCard);
        // controller.showCard.SetActive(true);   
        // controller.showCard.GetComponent<Image>().enabled = true;
        // controller.showCardText.GetComponent<TextMeshProUGUI> ().enabled = true;
        // Debug.Log(controller.showCard.transform.Find("Image"));
        controller.showCard.SetActive(true);
        CardStats card = transform.Find("Stats").GetComponent<CardStats>();
        //Image
        controller.showCard.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/Small imgs/" + card.id.ToString());
        controller.showCard.transform.Find("Image").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        //Type
        if (card.type == "Oro")
        {
            controller.showCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/goldCard");
            controller.showCard.transform.Find("Type").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.type == "Plata")
        { 
            controller.showCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/silverCard");
            controller.showCard.transform.Find("Type").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else {
            controller.showCard.transform.Find("Type").GetComponent<Image>().sprite = null;
            controller.showCard.transform.Find("Type").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }  
        //Power
        if (card.type == "Oro")
        {
            controller.showCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
            controller.showCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255);
        } else if (card.type == "Plata")
        { 
            controller.showCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
            controller.showCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0);
        } else {
            controller.showCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = "";
        }     
        //Row
        if (card.row == "close")
        {
            controller.showCard.transform.Find("Row").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/close");
            controller.showCard.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.row == "range")
        {
            controller.showCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/range");
            controller.showCard.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.row == "siege") 
        {
            controller.showCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/siege");
            controller.showCard.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.row == "close_range")
        {
            controller.showCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/close_range");
            controller.showCard.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else 
        {
            controller.showCard.transform.Find("Row").GetComponent<Image>().sprite = null;
            controller.showCard.transform.Find("Row").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        //Effect
        controller.showCard.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = effectList.effects[card.effect];
        //Name
        controller.showCard.transform.Find("CardName").GetComponent<TextMeshProUGUI>().text = card.name;
        //Card Type
        controller.showCard.transform.Find("CardType").GetComponent<TextMeshProUGUI>().text = card.type;



    }

    void HideInfo()
    {
        controller.showCard.SetActive(false);
    }
}