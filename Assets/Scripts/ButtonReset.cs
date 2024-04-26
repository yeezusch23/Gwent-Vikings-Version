using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ButtonReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnHoverEnter()
    {   
        //Cambiar colores en hover
        transform.GetComponent<Image>().color = new Color(0, 0, 0, 255);
        transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
    }

    public void OnHoverExit()
    {   
        //Cambiar colores en hover
        transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        transform.Find("text").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 255);
    }
}
