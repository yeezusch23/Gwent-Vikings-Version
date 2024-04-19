using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CardHover : MonoBehaviour
{   
    [HideInInspector]
    public GameObject Node;
    
    [HideInInspector]
    public GameObject NodeText;
    
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;
    public bool cardActive = true;
    public bool cardTop = false;

    public bool isHoverable = true;
    public GameObject showCard;

    public EffectList effectList;
    
    // public GameObject bigCardPrefab;
    // public StartGame showInfo;
    // public GameObject cardStats;
    public void Start()
    {      
        
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();

        InitEffectList();
        // Node.SetActive(true);
        Node = GameObject.Find("showCard");
        NodeText = GameObject.Find("Effect");
        Node.GetComponent<Image> ().enabled = false;
        NodeText.GetComponent<TextMeshProUGUI> ().enabled = false;
        // Node.SetActive(false);
    }

        void InitEffectList()
    {
        effectList.setEffect("Aumenta en 2 el poder de las cartas de la fila donde es colocada");
        effectList.setEffect("Aumenta en 1 el poder de las cartas de la fila donde es colocada");
        effectList.setEffect("Reduce en 1 la fuerza de todas las unidades cuerpo a cuerpo en el campo de batalla");
        effectList.setEffect("Reduce en 1 la potencia de los ataques a distancia de todas las unidades en el campo de batalla");
        effectList.setEffect("Despeja el clima eliminando todas las cartas de tipo clima");
        effectList.setEffect("Aumenta en 2 el poder de una carta");
        effectList.setEffect("Robar una carta");
        effectList.setEffect("Eliminar la carta con menos poder del rival");
        effectList.setEffect("Reduce en 2 el poder de una carta");
        effectList.setEffect("Eliminar la carta con mas poder del campo rival");
        effectList.setEffect("Aumenta en 1 el poder las cartas adyacentes");
        effectList.setEffect("Caclula el promedio de poder entre todas las cartas del campo del rival. Luego iguala el poder de todas las cartas del campo del rival a ese promedio.");
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
        DisableShowCard();
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

    public void ShowInfo()
    {   
        
        // Debug.Log(Node);
        Node.SetActive(true);
        Node.GetComponent<Image> ().enabled = true;
        // Debug.Log(Node.transform.Find("Image"));
        CardStats card = transform.Find("Stats").GetComponent<CardStats>();
        //Image
        Node.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/Small imgs/" + card.id.ToString());
        Node.transform.Find("Image").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        //Type
        if (card.type == "Oro")
        {
            Node.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/goldCard");
            Node.transform.Find("Type").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.type == "Plata")
        { 
            Node.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/silverCard");
            Node.transform.Find("Type").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else {
            Node.transform.Find("Type").GetComponent<Image>().sprite = null;
            Node.transform.Find("Type").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }  
        //Power
        if (card.type == "Oro")
        {
            Node.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
            Node.transform.Find("Power").GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255);
        } else if (card.type == "Plata")
        { 
            Node.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
            Node.transform.Find("Power").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0);
        } else {
            Node.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = "";
        }     
        //Row
        if (card.row == "close")
        {
            Node.transform.Find("Row").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/close");
            Node.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.row == "range")
        {
            Node.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/range");
            Node.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.row == "siege") 
        {
            Node.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/siege");
            Node.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else if (card.row == "close_range")
        {
            Node.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/close_range");
            Node.transform.Find("Row").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        } else 
        {
            Node.transform.Find("Row").GetComponent<Image>().sprite = null;
            Node.transform.Find("Row").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        //Effect
        NodeText.GetComponent<TextMeshProUGUI>().enabled = true;
        Node.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = effectList.effects[card.effect];
    }

    
    public void DisableShowCard()
    {
        Node.SetActive(false);
    }
}