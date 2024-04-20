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

    public int gemsVikingsCount = 2;
    public int gemsLastKingdomCount = 2;

    public Sprite greyGem;

    public bool playerMove = false;

    void Start () 
    {   
        InitGame();

    }
    
    public void CloseRound()
    {   
        int pwVikings = int.Parse(powerVikings.GetComponent<TextMeshProUGUI>().text);
        int pwLastKingdom = int.Parse(powerLastKingdom.GetComponent<TextMeshProUGUI>().text);
        if(pwVikings > pwLastKingdom)
        {
            //Vikings Win    
            gemsLastKingdomCount -= 1;
            gemsLastKingdom.transform.GetChild(gemsLastKingdomCount).GetComponent<Image>().sprite = greyGem;
            gameState = GameState.PLAYER1;
        }
        if(pwVikings < pwLastKingdom)
        {
            //Last Kingdom Win
            gemsVikingsCount -= 1;
            gemsVikings.transform.GetChild(gemsVikingsCount).GetComponent<Image>().sprite = greyGem;
            gameState = GameState.PLAYER2;
        }       
        if(pwVikings == pwLastKingdom)
        {
            gemsLastKingdomCount -= 1;
            gemsLastKingdom.transform.GetChild(gemsLastKingdomCount).GetComponent<Image>().sprite = greyGem;
            gemsVikingsCount -= 1;
            gemsVikings.transform.GetChild(gemsVikingsCount).GetComponent<Image>().sprite = greyGem;
            gameState = GameState.PLAYER1;
        } 
        
        ClearRows();
        AddCardDeck("Vikings");
        AddCardDeck("Vikings");
        AddCardDeck("Last Kingdom");
        AddCardDeck("Last Kingdom");
        UpdateStats();
    }

    public void AddCardDeck(string cardDeckName)
    {
        if(cardDeckName == "Vikings")
        {   
            InstantiateCard(vikingsDeck.cards[0], "Vikings");
            vikingsDeck.RemoveCard(0);
            if(handVikings.transform.childCount > 10)
            {   
                Transform card = handVikings.transform.GetChild(handVikings.transform.childCount-1); 
                card.transform.GetComponent<CardHover>().cardActive = false;
                card.transform.GetComponent<CardSelect>().isSelectable = false;
                if(discardVikings.transform.childCount > 0)
                    discardVikings.transform.GetChild(0).parent = null;
                card.transform.SetParent(discardVikings.transform, false); 
            }
        } else
        {
            InstantiateCard(lastKingdomDeck.cards[0], "Last Kingdom");
            lastKingdomDeck.RemoveCard(0);  
            if(handLastKingdom.transform.childCount > 10)
            {   
                Transform card = handLastKingdom.transform.GetChild(handLastKingdom.transform.childCount-1); 
                card.transform.GetComponent<CardHover>().cardActive = false;
                card.transform.GetComponent<CardSelect>().isSelectable = false;
                if(discardLastkingDom.transform.childCount > 0)
                    discardLastkingDom.transform.GetChild(0).parent = null;
                card.transform.SetParent(discardLastkingDom.transform, false); 
            }
        }

    }

    void ClearRows()
    {   
        int n;
        //Player1
        //close
        n = player1.transform.Find("close").transform.Find("row").childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = player1.transform.Find("close").transform.Find("row").GetChild(0);
            Debug.Log(discardVikings.transform.childCount);
            if(discardVikings.transform.childCount > 0) 
                discardVikings.transform.GetChild(0).transform.parent = null;
            child.transform.SetParent(discardVikings.transform, false);   
        }
        if(player1.transform.Find("close").transform.Find("special").childCount > 0)
        {   
            if(discardVikings.transform.childCount > 0) 
                discardVikings.transform.GetChild(0).transform.parent = null;
            player1.transform.Find("close").transform.Find("special").transform.GetChild(0).transform.SetParent(discardVikings.transform, false);
        }

        //range
        n = player1.transform.Find("range").transform.Find("row").childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = player1.transform.Find("range").transform.Find("row").GetChild(0);
            if(discardVikings.transform.childCount > 0) 
                discardVikings.transform.GetChild(0).transform.parent = null;
            child.transform.SetParent(discardVikings.transform, false);   
        }
        if(player1.transform.Find("range").transform.Find("special").childCount > 0)
        {   
            if(discardVikings.transform.childCount > 0) 
                discardVikings.transform.GetChild(0).transform.parent = null;
            player1.transform.Find("range").transform.Find("special").transform.GetChild(0).transform.SetParent(discardVikings.transform, false);
        }

        //siege
        n = player1.transform.Find("siege").transform.Find("row").childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = player1.transform.Find("siege").transform.Find("row").GetChild(0);
            if(discardVikings.transform.childCount > 0) 
                discardVikings.transform.GetChild(0).transform.parent = null;
            child.transform.SetParent(discardVikings.transform, false);      
        }
        if(player1.transform.Find("siege").transform.Find("special").childCount > 0)
        {   
            if(discardVikings.transform.childCount > 0) 
                discardVikings.transform.GetChild(0).transform.parent = null;
            player1.transform.Find("siege").transform.Find("special").transform.GetChild(0).transform.SetParent(discardVikings.transform, false);
        }
        //Player2
        //close
        n = player2.transform.Find("close").transform.Find("row").childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = player2.transform.Find("close").transform.Find("row").GetChild(0);
            if(discardLastkingDom.transform.childCount > 0) 
                discardLastkingDom.transform.GetChild(0).transform.parent = null;
            child.transform.SetParent(discardLastkingDom.transform, false);         
        }
        if(player2.transform.Find("close").transform.Find("special").childCount > 0)
        {   
            if(discardLastkingDom.transform.childCount > 0) 
                discardLastkingDom.transform.GetChild(0).transform.parent = null;
            player2.transform.Find("close").transform.Find("special").transform.GetChild(0).transform.SetParent(discardLastkingDom.transform, false);
        }
        //range
        n = player2.transform.Find("range").transform.Find("row").childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = player2.transform.Find("range").transform.Find("row").GetChild(0);
            if(discardLastkingDom.transform.childCount > 0) 
                discardLastkingDom.transform.GetChild(0).transform.parent = null;
            child.transform.SetParent(discardLastkingDom.transform, false);      
        }
        if(player2.transform.Find("range").transform.Find("special").childCount > 0)
        {   
            if(discardLastkingDom.transform.childCount > 0) 
                discardLastkingDom.transform.GetChild(0).transform.parent = null;
            player2.transform.Find("range").transform.Find("special").transform.GetChild(0).transform.SetParent(discardLastkingDom.transform, false);
        }
        //siege
        n = player2.transform.Find("siege").transform.Find("row").childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = player2.transform.Find("siege").transform.Find("row").GetChild(0);
            if(discardLastkingDom.transform.childCount > 0) 
                discardLastkingDom.transform.GetChild(0).transform.parent = null;
            child.transform.SetParent(discardLastkingDom.transform, false);      
        }
        if(player2.transform.Find("siege").transform.Find("special").childCount > 0)
        {   
            if(discardLastkingDom.transform.childCount > 0) 
                discardLastkingDom.transform.GetChild(0).transform.parent = null;
            player2.transform.Find("siege").transform.Find("special").transform.GetChild(0).transform.SetParent(discardLastkingDom.transform, false);
        }
        //Climas
        n = climaField.transform.childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = climaField.transform.GetChild(0);
            if(child.transform.rotation.z == 0)
            {
                if(discardVikings.transform.childCount > 0) 
                    discardVikings.transform.GetChild(0).transform.parent = null;
                child.transform.SetParent(discardVikings.transform, false);      
            } else
            {
                if(discardLastkingDom.transform.childCount > 0) 
                    discardLastkingDom.transform.GetChild(0).transform.parent = null;
                child.transform.SetParent(discardLastkingDom.transform, false);
            }
        }
    }

    public void UpdateClimaEffects(Transform row)
    {   
        if(row.transform.childCount > 0) {
            Transform card = row.transform.GetChild(row.transform.childCount - 1);
            string cardType = card.transform.Find("Stats").GetComponent<CardStats>().type;
            string cardRow = card.transform.Find("Stats").GetComponent<CardStats>().row;
            if(cardType != "Oro")
            {   
                // Debug.Log(1);
                if(cardRow == "close" || cardRow == "range" || cardRow == "close_range")
                {   
                    // Debug.Log(1);
                    card.transform.Find("Stats").GetComponent<CardStats>().power -= climaField.transform.childCount;
                    card.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
                }
            }
            if(row.parent.Find("special").transform.childCount > 0)
            {
                int effect = row.parent.Find("special").transform.GetChild(0).transform.Find("Stats").GetComponent<CardStats>().effect;
                if(effect == 0)
                {
                    card.transform.Find("Stats").GetComponent<CardStats>().power += 2;
                    card.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.transform.Find("Stats").GetComponent<CardStats>().power.ToString();          
                } else
                {
                    card.transform.Find("Stats").GetComponent<CardStats>().power += 1;
                    card.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
                }
            }
        }
    }
    public void ActiveEffect(Transform row)
    {   
        // Debug.Log(card.transform.GetChild(card.transform.childCount-1).name);
        int n = row.transform.GetChild(row.transform.childCount-1).Find("Stats").GetComponent<CardStats>().effect; 
        if(n == 0)
            AddPowerRow(row, 2);
        if(n == 1)
            AddPowerRow(row, 1);
        if(n == 2)
            ReducePowerRow("close", -1);
        if(n == 3)
            ReducePowerRow("range", -1);
        if(n == 4)
            DeletClimas(row, 1);
        if(n == 6 || n == 5)
            GetCardOfDeck(row);
        if(n == 7 || n == 8)
            DeletCard(row, 1);
        if(n == 9)
            DeletCard(row, 2);
        if(n == 10)
            AddPowerCardLeft(row);
        if(n == 11)
            AverageCards(row);
        if(n == 12)
            DeletMinRow(row);
        if(n == 13)
            MultiplyByN(row);
    }

    void MultiplyByN(Transform row)
    {   
        
        Transform card = row.transform.GetChild(row.transform.childCount - 1);
        string cardName = card.transform.Find("Stats").GetComponent<CardStats>().name;
        int cnt = 0;
        if(gameState == GameState.PLAYER2 || gameState == GameState.PLAYER1PASS)
        {
            //close
            for(int i = 0; i < player2.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("close").transform.Find("row").GetChild(i);
                if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
                {
                    cnt++;
                }
            }
            //range
            for(int i = 0; i < player2.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("range").transform.Find("row").GetChild(i);
                if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
                {
                    cnt++;
                }
            }
            //siege
            for(int i = 0; i < player2.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("siege").transform.Find("row").GetChild(i);
                if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
                {
                    cnt++;
                }
            }
            card.transform.Find("Stats").GetComponent<CardStats>().power *= cnt;
            card.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        } else
        {
            //close
            for(int i = 0; i < player1.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("close").transform.Find("row").GetChild(i);
                if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
                {
                    cnt++;
                }
            }
            //range
            for(int i = 0; i < player1.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("range").transform.Find("row").GetChild(i);
                if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
                {
                    cnt++;
                }
            }
            //siege
            for(int i = 0; i < player1.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("siege").transform.Find("row").GetChild(i);
                if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
                {
                    cnt++;
                }
            }
            card.transform.Find("Stats").GetComponent<CardStats>().power *= cnt;
            card.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
    }

    void DeletMinRow(Transform row)
    {   
        Transform minRow = row;
        int min = int.MaxValue;
        int childsCount = 0;
        int cnt;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {   
            //close
            cnt = 0;
            for(int i = 0; i < player2.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("close").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                cnt++;
            }
            if(min > cnt && cnt != 0)
            {
                min = cnt;
                minRow = player2.transform.Find("close").transform.Find("row");
            }
            //range
            cnt = 0;
            for(int i = 0; i < player2.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("range").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                cnt++;
            }
            if(min > cnt && cnt != 0)
            {
                min = cnt;
                minRow = player2.transform.Find("range").transform.Find("row");
            }
            //siege
            cnt = 0;
            for(int i = 0; i < player2.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("siege").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("siege").GetComponent<CardStats>().type == "Oro") continue;
                cnt++;
            }
            if(min > cnt && cnt != 0)
            {
                min = cnt;
                minRow = player2.transform.Find("siege").transform.Find("row");
            }
            if(min != int.MaxValue)
            {   
                int n = minRow.childCount;
                for(int i = 0; i < n; i++)
                {   
                    Transform child = minRow.GetChild(0);    
                    // if(child.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                    if(discardLastkingDom.transform.childCount != 0) discardLastkingDom.transform.GetChild(0).parent = null;
                    child.transform.SetParent(discardLastkingDom.transform, false);    
                }
            }
        } else
        {
            //close
            cnt = 0;
            for(int i = 0; i < player1.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("close").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                cnt++;
            }
            if(min > cnt && cnt != 0)
            {
                min = cnt;
                minRow = player1.transform.Find("close").transform.Find("row");
            }
            //range
            cnt = 0;
            for(int i = 0; i < player1.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("range").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                cnt++;
            }
            if(min > cnt && cnt != 0)
            {
                min = cnt;
                minRow = player1.transform.Find("range").transform.Find("row");
            }
            //range
            cnt = 0;
            for(int i = 0; i < player1.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("siege").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                cnt++;
            }
            if(min > cnt && cnt != 0)
            {
                min = cnt;
                minRow = player1.transform.Find("siege").transform.Find("row");
            }
            if(min != int.MaxValue)
            {   
                int n = minRow.childCount;
                for(int i = 0; i < n; i++)
                {   
                    Transform child = minRow.GetChild(0);    
                    // if(child.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                    if(discardVikings.transform.childCount != 0) discardVikings.transform.GetChild(0).parent = null;
                    child.transform.SetParent(discardVikings.transform, false);    
                }
            }
        }
    }

    void AverageCards(Transform row)
    {   
        int sum = 0, cnt = 0, val = 0;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            //close
            cnt += player2.transform.Find("close").transform.Find("row").childCount;
            for(int i = 0; i < player2.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                sum += player2.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
            }
            //range
            cnt += player2.transform.Find("range").transform.Find("row").childCount;
            for(int i = 0; i < player2.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                sum += player2.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
            }
            //siege
            cnt += player2.transform.Find("siege").transform.Find("row").childCount;
            for(int i = 0; i < player2.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                sum += player2.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
            }
            //****************************************************************************************
            if(cnt != 0)
                val = sum/cnt;
            //close
            for(int i = 0; i < player2.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("close").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                child.transform.Find("Stats").GetComponent<CardStats>().power = val;
                child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
            }
            //range
            for(int i = 0; i < player2.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("range").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                child.transform.Find("Stats").GetComponent<CardStats>().power = val;
                child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
            }
            //siege
            for(int i = 0; i < player2.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                Transform child = player2.transform.Find("siege").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                child.transform.Find("Stats").GetComponent<CardStats>().power = val;
                child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
            }
        } else
        {
            //close
            cnt += player1.transform.Find("close").transform.Find("row").childCount;
            for(int i = 0; i < player1.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                sum += player1.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
            }
            //range
            cnt += player1.transform.Find("range").transform.Find("row").childCount;
            for(int i = 0; i < player1.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                sum += player1.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
            }
            //siege
            cnt += player1.transform.Find("siege").transform.Find("row").childCount;
            for(int i = 0; i < player1.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                sum += player1.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
            }
            //****************************************************************************************
            if(cnt != 0)
                val = sum/cnt;
            //close
            for(int i = 0; i < player1.transform.Find("close").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("close").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                child.transform.Find("Stats").GetComponent<CardStats>().power = val;
                child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
            }
            //range
            for(int i = 0; i < player1.transform.Find("range").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("range").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                child.transform.Find("Stats").GetComponent<CardStats>().power = val;
                child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
            }
            //siege
            for(int i = 0; i < player1.transform.Find("siege").transform.Find("row").childCount; i++)
            {   
                Transform child = player1.transform.Find("siege").transform.Find("row").GetChild(i);    
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                child.transform.Find("Stats").GetComponent<CardStats>().power = val;
                child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
            }
        }
    }
    void AddPowerCardLeft(Transform row)
    {
        if(row.transform.childCount >= 2)
        {
            Transform child = row.transform.GetChild(row.transform.childCount-2);
            child.transform.Find("Stats").GetComponent<CardStats>().power += 1;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
    }

    public void DynamicEffects(Transform row)
    {
        CheckAddPowerRight(row);
    }
    void CheckAddPowerRight(Transform row)
    {
        if(row.transform.childCount >= 2)
        {
            Transform childLeft = row.transform.GetChild(row.transform.childCount-2);
            Transform childRight = row.transform.GetChild(row.transform.childCount-1);
            if(childLeft.Find("Stats").GetComponent<CardStats>().effect == 10)
            {   
                childRight.transform.Find("Stats").GetComponent<CardStats>().power += 1;
                childRight.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = childRight.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
            }
        }
    }

    void DeletCard(Transform row, int op)
    {   
        Transform cardMin = row.transform.GetChild(row.transform.childCount-1);
        Transform cardMax = row.transform.GetChild(row.transform.childCount-1);
        int min = int.MaxValue;
        int max = int.MinValue;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {   
            //close
            int childs = player2.transform.Find("close").transform.Find("row").childCount;
            // if(row.transform.parent.name == "close") childs--;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = player2.transform.Find("close").transform.Find("row").GetChild(i);
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    min = player2.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMin = player2.transform.Find("close").transform.Find("row").GetChild(i); 
                }
                if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    max = player2.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMax = player2.transform.Find("close").transform.Find("row").GetChild(i); 
                }
            }
            //range
            childs = player2.transform.Find("range").transform.Find("row").childCount;
            // if(row.transform.parent.name == "range") childs--;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = player2.transform.Find("range").transform.Find("row").GetChild(i);
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    min = player2.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMin = player2.transform.Find("range").transform.Find("row").GetChild(i); 
                }
                if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    max = player2.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMax = player2.transform.Find("range").transform.Find("row").GetChild(i); 
                }
            }
            //siege
            childs = player2.transform.Find("siege").transform.Find("row").childCount;
            // if(row.transform.parent.name == "siege") childs--;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = player2.transform.Find("siege").transform.Find("row").GetChild(i);
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    min = player2.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMin = player2.transform.Find("siege").transform.Find("row").GetChild(i); 
                }
                if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    max = player2.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMax = player2.transform.Find("siege").transform.Find("row").GetChild(i); 
                }
            }
            if(op == 1 && min != int.MaxValue)
            {   
                if(discardLastkingDom.transform.childCount != 0) Destroy(discardLastkingDom.transform.GetChild(0).gameObject);
                cardMin.transform.SetParent(discardLastkingDom.transform, false);
            }
            if(op == 2 && max != int.MinValue)
            {   
                if(discardLastkingDom.transform.childCount != 0) Destroy(discardLastkingDom.transform.GetChild(0).gameObject);
                cardMax.transform.SetParent(discardLastkingDom.transform, false);
            }
        }else{
            //close
            int childs = player1.transform.Find("close").transform.Find("row").childCount;
            // if(row.transform.parent.name == "close") childs--;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = player1.transform.Find("close").transform.Find("row").GetChild(i);
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    min = player1.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMin = player1.transform.Find("close").transform.Find("row").GetChild(i); 
                }
                if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    max = player1.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMax = player1.transform.Find("close").transform.Find("row").GetChild(i); 
                }
            }
            //range
            childs = player1.transform.Find("range").transform.Find("row").childCount;
            // if(row.transform.parent.name == "range") childs--;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = player1.transform.Find("range").transform.Find("row").GetChild(i);
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    min = player1.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMin = player1.transform.Find("range").transform.Find("row").GetChild(i); 
                }
                if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    max = player1.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMax = player1.transform.Find("range").transform.Find("row").GetChild(i); 
                }
            }
            //siege
            childs = player1.transform.Find("siege").transform.Find("row").childCount;
            // if(row.transform.parent.name == "siege") childs--;
            for(int i = 0; i < childs; i++)
            {   
                Transform child = player1.transform.Find("siege").transform.Find("row").GetChild(i);
                // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
                if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    min = player1.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMin = player1.transform.Find("siege").transform.Find("row").GetChild(i); 
                }
                if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
                {
                    max = player1.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                    cardMax = player1.transform.Find("siege").transform.Find("row").GetChild(i); 
                }
            }
            if(op == 1 && min != int.MaxValue)
            {   
                if(discardVikings.transform.childCount != 0) Destroy(discardVikings.transform.GetChild(0).gameObject);
                cardMin.transform.SetParent(discardVikings.transform, false);
            }
            if(op == 2 && max != int.MinValue)
            {   
                if(discardVikings.transform.childCount != 0) Destroy(discardVikings.transform.GetChild(0).gameObject);
                cardMax.transform.SetParent(discardVikings.transform, false);
            }
        }
    }

    void GetCardOfDeck(Transform row)
    {
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            InstantiateCard(vikingsDeck.cards[0], "Vikings");
            vikingsDeck.RemoveCard(0);    
        } else 
        {
            InstantiateCard(lastKingdomDeck.cards[0], "Last Kingdom");
            lastKingdomDeck.RemoveCard(0);
        }
    }

    void DeletClimas(Transform row, int add)
    {   
        // Debug.Log("COUNT: " + row.transform.name);
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
                // Debug.Log("Entra");
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