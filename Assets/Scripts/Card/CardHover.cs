using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class CardHover : MonoBehaviour
{   
    [HideInInspector]
    public GameObject controllerObject;
    
    [HideInInspector]
    public StartGame controller;
    public bool cardActive = true;
    public bool cardTop = false;
    public bool isHoverable = true;
    public EffectList effectList;
    
    public void Start()
    {      
        
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();

        //Inicializar efectos de las cartas
        InitEffectList();
    }

    //Efectos
    void InitEffectList()
    {
        effectList.setEffect("Aumenta en 2 el poder de las cartas de la fila donde es colocada");
        effectList.setEffect("Aumenta en 1 el poder de las cartas de la fila donde es colocada");
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
        
        effectList.setEffect("Carta creada, sin efecto");
    }
    
    //Desplazar carta cuando esta en hover
    public void OnHoverEnter()
    {   
        if(isHoverable)
            ShowInfo();
        if(gameObject != controller.selectedCard)
        {
            if (cardActive && !cardTop)
            {   
                TranslateUp();
            }
        } 
    }

    //Desplazar carta cuando esta en hover
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
    }

    //Trazladar hacia arriba
    public void TranslateUp()
    {
        transform.Translate(0, 30, 0);
        cardTop = true;
    }
    
    //Trazladar hacia abajo
    public void TranslateDown()
    {
        transform.Translate(0, -30, 0);
        cardTop = false;
    }

    //Mostrar informacion de la carta en hover
    void ShowInfo()
    {   
        controller.showCard.SetActive(true);
        CardStats card = transform.Find("Stats").GetComponent<CardStats>();
        //Image
        controller.showCard.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/Small imgs/" + card.id.ToString());
        controller.showCard.transform.Find("Image").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        //Type
        if (card.type == Card.Type.Golden)
        {
            controller.showCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/goldCard");
            controller.showCard.transform.Find("Type").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.type == Card.Type.Silver)
        { 
            controller.showCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/silverCard");
            controller.showCard.transform.Find("Type").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else {
            controller.showCard.transform.Find("Type").GetComponent<Image>().sprite = null;
            controller.showCard.transform.Find("Type").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }  
        //Power
        if (card.type == Card.Type.Golden)
        {
            controller.showCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
            controller.showCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255);
        } else if (card.type == Card.Type.Silver)
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
        controller.showCard.transform.Find("CardType").GetComponent<TextMeshProUGUI>().text = Tools.GetCardTypeString(card.type);
    }

    //Ocultar informacion de la carta
    void HideInfo()
    {
        controller.showCard.SetActive(false);
    }
}