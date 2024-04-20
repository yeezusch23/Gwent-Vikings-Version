using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{   
    
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;
    // public bool isHoverable = false;
    // Start is called before the first frame update
    void Start()
    {
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();
    }

    public void OnHoverEnter()
    {   
        bool ok = false;
        if(transform.name == "Button1" && controller.gameState == GameState.PLAYER1)
        {   
            ok = true;
        } else if (transform.name == "Button2" && controller.gameState == GameState.PLAYER2)
        {   ok = true;
        } else if(transform.name == "Button2" && controller.gameState == GameState.PLAYER1PASS)
        {   ok = true;
        }else if(transform.name == "Button1" && controller.gameState == GameState.PLAYER2PASS)
        {   ok = true;
        }
        if (ok){
            transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
            transform.GetComponent<Image>().color = new Color(0, 0, 0, 255);
        }
    }
    public void OnHoverExit()
    {   
        bool ok = false;
        if(transform.name == "Button1" && controller.gameState == GameState.PLAYER1)
        {   
            ok = true;
        } else if (transform.name == "Button2" && controller.gameState == GameState.PLAYER2)
        {   ok = true;
        } else if(transform.name == "Button2" && controller.gameState == GameState.PLAYER1PASS)
        {   ok = true;
        }else if(transform.name == "Button1" && controller.gameState == GameState.PLAYER2PASS)
        {   ok = true;
        }
        if (ok){
            transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
    }
}
