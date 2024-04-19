using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{   
    public bool isHoverable = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnHoverEnter()
    {   
        if (isHoverable){
            transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
            transform.GetComponent<Image>().color = new Color(0, 0, 0, 255);
        }
    }
    public void OnHoverExit()
    {   
        if(isHoverable)
        {
            transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
            transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
    }
}
