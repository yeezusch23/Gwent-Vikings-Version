using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClimaField : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public StartGame controller;

    public Sprite climaImg;
    public Sprite climsImgSelect;

    public bool isClimaCard;

    private void Start()
    {
        controllerObject = GameObject.Find("StartGame");
        controller = controllerObject.GetComponent<StartGame>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        if (controller.selectedCard != null && isClimaCard)
        {
            // controller.DirectlyPlaceCard(controller.selectedCard, gameObject, true);
        }
    }

    public void ClearClimaObject()
    {
        for (int i=0; i< gameObject.transform.childCount; i++)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }
        gameObject.transform.DetachChildren();
    }
}
