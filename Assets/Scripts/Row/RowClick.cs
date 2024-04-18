using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class RowClick : MonoBehaviour
{   
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;
    // Start is called before the first frame update
    public GameObject cardPrefab;
    UnityEvent unityEvent;
    void Start()
    {   
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();
    }

    public void OnRowClick()
    {
        // Debug.Log("Player : " + transform.parent.parent.name + "-> Row: " + transform.parent.name);
        if (controller.selectedCard != null)
        {   
            CardStats card = controller.selectedCard.transform.Find("Stats").GetComponent<CardStats>();
            if((transform.parent.parent.name == "Player1" && card.faction == "Vikings") || (transform.parent.parent.name == "Player2" && card.faction == "Last Kingdom"))
            {
                if (transform.parent.name == card.row)
                {
                    controller.selectedCard.transform.SetParent(transform.parent, false); 
                } else if (card.row == "close_range" && (transform.parent.name == "close" || transform.parent.name == "range"))
                {
                    controller.selectedCard.transform.SetParent(transform.parent, false); 
                } else if (card.row == "all")
                {
                    controller.selectedCard.transform.SetParent(transform.parent, false); 
                }
            }
                
            // controller.selectedCard.transform.SetParent(transform.parent, false);
            
        }
    }

   
}
