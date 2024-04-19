using System.Reflection;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum GameState { PLAYER1, PLAYER2, PLAYER1PASS, PLAYER2PASS }
public enum RoundState {ROUND1, ROUND2, ROUND3}

public class StartGame : MonoBehaviour
{   
    public GameObject cardPrefab;
    public Deck vikingsDeck;
    public Deck lastKingdomDeck;
    public GameObject handVikings;
    public GameObject handLastKingdom;

    public GameObject handVikingsCount;
    public GameObject handLastKingdomCount;

    public GameObject showCard;

    public GameState gameState;

    public GameObject player1;
    public GameObject player2;

    public GameObject climaField;

    public GameObject selectedCard;

    //Vikings Stats
    public GameObject powerVikings;
    public GameObject closePowerVikings;
    public GameObject rangePowerVikings;
    public GameObject siegePowerVikings;
    public GameObject cardsHandVikings;
    public GameObject cardsDeckVikings;
    public GameObject gemsVikings;
    public GameObject discardVikings;

    //Last Kingdom Stats
    public GameObject powerLastKingdom;
    public GameObject closePowerLastKingdom;
    public GameObject rangePowerLastKingdom;
    public GameObject siegePowerLastKingdom;
    public GameObject cardsHandLastKingdom;
    public GameObject cardsDeckLastKingdom;
    public GameObject gemsLastKingdom;
    public GameObject discardLastkingDom;

    //Buttons
    public GameObject button1;
    public GameObject button2;


    Player PLAYER1;
    Player PLAYER2;
    void Start () 
    {   
        InitGame();



    }
    
    // public void UpdateEffects(Transform row)
    // {   
    //     Transform card = row.transform.GetChild(row.transform.childCount - 1);
    // }
    public void ActiveEffect(Transform row)
    {   
        // Debug.Log(card.transform.GetChild(card.transform.childCount-1).name);
        int n = row.transform.GetChild(row.transform.childCount-1).Find("Stats").GetComponent<CardStats>().effect; 
        if(n == 0)
            AddPowerRow(row, 2);
        if(n == 1)
            AddPowerRow(row, 2);
        if(n == 2)
            ReducePowerRow("close", -1);
        if(n == 3)
            ReducePowerRow("range", -1);
        if(n == 4)
            DeletClimas(row, 1);
        // if(n == 5)
        if(n == 6)
            GetCardOfDeck(row);
    }

