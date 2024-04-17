using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DrawCard : MonoBehaviour
{   
    public GameObject cardStats;
    public GameObject cardImage;
    public GameObject cardType;
    public GameObject cardPower;    
    public GameObject cardRow;

    void Start() {
        // Debug.Log("Hola");
        CardStats card = cardStats.GetComponent<CardStats>();
        //Image
        Debug.Log(card.name);
        cardImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/Small imgs/" + card.id);
        // //Type
        // if (card.type == "Oro")
        // {
        //     cardType.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/goldCard");
        // } else if (card.type == "Plata")
        // {
        //     cardType.GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/silverCard");
        // } else {
        //     Destroy(cardType.GetComponent<Image>());
        // }  
        // //Power
        // if (card.type == "Oro")
        // {
        //     cardPower.GetComponent<TextMeshProUGUI>().text = card.power.ToString();
        // } else if (card.type == "Plata")
        // { 
        //     cardPower.GetComponent<TextMeshProUGUI>().text = card.power.ToString();
        //     cardPower.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0);
        // } else {
        //     Destroy(cardPower.GetComponent<TextMeshProUGUI>());
        // }     
        // //Row
        // if (card.row == "close")
        // {
        //     cardRow.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/close");
        // } else if (card.row == "range")
        // {
        //     cardRow.GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/range");
        // } else if (card.row == "siege") 
        // {
        //     cardRow.GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/siege");
        // } else if (card.row == "close_siege")
        // {
        //     cardRow.GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/close_range");
        // } else 
        // {
        //     Destroy(cardRow.GetComponent<Image>());
        // }
        //Debug.Log(cardStats.name);
    }
}