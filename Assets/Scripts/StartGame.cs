using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.XR;

public enum GameState { PLAYER1, PLAYER2, PLAYER1PASS, PLAYER2PASS }

public class StartGame : MonoBehaviour
{   
    public Player Player1 = new Player();
    public Player Player2 = new Player();

    public GameComponent board;
    public GameObject cardPrefab;
    
    public Deck CreatedCards;
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
    public Sprite redGem;

    public Sprite leaderImage;

    public bool playerMove = false;

    public GameObject menuReset;
    
    public int roundCount = 0;
    public int changedCard = 0;

    public static List<Card> deck1;
    public static List<Card> deck2;

    // Logic
    public Hand vikingsHand = new Hand();
    public Hand lastKingdomHand = new Hand();

    public Board boardCards = new Board();

    void Start () 
    {   
        // if(DataManager.myStringList.Count == 0)
        // {
        //     Debug.Log("Aun no hay cartas creadas");
        // }
        // for(int i = 0; i < DataManager.myStringList.Count; i++){
        //     Debug.Log(DataManager.myStringList[i]);
        // }



        //Iniciar Juego
        InitGame();
    }
    
    //Evaluar ganador de la ronda
    public void CloseRound()
    {   
        //Poderes acumulados por los jugadores en la ronda
        int pwVikings = int.Parse(powerVikings.GetComponent<TextMeshProUGUI>().text);
        int pwLastKingdom = int.Parse(powerLastKingdom.GetComponent<TextMeshProUGUI>().text);
        
        //Vikings Gana
        if(pwVikings > pwLastKingdom)
        {
            gemsLastKingdomCount -= 1;
            gemsLastKingdom.transform.GetChild(gemsLastKingdomCount).GetComponent<Image>().sprite = greyGem;
            gameState = GameState.PLAYER1;
        }
        
        //Last Kingdom Gana
        if(pwVikings < pwLastKingdom)
        {
            gemsVikingsCount -= 1;
            gemsVikings.transform.GetChild(gemsVikingsCount).GetComponent<Image>().sprite = greyGem;
            gameState = GameState.PLAYER2;
        }       
        
        //Empate
        if(pwVikings == pwLastKingdom)
        {
            gemsLastKingdomCount -= 1;
            gemsLastKingdom.transform.GetChild(gemsLastKingdomCount).GetComponent<Image>().sprite = greyGem;
            gemsVikingsCount -= 1;
            gemsVikings.transform.GetChild(gemsVikingsCount).GetComponent<Image>().sprite = greyGem;
            gameState = GameState.PLAYER1;
        } 
        
        //Limpiar el campo
        ClearRows();
        //Agregar dos cartas a cada mazo
        AddCardDeck("Vikings");
        AddCardDeck("Vikings");
        AddCardDeck("Last Kingdom");
        AddCardDeck("Last Kingdom");
        //Actualizar estadisticas
        UpdateStats();
        
        //Terminar la partida si al menos uno no tiene gemas
        if(gemsVikingsCount == 0 && gemsLastKingdomCount == 0)
        {
            menuReset.SetActive(true);
            menuReset.transform.Find("Ganador").GetComponent<TextMeshProUGUI>().text = "EMPATE";
        }else if(gemsVikingsCount == 0)
        {
            menuReset.SetActive(true);
            menuReset.transform.Find("Ganador").GetComponent<TextMeshProUGUI>().text = "JUGADOR 2 GANA";
        } else if(gemsLastKingdomCount == 0)
        {
            menuReset.SetActive(true);
            menuReset.transform.Find("Ganador").GetComponent<TextMeshProUGUI>().text = "JUGADOR 1 GANA";
        }

    }

