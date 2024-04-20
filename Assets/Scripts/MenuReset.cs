using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuReset : MonoBehaviour
{   
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;
    // Start is called before the first frame update
    void Start()
    {
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();   
    }

    public void OnMenuReset()
    {
        controller.ResetGame();
    }
}
