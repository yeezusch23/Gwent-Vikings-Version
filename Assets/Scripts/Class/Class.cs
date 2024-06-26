using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Card 
{
    public string name { get; set; }
    public int id { get; set; }
    public string faction { get; set; }
    public string type { get; set; }
    public int power { get; set; }
    public string row { get; set; }
    public int effect { get; set; }

    public Card(string name, int id, string faction, string type, int power, string row, int effect)   
    {
        this.name = name;
        this.id = id;
        this.faction = faction;
        this.type = type;
        this.power = power;
        this.row = row;
        this.effect = effect;
    }
}

[System.Serializable]
public class Deck
{
    public List<Card> cards;

    public Deck()
    {
        // Constructor: Inicializa la lista de cartas
        cards = new List<Card>();
    }

    public void Shuffle()
    {
        // Método para barajar las cartas en el mazo
        Random rng = new Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public Card GetCard(int x = 0)
    {
        // Método para obtener una carta del mazo por su índice
        return cards[x];
    }

    public void SetCard(int x, Card card)
    {
        // Método para establecer una carta en una posición específica del mazo
        cards[x] = card;
    }

    public void AddCard(Card card)
    {   
        //Agregar carta
        cards.Add(card);
    }

    public void RemoveCard(int x)
    {
        // Método para eliminar una carta del mazo por su índice
        cards.RemoveAt(x);
    }
}

[System.Serializable]
public class EffectList
{
    public List<string> effects;

    public EffectList()
    {
        effects = new List<string>();
    }

    public void setEffect(string effect)
    {
        effects.Add(effect);
    }
}

[System.Serializable]
public class Player
{
    public int power { get; set; }
    public int closePower { get; set; }
    public int rangePower { get; set; }
    public int siegePower { get; set; }
    public int cardsHand { get; set; }
    public int cardsDeck { get; set; }
    public int gems { get; set; }

    public Player()
    {
        power = 0;
        closePower = 0;
        rangePower = 0;
        siegePower = 0;
        cardsHand = 10;
        cardsDeck = 14;
        gems = 2;
    }
}