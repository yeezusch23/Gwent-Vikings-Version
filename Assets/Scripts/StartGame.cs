using System.Reflection;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class StartGame : MonoBehaviour
{   
    public GameObject cardPrefab;
    public Deck vikingsDeck;
    public Deck lastKingdomDeck;
    public GameObject handVikings;
    public GameObject handLastKingdom;

    public GameObject handVikingsCount;
    public GameObject handLastKingdomCount;

    void Start () 
    {   
        InitVikingsDeck();
        InitLastKingdomDeck();
        for(int i = 0; i < 10; i++)
        {   
            InstantiateCard(vikingsDeck.cards[i], "Vikings");
            vikingsDeck.RemoveCard(i);
        }
        for(int i = 0; i < 10; i++)
        {   
            InstantiateCard(lastKingdomDeck.cards[i], "Last Kingdom");
            lastKingdomDeck.RemoveCard(i);
        }
    }
    void InstantiateCard (Card card, string faction) 
    {
        GameObject instantiateCard = Instantiate(cardPrefab);
        instantiateCard.name = card.id.ToString();
        //Stats
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().name = card.name;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().id = card.id;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().faction = card.faction;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().type = card.type;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().power = card.power;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().row = card.row;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().effect = card.effect;

        //Image
        instantiateCard.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/Small imgs/" + card.id);
        //Type
        if (card.type == "Oro")
        {
            instantiateCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/goldCard");
        } else if (card.type == "Plata")
        { 
            instantiateCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/silverCard");
        } else {
            Destroy(instantiateCard.transform.Find("Type").GetComponent<Image>());
        }  
        //Power
        if (card.type == "Oro")
        {
            instantiateCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
        } else if (card.type == "Plata")
        { 
            instantiateCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
            instantiateCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0);
        } else {
            Destroy(instantiateCard.transform.Find("Power").GetComponent<TextMeshProUGUI>());
        }     
        //Row
        if (card.row == "close")
        {
            instantiateCard.transform.Find("Row").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/close");
        } else if (card.row == "range")
        {
            instantiateCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/range");
        } else if (card.row == "siege") 
        {
            instantiateCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/siege");
        } else if (card.row == "close_siege")
        {
            instantiateCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/close_range");
        } else 
        {
            Destroy(instantiateCard.transform.Find("Row").GetComponent<Image>());
        }
        //Agrgando el objeto a la escena
        if (faction == "Vikings")
        {
            instantiateCard.transform.SetParent(handVikings.transform, false);
        } else
        {
            instantiateCard.transform.Rotate(0, 0, 180);
            instantiateCard.transform.SetParent(handLastKingdom.transform, false);
        }
        // Debug.Log(card.name);
    }

    
    
    void InitVikingsDeck()
    {   
        
        vikingsDeck.AddCard(new Card("Niebla", 0, "Neutral", "Clima", 0, "Clima", 3));
        vikingsDeck.AddCard(new Card("Tormenta Nórdica", 2, "Neutral", "Clima", 0, "Clima", 2));
        vikingsDeck.AddCard(new Card("Odin", 4, "Vikings", "Aumento", 0, "Aumento", 0));
        vikingsDeck.AddCard(new Card("Ivar the Boneless", 5, "Vikings", "Oro", 6, "range", 8));
        vikingsDeck.AddCard(new Card("Lagertha, la Guerrera Escudo", 6, "Vikings", "Oro", 7, "close_range", 6));
        vikingsDeck.AddCard(new Card("Rollo, el Berserker", 7, "Vikings", "Oro", 8, "close", 9));
        vikingsDeck.AddCard(new Card("Thor", 8, "Vikings", "Aumento", 0, "Aumento", 1));
        //vikingsDeck.AddCard(new Card("Ragnar Lothbrok", 9, "Vikings", "Lider", 0, "Lider", 5));
        vikingsDeck.AddCard(new Card("Ragnarok", 10, "Vikings", "Despeje", 0, "all", 4));
        vikingsDeck.AddCard(new Card("Bjorn Ironside", 11, "Vikings", "Oro", 6, "close", 7));
        vikingsDeck.AddCard(new Card("Floki, el Constructor", 12, "Vikings", "Plata", 4, "close", 10));
        vikingsDeck.AddCard(new Card("Floki, el Constructor", 13, "Vikings", "Plata", 4, "close", 10));
        vikingsDeck.AddCard(new Card("Valhalla", 14, "Vikings", "Plata", 3, "siege", 11));
        vikingsDeck.AddCard(new Card("Valhalla", 15, "Vikings", "Plata", 3, "siege", 11));
        vikingsDeck.AddCard(new Card("Valhalla", 16, "Vikings", "Plata", 3, "siege", 11));
        vikingsDeck.AddCard(new Card("Cuervo", 17, "Vikings", "Plata", 2, "range", 6));
        vikingsDeck.AddCard(new Card("Cuervo", 18, "Vikings", "Plata", 2, "range", 6));
        vikingsDeck.AddCard(new Card("Cuervo", 19, "Vikings", "Plata", 2, "range", 6));
        vikingsDeck.AddCard(new Card("Soldado Distractor", 20, "Vikings", "Señuelo", 0, "", 14));
        vikingsDeck.AddCard(new Card("Ariete Nórdico", 21, "Vikings", "Plata", 1, "siege", 13));
        vikingsDeck.AddCard(new Card("Ariete Nórdico", 22, "Vikings", "Plata", 1, "siege", 13));
        vikingsDeck.AddCard(new Card("Ariete Nórdico", 23, "Vikings", "Plata", 1, "siege", 13));
        vikingsDeck.AddCard(new Card("Catapulta Vikinga", 24, "Vikings", "Plata", 4, "siege", 12));
        vikingsDeck.AddCard(new Card("Catapulta Vikinga", 25, "Vikings", "Plata", 4, "siege", 12));
        vikingsDeck.AddCard(new Card("Catapulta Vikinga", 26, "Vikings", "Plata", 4, "siege", 12));
        vikingsDeck.Shuffle();
    }
    void InitLastKingdomDeck()
    {
        lastKingdomDeck.AddCard(new Card("Niebla", 1, "Neutral", "Clima", 0, "Clima", 3));
        lastKingdomDeck.AddCard(new Card("Tormenta Nórdica", 3, "Neutral", "Clima", 0, "Clima", 2));
        //lastKingdomDeck.AddCard(new Card("Uhtred de Bebbanburg", 27, "Last Kingdom", "Lider", 0, "Lider", 8));
        lastKingdomDeck.AddCard(new Card("Alfred el Grande", 28, "Last Kingdom", "Oro", 8, "close_range", 9));
        lastKingdomDeck.AddCard(new Card("Aethelflaed", 29, "Last Kingdom", "Oro", 7, "close_range", 5));
        lastKingdomDeck.AddCard(new Card("Beocca", 30, "Last Kingdom", "Aumento", 0, "Aumento", 0));
        lastKingdomDeck.AddCard(new Card("Dios Cristiano", 31, "Last Kingdom", "Aumento", 0, "Aumento", 1));
        lastKingdomDeck.AddCard(new Card("Iglesia Cristiana de Wessex", 32, "Last Kingdom", "all", 0, "", 4));
        lastKingdomDeck.AddCard(new Card("Leofric", 33, "Last Kingdom", "Oro", 6, "close", 6));
        lastKingdomDeck.AddCard(new Card("Finan", 34, "Last Kingdom", "Oro", 6, "close", 7));
        lastKingdomDeck.AddCard(new Card("Sihtric", 35, "Last Kingdom", "Plata", 4, "close", 10));
        lastKingdomDeck.AddCard(new Card("Sihtric", 36, "Last Kingdom", "Plata", 4, "close", 10));
        lastKingdomDeck.AddCard(new Card("Steapa", 37, "Last Kingdom", "Plata", 4, "close", 11));
        lastKingdomDeck.AddCard(new Card("Steapa", 38, "Last Kingdom", "Plata", 4, "close", 11));
        lastKingdomDeck.AddCard(new Card("Steapa", 39, "Last Kingdom", "Plata", 4, "close", 11));
        lastKingdomDeck.AddCard(new Card("Sigtryggr & Stiorra", 40, "Last Kingdom", "Plata", 2, "close_range", 6));
        lastKingdomDeck.AddCard(new Card("Sigtryggr & Stiorra", 41, "Last Kingdom", "Plata", 2, "close_range", 6));
        lastKingdomDeck.AddCard(new Card("Sigtryggr & Stiorra", 42, "Last Kingdom", "Plata", 2, "close_range", 6));
        lastKingdomDeck.AddCard(new Card("Ballesta Gigante", 43, "Last Kingdom", "Plata", 3, "siege", 12));
        lastKingdomDeck.AddCard(new Card("Ballesta Gigante", 44, "Last Kingdom", "Plata", 3, "siege", 12));
        lastKingdomDeck.AddCard(new Card("Ballesta Gigante", 45, "Last Kingdom", "Plata", 3, "siege", 12));
        lastKingdomDeck.AddCard(new Card("Aluvión de Flechas", 46, "Last Kingdom", "Plata", 1, "range", 13));
        lastKingdomDeck.AddCard(new Card("Aluvión de Flechas", 47, "Last Kingdom", "Plata", 1, "range", 13));
        lastKingdomDeck.AddCard(new Card("Aluvión de Flechas", 48, "Last Kingdom", "Plata", 1, "range", 13));
        lastKingdomDeck.AddCard(new Card("Halcón Mensajero", 49, "Last Kingdom", "Señuelo", 0, "all", 14));
        lastKingdomDeck.Shuffle();
    }
}