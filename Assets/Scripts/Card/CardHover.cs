using UnityEngine;
using UnityEngine.UI;
public class CardHover : MonoBehaviour
{   
    public bool cardActive = true;
    public bool cardTop = false;
    public GameObject showCard;
    public void Start()
    {
        
    }
      public void OnHoverEnter()
    {   
        ShowCard();
        if (cardActive && !cardTop)
        {
            TranslateUp();
        }
        //Debug.Log("Hover Enter");
    }

    public void OnHoverExit()
    {   
        DisableShowCard();
        if (cardActive && cardTop)
        {
            TranslateDown();
        }
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

    public void ShowCard()
    {

    }
    public void DisableShowCard()
    {

    }
}