    void GetCardOfDeck(Transform row)
    {
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            int n = int.Parse(handVikingsCount.transform.Find("number").GetComponent<TextMeshProUGUI>().text);
            InstantiateCard(vikingsDeck.cards[0], "Vikings");
            vikingsDeck.RemoveCard(0);    
        } else 
        {
            int n = int.Parse(handLastKingdomCount.transform.Find("number").GetComponent<TextMeshProUGUI>().text);
            InstantiateCard(lastKingdomDeck.cards[0], "Vikings");
            lastKingdomDeck.RemoveCard(0);
        }
    }

    void DeletClimas(Transform row, int add)
    {   
        Debug.Log("COUNT: " + row.transform.name);
        int childs = climaField.transform.childCount;
        for(int i = 0; i < childs; i++)
        {    
            // Debug.Log(i);
            int n = climaField.transform.GetChild(0).Find("Stats").GetComponent<CardStats>().effect; 
            if(n == 2)
                ReducePowerRow("close", 1);
            else
                ReducePowerRow("range", 1);
            
            if(climaField.transform.GetChild(0).rotation.z == 0){
                if(discardVikings.transform.childCount != 0) Destroy(discardVikings.transform.GetChild(0).gameObject);
                climaField.transform.GetChild(0).transform.SetParent(discardVikings.transform, false);
            }
            else
            {   
                if(discardLastkingDom.transform.childCount != 0) Destroy(discardLastkingDom.transform.GetChild(0).gameObject);
                climaField.transform.GetChild(0).transform.SetParent(discardLastkingDom.transform, false);
            }
        }
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            if(discardVikings.transform.childCount != 0) Destroy(discardVikings.transform.GetChild(0).gameObject);
            row.transform.GetChild(row.transform.childCount-1).transform.SetParent(discardVikings.transform, false);
        }else{
            if(discardLastkingDom.transform.childCount != 0) Destroy(discardLastkingDom.transform.GetChild(0).gameObject);
            row.transform.GetChild(row.transform.childCount-1).transform.SetParent(discardLastkingDom.transform, false);
        }
    }

    void ReducePowerRow(string rowType, int add)
    {
        int childs = player1.transform.Find(rowType).transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = player1.transform.Find(rowType).transform.Find("row").GetChild(i);
            if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro" || child.transform.Find("Stats").GetComponent<CardStats>().type == "Despeje") continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power += add;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        childs = player2.transform.Find(rowType).transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = player2.transform.Find(rowType).transform.Find("row").GetChild(i);
            Debug.Log(child.transform.name);
            if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro" || child.transform.Find("Stats").GetComponent<CardStats>().type == "Despeje") continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power += add;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        UpdateStats();
    }
    void AddPowerRow(Transform row, int cnt)
    {   
        int childs = row.parent.transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {
            Transform child = row.parent.transform.Find("row").GetChild(i);
            if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power += cnt;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        UpdateStats();
    }

    public void UpdateStats()
    {   
        ResetField(1);
        ResetField(2);
        int sum, power = 0;
        //Vikings
        //close
        sum = 0;
        for(int i = 0; i < player1.transform.Find("close").transform.Find("row").childCount; i++)
        {   
            sum += player1.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        power += sum;
        player1.transform.Find("close").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = sum.ToString();
        //range
        sum = 0;
        for(int i = 0; i < player1.transform.Find("range").transform.Find("row").childCount; i++)
        {   
            sum += player1.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        power += sum;
        player1.transform.Find("range").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = sum.ToString();
        //siege
        sum = 0;
        for(int i = 0; i < player1.transform.Find("siege").transform.Find("row").childCount; i++)
        {   
            sum += player1.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        power += sum;
        player1.transform.Find("siege").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = sum.ToString();
        //hand
        int cnt = player1.transform.Find("hand").transform.childCount;
        player1.transform.Find("Stats").transform.Find("handCount").transform.Find("number").GetComponent<TextMeshProUGUI>().text = cnt.ToString();
        //power
        player1.transform.Find("Stats").transform.Find("powerCount").transform.Find("number").GetComponent<TextMeshProUGUI>().text = power.ToString();
        //Deck
        player1.transform.Find("Deck").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = vikingsDeck.cards.Count.ToString();
        
        //LastKingdom
        //close
        power = 0; 
        sum = 0;
        for(int i = 0; i < player2.transform.Find("close").transform.Find("row").childCount; i++)
        {   
            sum += player2.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        power += sum;
        player2.transform.Find("close").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = sum.ToString();
        //range
        sum = 0;
        for(int i = 0; i < player2.transform.Find("range").transform.Find("row").childCount; i++)
        {   
            sum += player2.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        power += sum;
        player2.transform.Find("range").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = sum.ToString();
        //siege
        sum = 0;
        for(int i = 0; i < player2.transform.Find("siege").transform.Find("row").childCount; i++)
        {   
            sum += player2.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        power += sum;
        player2.transform.Find("siege").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = sum.ToString();
        //hand
        int cnt2 = player2.transform.Find("hand").transform.childCount;
        player2.transform.Find("Stats").transform.Find("handCount").transform.Find("number").GetComponent<TextMeshProUGUI>().text = cnt2.ToString();
        //power
        player2.transform.Find("Stats").transform.Find("powerCount").transform.Find("number").GetComponent<TextMeshProUGUI>().text = power.ToString();
        //Deck
        player2.transform.Find("Deck").transform.Find("count").transform.Find("number").GetComponent<TextMeshProUGUI>().text = lastKingdomDeck.cards.Count.ToString();
        
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            for(int i = 0; i < player1.transform.Find("hand").childCount; i++)
            {
                player1.transform.Find("hand").GetChild(i).transform.GetComponent<CardSelect>().isSelectable = true;
                player1.transform.Find("hand").GetChild(i).transform.GetComponent<CardHover>().isHoverable = true;
                player1.transform.Find("hand").GetChild(i).Find("Back").transform.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            } 
            for(int i = 0; i < player2.transform.Find("hand").childCount; i++)
            {
                player2.transform.Find("hand").GetChild(i).transform.GetComponent<CardSelect>().isSelectable = false;
                player2.transform.Find("hand").GetChild(i).transform.GetComponent<CardHover>().isHoverable = false;
                player2.transform.Find("hand").GetChild(i).Find("Back").transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
        } 
        if(gameState == GameState.PLAYER2 || gameState == GameState.PLAYER1PASS)
        {
            for(int i = 0; i < player1.transform.Find("hand").childCount; i++)
            {
                player1.transform.Find("hand").GetChild(i).transform.GetComponent<CardSelect>().isSelectable = false;
                player1.transform.Find("hand").GetChild(i).transform.GetComponent<CardHover>().isHoverable = false;
                player1.transform.Find("hand").GetChild(i).Find("Back").transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            } 
            for(int i = 0; i < player2.transform.Find("hand").childCount; i++)
            {
                player2.transform.Find("hand").GetChild(i).transform.GetComponent<CardSelect>().isSelectable = true;
                player2.transform.Find("hand").GetChild(i).transform.GetComponent<CardHover>().isHoverable = true;
                player2.transform.Find("hand").GetChild(i).Find("Back").transform.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
        }
    }
    void InitGame()
    {   
        PLAYER1 = new Player();
        PLAYER2 = new Player();
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
    public void ResetField(int player) 
    {
        GameObject playerField;
        if (player == 1)
        {
            playerField = player1;
        } else
        {
            playerField = player2;
        }
        // Reiniciando todos los campos
        playerField.GetComponent<RowInfo>().Reset();

        // Restaurando los special Sprites de los campos, Tambien se puede usar la variante Resources.Load<Sprite>("path");
        Sprite specialSprite = playerField.GetComponent<RowInfo>().special;
        playerField.transform.Find("close").Find("special").GetComponent<Image>().sprite = specialSprite;
        playerField.transform.Find("range").Find("special").GetComponent<Image>().sprite = specialSprite;
        playerField.transform.Find("siege").Find("special").GetComponent<Image>().sprite = specialSprite;

        // Restaurando las rows de los campos, Tambien se puede usar la variante Resources.Load<Sprite>("path");
        Sprite rowSprite = playerField.GetComponent<RowInfo>().close;
        playerField.transform.Find("close").Find("row").GetComponent<Image>().sprite = rowSprite;
        rowSprite = playerField.GetComponent<RowInfo>().range;
        playerField.transform.Find("range").Find("row").GetComponent<Image>().sprite = rowSprite;
        rowSprite = playerField.GetComponent<RowInfo>().siege;
        playerField.transform.Find("siege").Find("row").GetComponent<Image>().sprite = rowSprite;
        
        Sprite spriteClima = playerField.GetComponent<RowInfo>().clima;
        climaField.GetComponent<Image>().sprite = spriteClima;
        // Light off the weather board and deselect weather card
        // climaField.GetComponent<Image>().sprite = climaField.GetComponent<WeatherManager>().weather;
        // climaField.GetComponent<WeatherManager>().isWeatherCard = false;
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
        } else if (card.row == "close_range")
        {
            instantiateCard.transform.Find("Row").GetComponent<Image>().sprite= Resources.Load<Sprite>("Cards/close_range");
        } else 
        {
            Destroy(instantiateCard.transform.Find("Row").GetComponent<Image>());
        }
        //Back
        if (faction != "Vikings")
        {
            instantiateCard.transform.Find("Back").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/backLastKingdomCard");
            instantiateCard.transform.Find("Back").GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        
        
        //Agrgando el objeto a la escena
        if (faction == "Vikings")
        {
            instantiateCard.transform.SetParent(handVikings.transform, false);
            instantiateCard.transform.GetComponent<CardSelect>().isSelectable = true;
        } else
        {
            instantiateCard.transform.Rotate(0, 0, 180);
            instantiateCard.transform.SetParent(handLastKingdom.transform, false);
            instantiateCard.transform.GetComponent<CardHover>().isHoverable = false;
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
        vikingsDeck.AddCard(new Card("Soldado Distractor", 20, "Vikings", "all", 0, "", 14));
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
        lastKingdomDeck.AddCard(new Card("Iglesia Cristiana de Wessex", 32, "Last Kingdom", "Despeje", 0, "all", 4));
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
        lastKingdomDeck.AddCard(new Card("Halcón Mensajero", 49, "Last Kingdom", "all", 0, "all", 14));
        lastKingdomDeck.Shuffle();
    }

}