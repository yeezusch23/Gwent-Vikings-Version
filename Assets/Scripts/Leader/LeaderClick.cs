using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;


public class LeaderClick : MonoBehaviour
{   
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;
    public bool leaderActive = false;
    public Sprite imgActive;
    // Start is called before the first frame update
    void Start()
    {
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();
    }

    public void OnLeaderClick()
    {
        if(!leaderActive && !controller.playerMove) 
        {   
            if(transform.parent.parent.name == "Player1")
            {
                controller.AddCardDeck("Vikings");
            } else {
                controller.AddCardDeck("Last Kingdom");
            }
            transform.GetComponent<Image>().sprite = imgActive;
            if(controller.gameState == GameState.PLAYER1 || controller.gameState == GameState.PLAYER2)
                controller.playerMove = true;
            
            controller.UpdateStats();
            leaderActive = true;
        }   
    }
}