    //Agregar una carta al mazo
    public void AddCardDeck(string cardDeckName)
    {   
        if(cardDeckName == "Vikings")
        {   
            InstantiateCard(vikingsDeck.cards[0], "Vikings");
            vikingsDeck.Remove(vikingsDeck.cards[0]);
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
            lastKingdomDeck.Remove(lastKingdomDeck.cards[0]);  
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

    //Borrar Carta de una Fila
    void ClearCardRow(Transform card, int player)
    {   
        if(player == 1)
        {   
            if(discardVikings.transform.childCount > 0) 
                discardVikings.transform.GetChild(0).transform.parent = null;
            card.transform.SetParent(discardVikings.transform, false);
        } else
        {
            if(discardLastkingDom.transform.childCount > 0) 
                discardLastkingDom.transform.GetChild(0).transform.parent = null;
            card.transform.SetParent(discardLastkingDom.transform, false);
        }
    }
    
    //Borrar carta de campo de aumento
    void ClearCardSpecial(string specialName, int player)
    {   
        if(player == 1) {
            if(player1.transform.Find(specialName).transform.Find("special").childCount > 0)
            {   
                if(discardVikings.transform.childCount > 0) 
                    discardVikings.transform.GetChild(0).transform.parent = null;
                player1.transform.Find(specialName).transform.Find("special").transform.GetChild(0).transform.SetParent(discardVikings.transform, false);
            }
        }else
        {
            if(player2.transform.Find(specialName).transform.Find("special").childCount > 0)
            {   
                if(discardLastkingDom.transform.childCount > 0) 
                    discardLastkingDom.transform.GetChild(0).transform.parent = null;
                player2.transform.Find(specialName).transform.Find("special").transform.GetChild(0).transform.SetParent(discardLastkingDom.transform, false);
            }
        }    
    }
    
    //Limpiar fila
    void ClearRow(string rowName, int player)
    {   
        GameObject playerField;
        if(player == 1) 
        {
            playerField = player1;
        } else 
        {
            playerField = player2;    
        }
        int n = playerField.transform.Find(rowName).transform.Find("row").childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = playerField.transform.Find(rowName).transform.Find("row").GetChild(0);
            ClearCardRow(child, player);
        }
        ClearCardSpecial(rowName, player);
    }

    //Limpiar
    void ClearWeathers()
    {
        int n = climaField.transform.childCount;
        for(int i = 0; i < n; i++)
        {   
            Transform child = climaField.transform.GetChild(0);
            if(child.transform.rotation.z == 0)
            {   
                ClearCardRow(child, 1);
            } else
            {
                ClearCardRow(child, 2);
            }
        }
    }

    //Limpiar el campo
    void ClearRows()
    {   
        //Player1
        //close
        ClearRow("close", 1);
        //range
        ClearRow("range", 1);
        //siege
        ClearRow("siege", 1);
        //Player2
        //close
        ClearRow("close", 2);
        //range
        ClearRow("range", 2);
        //siege
        ClearRow("siege", 2);
        //Climas
        ClearWeathers();
    }

    //Actualizar los efectos de (clima y aumento) dinamicos
    public void UpdateClimaEffects(Transform row)
    {   
        if(row.transform.childCount > 0) {
            Transform card = row.transform.GetChild(row.transform.childCount - 1);
            Card.Type? cardType = card.transform.Find("Stats").GetComponent<CardStats>().type;
            string cardRow = row.parent.name;
            if(cardType != Card.Type.Golden)
            {   
                //Efecto del clima
                for(int i  = 0; i < climaField.transform.childCount; i++)
                {   
                    int cardClimaEffect = climaField.transform.GetChild(i).Find("Stats").GetComponent<CardStats>().effect;
                    if((cardRow == "close" && cardClimaEffect == 2) || (cardRow == "range" && cardClimaEffect == 3))
                    {         
                        card.transform.Find("Stats").GetComponent<CardStats>().power -= 1;
                        card.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
                    } 
                }
                //Efecto de los aumentos
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
    }
    
    //Activar efectos
    public void ActiveEffect(Transform row, CardStats card)
    {   
        //Enconrar numero del efecto de la carta
        // int n = row.transform.GetChild(row.transform.childCount-1).Find("Stats").GetComponent<CardStats>().effect; 
        int n = card.effect;
        if(n == 100)
        {

        }
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
        if(n == 6 || n == 5 || n == 17 || n == 18)
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

        if(n == 15) Damage(row, DataManager.myAmountList["Damage"]);
        if(n == 16)
            Boost(row, DataManager.myAmountList["Boost"]);        
        if(n == 18)
            Boost(row, DataManager.myAmountList["DrawAndBoost"]);

        CheckAddPowerRight(row);
    }

    /*
    Efecto (#13)
    *Multiplica por n su ataque, siendo n la cantidad de cartas iguales a ella en el campo*/
    void MultiplyByN(Transform row)
    {   
        
        Transform card = row.transform.GetChild(row.transform.childCount - 1);
        string cardName = card.transform.Find("Stats").GetComponent<CardStats>().name;
        int cnt = 0;
        GameObject playerField;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
            playerField = player1;
        else
            playerField = player2;

        //Contando la cantidad de cartas iguales a ella en cada fila
        //close
        for(int i = 0; i < playerField.transform.Find("close").transform.Find("row").childCount; i++)
        {   
            Transform child = playerField.transform.Find("close").transform.Find("row").GetChild(i);
            if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
            {
                cnt++;
            }
        }
        //range
        for(int i = 0; i < playerField.transform.Find("range").transform.Find("row").childCount; i++)
        {   
            Transform child = playerField.transform.Find("range").transform.Find("row").GetChild(i);
            if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
            {
                cnt++;
            }
        }
        //siege
        for(int i = 0; i < playerField.transform.Find("siege").transform.Find("row").childCount; i++)
        {   
            Transform child = playerField.transform.Find("siege").transform.Find("row").GetChild(i);
            if(child.transform.Find("Stats").GetComponent<CardStats>().name == cardName)
            {
                cnt++;
            }
        }

        card.transform.Find("Stats").GetComponent<CardStats>().power *= cnt;
        card.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
    }

    /*
    Efecto (#12)
    *Limpia la fila del campo (no vacia) del rival con menos unidades*/
    void DeletMinRow(Transform row)
    {   
        Transform minRow = row;
        int min = int.MaxValue, cnt, player;
        GameObject playerField;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            playerField = player2;
            player = 2;
        } else
        {
            playerField = player1;
            player = 1;
        }
        
        //Buscar la fila con la menor cantidad de cartas
        //close
        cnt = playerField.transform.Find("close").transform.Find("row").childCount;
        if(min > cnt && cnt != 0)
        {
            min = cnt;
            minRow = playerField.transform.Find("close").transform.Find("row");
        }
        //range
        cnt = playerField.transform.Find("range").transform.Find("row").childCount;
        if(min > cnt && cnt != 0)
        {
            min = cnt;
            minRow = playerField.transform.Find("range").transform.Find("row");
        }
        //siege
        cnt = playerField.transform.Find("siege").transform.Find("row").childCount;
        if(min > cnt && cnt != 0)
        {
            min = cnt;
            minRow = playerField.transform.Find("siege").transform.Find("row");
        }

        //Borar fila
        if(min != int.MaxValue)
        {   
            int n = minRow.childCount;
            for(int i = 0; i < n; i++)
            {   
                Transform child = minRow.GetChild(0);    
                ClearCardRow(child, player);    
            }
        }
    }

    
    public void Damage(Transform row, int amount)
    {   
        GameObject playerField;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            playerField = player2;
        } else
        {
            playerField = player1;
        }

        //close
        for(int i = 0; i < playerField.transform.Find("close").transform.Find("row").childCount; i++)
        {   
            playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power -= amount;
            playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Power").GetComponent<TextMeshProUGUI>().text = playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        //range
        for(int i = 0; i < playerField.transform.Find("range").transform.Find("row").childCount; i++)
        {   
            playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power -= amount;
            playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Power").GetComponent<TextMeshProUGUI>().text = playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        //siege
        for(int i = 0; i < playerField.transform.Find("siege").transform.Find("row").childCount; i++)
        {   
            playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power -= amount;
            playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Power").GetComponent<TextMeshProUGUI>().text = playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power.ToString();

        }

            // Transform child = playerField.transform.Find("close").transform.Find("row").GetChild(i);
            // child.transform.Find("Stats").GetComponent<CardStats>().power = val;
            // child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
       
    }

    
    public void Boost(Transform row, int amount)
    {   
        GameObject playerField;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            playerField = player1;
        } else
        {
            playerField = player2;
        }

        //close
        for(int i = 0; i < playerField.transform.Find("close").transform.Find("row").childCount; i++)
        {   
            playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power += amount;
            playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Power").GetComponent<TextMeshProUGUI>().text = playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        //range
        for(int i = 0; i < playerField.transform.Find("range").transform.Find("row").childCount; i++)
        {   
            playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power += amount;
            playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Power").GetComponent<TextMeshProUGUI>().text = playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        //siege
        for(int i = 0; i < playerField.transform.Find("siege").transform.Find("row").childCount; i++)
        {   
            playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power += amount;
            playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Power").GetComponent<TextMeshProUGUI>().text = playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power.ToString();

        }

       
    }

        void AverageCards(Transform row)
    {   
        int sum = 0, cnt = 0, val = 0;
        GameObject playerField;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            playerField = player2;
        } else
        {
            playerField = player1;
        }

        //Calcular la suma de todas las cartas del campo rival
        //close
        cnt += playerField.transform.Find("close").transform.Find("row").childCount;
        for(int i = 0; i < playerField.transform.Find("close").transform.Find("row").childCount; i++)
        {   
            sum += playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        //range
        cnt += playerField.transform.Find("range").transform.Find("row").childCount;
        for(int i = 0; i < playerField.transform.Find("range").transform.Find("row").childCount; i++)
        {   
            sum += playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }
        //siege
        cnt += playerField.transform.Find("siege").transform.Find("row").childCount;
        for(int i = 0; i < playerField.transform.Find("siege").transform.Find("row").childCount; i++)
        {   
            sum += playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
        }

        //Calculando el promedio, //*Condicion cubrir el de la division por 0 (NO HAY CARTAS EN EL CAMPO)
        if(cnt != 0)
            val = sum/cnt;
        
        //Actualizar el poder de las cartas
        //close
        for(int i = 0; i < playerField.transform.Find("close").transform.Find("row").childCount; i++)
        {   
            Transform child = playerField.transform.Find("close").transform.Find("row").GetChild(i);
            child.transform.Find("Stats").GetComponent<CardStats>().power = val;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
        }
        //range
        for(int i = 0; i < playerField.transform.Find("range").transform.Find("row").childCount; i++)
        {   
            Transform child = playerField.transform.Find("range").transform.Find("row").GetChild(i);    
            // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power = val;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
        }
        //siege
        for(int i = 0; i < playerField.transform.Find("siege").transform.Find("row").childCount; i++)
        {   
            Transform child = playerField.transform.Find("siege").transform.Find("row").GetChild(i);    
            // if(child.transform.Find("Stats").GetComponent<CardStats>().type == "Oro") continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power = val;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = val.ToString();
        }
    }


    
    /*
    Efecto (#10)
    *Aumenta en 1 el poder las cartas adyacentes*/
    //Agregando poder a la carta de la izquierda
    void AddPowerCardLeft(Transform row)
    {
        if(row.transform.childCount >= 2)
        {
            Transform child = row.transform.GetChild(row.transform.childCount-2);
            child.transform.Find("Stats").GetComponent<CardStats>().power += 1;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
    }

    //Agregando poder a la carta de la derecha
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

    /*
    Efecto (#7, #8, #9)
    *Eliminar la carta con mas(menos) poder del campo rival*/
    void DeletCard(Transform row, int op)
    {   
        Transform cardMin = null;
        Transform cardMax = null;
        int min = int.MaxValue, max = int.MinValue, player;
        
        GameObject playerField;
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            playerField = player2;
            player = 2;
        } else
        {
            playerField = player1;
            player = 1;
        }

        //Buscar la carta con menor y mayor poder
        //close
        int childs = playerField.transform.Find("close").transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = playerField.transform.Find("close").transform.Find("row").GetChild(i);
            if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
            {
                min = playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                cardMin = playerField.transform.Find("close").transform.Find("row").GetChild(i); 
            }
            if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
            {
                max = playerField.transform.Find("close").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                cardMax = playerField.transform.Find("close").transform.Find("row").GetChild(i); 
            }
        }
        //range
        childs = playerField.transform.Find("range").transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = playerField.transform.Find("range").transform.Find("row").GetChild(i);
            if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
            {
                min = playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                cardMin = playerField.transform.Find("range").transform.Find("row").GetChild(i); 
            }
            if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
            {
                max = playerField.transform.Find("range").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                cardMax = playerField.transform.Find("range").transform.Find("row").GetChild(i); 
            }
        }
        //siege
        childs = playerField.transform.Find("siege").transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = playerField.transform.Find("siege").transform.Find("row").GetChild(i);
            if(min > child.transform.Find("Stats").GetComponent<CardStats>().power)
            {
                min = playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                cardMin = playerField.transform.Find("siege").transform.Find("row").GetChild(i); 
            }
            if(max < child.transform.Find("Stats").GetComponent<CardStats>().power)
            {
                max = playerField.transform.Find("siege").transform.Find("row").GetChild(i).transform.Find("Stats").GetComponent<CardStats>().power;
                cardMax = playerField.transform.Find("siege").transform.Find("row").GetChild(i); 
            }
        }
        
        //Borrar la carta con menor poder
        if(op == 1 && min != int.MaxValue)
        {   
            ClearCardRow(cardMin, player);
        }
        //Borrar la carta con mayor poder
        if(op == 2 && max != int.MinValue)
        {   
            ClearCardRow(cardMax, player);
        }

    }

    /*
    Efecto (#5, #6)
    *Robar una carta*/
    void GetCardOfDeck(Transform row)
    {
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {
            InstantiateCard(vikingsDeck.cards[0], "Vikings");
            vikingsDeck.Remove(vikingsDeck.cards[0]);    
        } else 
        {
            InstantiateCard(lastKingdomDeck.cards[0], "Last Kingdom");
            lastKingdomDeck.Remove(lastKingdomDeck.cards[0]);
        }
    }

    /*
    Efecto (#4)
    *Despeja el clima eliminando todas las cartas de tipo clima*/
    void DeletClimas(Transform row, int add)
    {   
        int childs = climaField.transform.childCount;
        for(int i = 0; i < childs; i++)
        {    
            int n = climaField.transform.GetChild(0).Find("Stats").GetComponent<CardStats>().effect; 
            //Actualizar poderes de las cartas a las que afecta el clima
            if(n == 2)
                ReducePowerRow("close", 1);
            else
                ReducePowerRow("range", 1);
            
            //Borar clima
            if(climaField.transform.GetChild(0).rotation.z == 0){
                ClearCardRow(climaField.transform.GetChild(0).transform, 1);
            }
            else
            {   
                ClearCardRow(climaField.transform.GetChild(0).transform, 2);
            }
        }
        //Borar despeje
        if(gameState == GameState.PLAYER1 || gameState == GameState.PLAYER2PASS)
        {   
            ClearCardRow(row.transform.GetChild(row.transform.childCount-1).transform, 1);
        }else{
            ClearCardRow(row.transform.GetChild(row.transform.childCount-1).transform, 2);
        }
    }

    /*
    Efecto (#2, #3)
    *Reduce en 1 la fuerza de todas las unidades (cuerpo a cuerpo) en el campo de batalla
    *Reduce en 1 la potencia de los (ataques a distancia) de todas las unidades en el campo de batalla*/
    void ReducePowerRow(string rowType, int add)
    {   
        // Debug.Log(rowType);
        int childs = player1.transform.Find(rowType).transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = player1.transform.Find(rowType).transform.Find("row").GetChild(i);
            if(child.transform.Find("Stats").GetComponent<CardStats>().type == Card.Type.Golden) continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power += add;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        childs = player2.transform.Find(rowType).transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = player2.transform.Find(rowType).transform.Find("row").GetChild(i);
            Debug.Log(child.transform.name);
            if(child.transform.Find("Stats").GetComponent<CardStats>().type == Card.Type.Golden) continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power += add;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        //Actualizar estadisticas
        UpdateStats();
    }
    
    /*
    Efecto (#2, #3)
    *Aumenta en 1 (o 2) el poder de las cartas de la fila donde es colocada*/
    void AddPowerRow(Transform row, int cnt)
    {   
        int childs = row.parent.transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {
            Transform child = row.parent.transform.Find("row").GetChild(i);
            if(child.transform.Find("Stats").GetComponent<CardStats>().type == Card.Type.Golden) continue;
            child.transform.Find("Stats").GetComponent<CardStats>().power += cnt;
            child.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = child.transform.Find("Stats").GetComponent<CardStats>().power.ToString();
        }
        UpdateStats();
    }

    //Actualizar estadisticas
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

    // //Reiniciar la partida
    // //!Metodo sin uso
    // public void ResetGame()
    // {   
    //     gameState = GameState.PLAYER1;
    //     selectedCard = null;

    //     vikingsDeck.cards.Clear();
    //     lastKingdomDeck.cards.Clear();

    //     gemsVikingsCount = 2;
    //     gemsLastKingdomCount = 2;

    //     gemsVikings.transform.GetChild(0).GetComponent<Image>().sprite = redGem;
    //     gemsVikings.transform.GetChild(1).GetComponent<Image>().sprite = redGem;
    //     gemsLastKingdom.transform.GetChild(0).GetComponent<Image>().sprite = redGem;
    //     gemsLastKingdom.transform.GetChild(1).GetComponent<Image>().sprite = redGem;

    //     player1.transform.Find("Leader").transform.Find("Image").GetComponent<Image>().sprite = leaderImage;
    //     player2.transform.Find("Leader").transform.Find("Image").GetComponent<Image>().sprite = leaderImage;

    //     int n = handVikings.transform.childCount;
    //     for (int i = 0; i < n; i++)
    //     {
    //         handVikings.transform.GetChild(0).parent = null;
    //     }
    //     n = handLastKingdom.transform.childCount;
    //     for (int i = 0; i < n; i++)
    //     {
    //         handLastKingdom.transform.GetChild(0).parent = null;
    //     }

    //     if(discardVikings.transform.childCount > 0)
    //         discardVikings.transform.GetChild(0).parent = null;
    //     if(discardLastkingDom.transform.childCount > 0)
    //         discardLastkingDom.transform.GetChild(0).parent = null;
        
    //     UpdateStats();
    //     InitGame();

    //     menuReset.SetActive(false);
        
    // }
    
    //Iniciar la partida
    void InitGame()
    {   
        //Incializar cartas creadas
        
        vikingsDeck = new Deck();
        
        lastKingdomDeck = new Deck();

        for(int i = 0; i < DataManager.myStringList.Count; i++)
        {
            vikingsDeck.Push(DataManager.myStringList[i]);
            lastKingdomDeck.Push(DataManager.myStringList[i]);
        }

        InitVikingsDeck();
        InitLastKingdomDeck();

        for(int i = 0; i < 10; i++)
        {   
            InstantiateCard(vikingsDeck.cards[i], "Vikings");
            vikingsHand.Push(vikingsDeck.cards[i]);
            vikingsDeck.Remove(vikingsDeck.cards[i]);
        }

        for(int i = 0; i < 10; i++)
        {   
            InstantiateCard(lastKingdomDeck.cards[i], "Last Kingdom");
            lastKingdomHand.Push(lastKingdomDeck.cards[i]);
            lastKingdomDeck.Remove(lastKingdomDeck.cards[i]);
        }

    }

    //Reinicar campos de batalla
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

        int childs = playerField.transform.Find("close").transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = playerField.transform.Find("close").transform.Find("row").GetChild(i);
            child.transform.Find("Frame").transform.GetComponent<Image>().color = new Color(0, 255, 0, 0);
        }
        childs = playerField.transform.Find("range").transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = playerField.transform.Find("range").transform.Find("row").GetChild(i);
            child.transform.Find("Frame").transform.GetComponent<Image>().color = new Color(0, 255, 0, 0);
        }
        childs = playerField.transform.Find("siege").transform.Find("row").childCount;
        for(int i = 0; i < childs; i++)
        {   
            Transform child = playerField.transform.Find("siege").transform.Find("row").GetChild(i);
            child.transform.Find("Frame").transform.GetComponent<Image>().color = new Color(0, 255, 0, 0);
        }
    }

    //Instanciar Carta
    public void InstantiateCard (Card card, string faction) 
    {
        GameObject instantiateCard = Instantiate(cardPrefab);
        instantiateCard.name = card.id.ToString();
        // Debug.Log(card.description);
        if(card.description == "Damage") card.effect = 15;
        if(card.description == "Boost") card.effect = 16;
        if(card.description == "DrawCard") card.effect = 17;
        if(card.description == "DrawAndBoost") card.effect = 18;
        // Debug.Log(card.effect);
        //Stats
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().name = card.name;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().id = card.id;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().faction = card.faction;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().type = card.type;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().power = card.power;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().row = card.row;
        instantiateCard.transform.Find("Stats").GetComponent<CardStats>().effect = card.effect;


        //Image
        if(card.id <= 49)
            instantiateCard.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/Small imgs/" + card.id);
        else
            instantiateCard.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/Small imgs/" + 100);

        //Type
        if (card.type == Card.Type.Golden)
        {
            instantiateCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/goldCard");
        } else if (card.type == Card.Type.Silver)
        { 
            instantiateCard.transform.Find("Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/silverCard");
        } else {
            Destroy(instantiateCard.transform.Find("Type").GetComponent<Image>());
        }  
        //Power
        if (card.type == Card.Type.Golden)
        {
            instantiateCard.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = card.power.ToString();
        } else if (card.type == Card.Type.Silver)
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

    //Inicializar el mazo Vikings
    void InitVikingsDeck()
    {   
        deck1 = new List<Card>();
        deck1.Add(new Unit(5, null, "Ivar the Boneless", Resources.Load<Sprite>("Cards/Small imgs/" + 5), Card.Type.Golden, "description", "Vikings", new List<Card.Position>(){Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 6, "range", 8));
        deck1.Add(new Unit(6, null, "Lagertha, la Guerrera Escudo", Resources.Load<Sprite>("Cards/Small imgs/" + 6), Card.Type.Golden, "description", "Vikings", new List<Card.Position>(){Card.Position.Melee ,Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 7, "close_range", 6));
        deck1.Add(new Unit(7, null, "Rollo, el Berserker", Resources.Load<Sprite>("Cards/Small imgs/" + 7), Card.Type.Golden, "description", "Vikings", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 8, "close", 9));
        deck1.Add(new Unit(11, null, "Bjorn Ironside", Resources.Load<Sprite>("Cards/Small imgs/" + 11), Card.Type.Golden, "description", "Vikings", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 6, "close", 7));
        deck1.Add(new Unit(12, null, "Floki, el Constructor", Resources.Load<Sprite>("Cards/Small imgs/" + 12), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 4, "close", 10));
        deck1.Add(new Unit(13, null, "Floki, el Constructor", Resources.Load<Sprite>("Cards/Small imgs/" + 13), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 4, "close", 10));
        deck1.Add(new Unit(14, null, "Valhalla", Resources.Load<Sprite>("Cards/Small imgs/" + 14), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 3, "siege", 11));
        deck1.Add(new Unit(15, null, "Valhalla", Resources.Load<Sprite>("Cards/Small imgs/" + 15), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 3, "siege", 11));
        deck1.Add(new Unit(16, null, "Valhalla", Resources.Load<Sprite>("Cards/Small imgs/" + 16), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 3, "siege", 11));
        deck1.Add(new Unit(17, null, "Cuervo", Resources.Load<Sprite>("Cards/Small imgs/" + 17), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 2, "range", 6));
        deck1.Add(new Unit(18, null, "Cuervo", Resources.Load<Sprite>("Cards/Small imgs/" + 18), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 2, "range", 6));
        deck1.Add(new Unit(19, null, "Cuervo", Resources.Load<Sprite>("Cards/Small imgs/" + 19), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 2, "range", 6));
        deck1.Add(new Unit(21, null, "Ariete Nórdico", Resources.Load<Sprite>("Cards/Small imgs/" + 21), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 1, "siege", 13));
        deck1.Add(new Unit(22, null, "Ariete Nórdico", Resources.Load<Sprite>("Cards/Small imgs/" + 22), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 1, "siege", 13));
        deck1.Add(new Unit(23, null, "Ariete Nórdico", Resources.Load<Sprite>("Cards/Small imgs/" + 23), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 1, "siege", 13));
        deck1.Add(new Unit(24, null, "Ariete Nórdico", Resources.Load<Sprite>("Cards/Small imgs/" + 24), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 4, "siege", 12));
        deck1.Add(new Unit(25, null, "Ariete Nórdico", Resources.Load<Sprite>("Cards/Small imgs/" + 25), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 4, "siege", 12));
        deck1.Add(new Unit(26, null, "Catapulta Vikinga", Resources.Load<Sprite>("Cards/Small imgs/" + 26), Card.Type.Silver, "description", "Vikings", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 4, "siege", 12));
        deck1.Add(new Clear(10, null, "Ragnarok", Resources.Load<Sprite>("Cards/Small imgs/" + 10), Card.Type.Clear, "description", "Vikings", new List<Card.Position>(){Card.Position.All}, new Onactivation(new List<EffectActivation>()), 0, "all", 4));
        deck1.Add(new Weather(0, null, "Niebla", Resources.Load<Sprite>("Cards/Small imgs/" + 0), Card.Type.Weather, "description", "Neutral", new List<Card.Position>(){Card.Position.Weather}, new Onactivation(new List<EffectActivation>()), 0, "Clima", 3));
        deck1.Add(new Weather(2, null, "Tormenta Nórdica", Resources.Load<Sprite>("Cards/Small imgs/" + 2), Card.Type.Weather, "description", "Neutral", new List<Card.Position>(){Card.Position.Weather}, new Onactivation(new List<EffectActivation>()), 0, "Clima", 2));
        deck1.Add(new Boost(4, null, "Odin", Resources.Load<Sprite>("Cards/Small imgs/" + 4), Card.Type.Boost, "description", "Vikings", new List<Card.Position>(){Card.Position.Boost}, new Onactivation(new List<EffectActivation>()), 0, "Aumento", 0));
        deck1.Add(new Boost(8, null, "Thor", Resources.Load<Sprite>("Cards/Small imgs/" + 8), Card.Type.Boost, "description", "Vikings", new List<Card.Position>(){Card.Position.Boost}, new Onactivation(new List<EffectActivation>()), 0, "Aumento", 1));
        deck1.Add(new Decoy(20, null, "Soldado Distractor", Resources.Load<Sprite>("Cards/Small imgs/" + 20), Card.Type.Decoy, "description", "Neutral", new List<Card.Position>(){Card.Position.All}, new Onactivation(new List<EffectActivation>()), 0, "all", 14));
        //vikingsDeck.Add(new Card("Ragnar Lothbrok", 9, "Vikings", "Lider", 0, "Lider", 5));
        //Desordenar mazo
        foreach(Card card in deck1){
            vikingsDeck.Push(card);
        }
        vikingsDeck.Shuffle();
    }

    //Inicializar el mazo Last Kingdom
    void InitLastKingdomDeck()
    {   
        deck2 = new List<Card>();
        deck2.Add(new Weather(1, null, "Niebla", Resources.Load<Sprite>("Cards/Small imgs/" + 1), Card.Type.Weather, "description", "Neutral", new List<Card.Position>(){Card.Position.Weather}, new Onactivation(new List<EffectActivation>()), 0, "Clima", 3));
        deck2.Add(new Weather(3, null, "Tormenta Nórdica", Resources.Load<Sprite>("Cards/Small imgs/" + 3), Card.Type.Weather, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Weather}, new Onactivation(new List<EffectActivation>()), 0, "Clima", 2));
        deck2.Add(new Unit(28, null, "Alfred el Grande", Resources.Load<Sprite>("Cards/Small imgs/" + 28), Card.Type.Golden, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee, Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 8, "close_range", 9));
        deck2.Add(new Unit(29, null, "Aethelflaed", Resources.Load<Sprite>("Cards/Small imgs/" + 29), Card.Type.Golden, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee, Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 5, "close_range", 5));
        deck2.Add(new Boost(30, null, "Beocca", Resources.Load<Sprite>("Cards/Small imgs/" + 30), Card.Type.Boost, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Boost}, new Onactivation(new List<EffectActivation>()), 0, "Aumento", 0));
        deck2.Add(new Boost(31, null, "Dios Cristiano", Resources.Load<Sprite>("Cards/Small imgs/" + 31), Card.Type.Boost, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Boost}, new Onactivation(new List<EffectActivation>()), 0, "Aumento", 1));
        deck2.Add(new Clear(32, null, "Iglesia Cristiana de Wessex", Resources.Load<Sprite>("Cards/Small imgs/" + 32), Card.Type.Clear, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.All}, new Onactivation(new List<EffectActivation>()), 0, "all", 4));
        deck2.Add(new Unit(33, null, "Leofric", Resources.Load<Sprite>("Cards/Small imgs/" + 33), Card.Type.Golden, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.All}, new Onactivation(new List<EffectActivation>()), 6, "all", 6));
        deck2.Add(new Unit(34, null, "Finan", Resources.Load<Sprite>("Cards/Small imgs/" + 34), Card.Type.Golden, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 6, "close", 7));
        deck2.Add(new Unit(35, null, "Sihtric", Resources.Load<Sprite>("Cards/Small imgs/" + 35), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 4, "close", 10));
        deck2.Add(new Unit(36, null, "Sihtric", Resources.Load<Sprite>("Cards/Small imgs/" + 36), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 4, "close", 10));
        deck2.Add(new Unit(37, null, "Steapa", Resources.Load<Sprite>("Cards/Small imgs/" + 37), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 4, "close", 11));
        deck2.Add(new Unit(38, null, "Steapa", Resources.Load<Sprite>("Cards/Small imgs/" + 38), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 4, "close", 11));
        deck2.Add(new Unit(39, null, "Steapa", Resources.Load<Sprite>("Cards/Small imgs/" + 39), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee}, new Onactivation(new List<EffectActivation>()), 4, "close", 11));
        deck2.Add(new Unit(40, null, "Sigtryggr & Stiorra", Resources.Load<Sprite>("Cards/Small imgs/" + 40), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee, Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 2, "close_range", 6));
        deck2.Add(new Unit(41, null, "Sigtryggr & Stiorra", Resources.Load<Sprite>("Cards/Small imgs/" + 41), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee, Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 2, "close_range", 6));
        deck2.Add(new Unit(42, null, "Sigtryggr & Stiorra", Resources.Load<Sprite>("Cards/Small imgs/" + 42), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Melee, Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 2, "close_range", 6));
        deck2.Add(new Unit(43, null, "Ballesta Gigante", Resources.Load<Sprite>("Cards/Small imgs/" + 43), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 3, "siege", 12));
        deck2.Add(new Unit(44, null, "Ballesta Gigante", Resources.Load<Sprite>("Cards/Small imgs/" + 44), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 3, "siege", 12));
        deck2.Add(new Unit(45, null, "Ballesta Gigante", Resources.Load<Sprite>("Cards/Small imgs/" + 45), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Siege}, new Onactivation(new List<EffectActivation>()), 3, "siege", 12));
        deck2.Add(new Unit(46, null, "Aluvión de Flechas", Resources.Load<Sprite>("Cards/Small imgs/" + 46), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 1, "range", 13));
        deck2.Add(new Unit(47, null, "Aluvión de Flechas", Resources.Load<Sprite>("Cards/Small imgs/" + 47), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 1, "range", 13));
        deck2.Add(new Unit(48, null, "Aluvión de Flechas", Resources.Load<Sprite>("Cards/Small imgs/" + 48), Card.Type.Silver, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.Ranged}, new Onactivation(new List<EffectActivation>()), 1, "range", 13));
        deck2.Add(new Decoy(49, null, "Halcón Mensajero", Resources.Load<Sprite>("Cards/Small imgs/" + 49), Card.Type.Decoy, "description", "Last Kingdom", new List<Card.Position>(){Card.Position.All}, new Onactivation(new List<EffectActivation>()), 0, "all", 14));
        
        foreach(Card card in deck2){
            lastKingdomDeck.Push(card);
        }
        //lastKingdomDeck.Add(new Card("Uhtred de Bebbanburg", 27, "Last Kingdom", "Lider", 0, "Lider", 8));
        //Desordenar mazo
        lastKingdomDeck.Shuffle();
    }

}