using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;



[Serializable]
public abstract class Card 
{
    public Card(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect){
        this.id = id;
        this.owner = owner;
        this.name = name;
        this.image = image;
        this.type = type;
        this.description = description;
        this.faction = faction;
        this.positions = positions;
        this.activation = activation;
        this.power = power;
        this.row = row;
        this.effect = effect;
    }
    public int id;
    public Player owner;
    public string name;
    public Sprite image;
    public Type? type;
    public string description;
    public string faction;
    public List<Position> positions;

    public int power;
    public string row;
    public int effect;

    public string field;

    public Onactivation activation;

    public void ActivateEffect(Player triggerplayer)
    {
        activation.Execute(triggerplayer);
    }

    public enum Position
    {
        Melee, Ranged, Siege, All, Weather, Boost, Decoy
    }


    public enum Type
    {
        Leader,
        Golden,
        Silver,
        Weather,
        Boost,
        Decoy,
        Clear,
    }

}

// public class CardGame : Card
// {
//     public CardGame(string name, int id, string faction, Type? type, int power, string row, int effect):
//         base(name, id, faction, type, power, row, effect){}

// }

[Serializable]
public abstract class FieldCard : Card
{
    public FieldCard(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect):
        base(id, owner, name, image, type, description, faction, positions, activation, power, row, effect){
            for (int i = 0; i<4; i++) powers[i] = power;
        }
    /*
    It is necessary to save the values of different power layers 
    powers[0]: holds de basepower value
    powers[1]: holds extra modifications resulting power (user-created effects)
    powers[2]: holds the boostaffected power
    powers[3]: holds the climate affected power
    */

    public int[] powers = new int[4];
}

[Serializable]
public class Unit : FieldCard {
    public Unit(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect):
        base(id,owner, name, image, type, description, faction,positions, activation, power, row, effect){}

}

[Serializable]
public class Decoy : FieldCard
{
    public Decoy(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect):
        base(id,owner, name, image, type, description, faction,positions, activation, power, row, effect){}

}
[Serializable]
public class Weather : Card
{
    public Weather(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect):
        base(id,owner, name, image, type, description, faction,positions, activation, power, row, effect){}
}
[Serializable]
public class Boost : Card
{
    public Boost(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect):
        base(id,owner, name, image, type, description, faction,positions, activation, power, row, effect){}
   
}
[Serializable]
public class Leader : Card {
    public Leader(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect):
        base(id,owner, name, image, type, description, faction,positions, activation, power, row, effect){}
}
[Serializable]
public class Clear : Card
{
    public Clear(int id, Player owner, string name, Sprite image, Type? type, string description, string faction, List<Position> positions, Onactivation activation, int power, string row, int effect):
        base(id,owner, name, image, type, description, faction,positions, activation, power, row, effect){}
    
}
[System.Serializable]
public class DeckFirst
{
    public List<Card> cards;

    public DeckFirst()
    {
        // Constructor: Inicializa la lista de cartas
        cards = new List<Card>();
    }

    public void Shuffle()
    {   
        // Método para barajar las cartas en el mazo
        System.Random rng = new System.Random();
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


public class Deck : GameComponent
{
    public override void Push(Card card)
    {
        cards.Add(card);
    }
    public override Card Pop()
    {   
        Card removed = cards[^1];
        cards.RemoveAt(Size - 1);
        return removed;
    }

    public override void Remove(Card card)
    {
        cards.Remove(card);
    }

    public override void SendBottom(Card card)
    {
        cards.Insert(0, card);
    }

    public Card GetCard(int x = 0)
    {
        // Método para obtener una carta del mazo por su índice
        return cards[x];
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

    public GameComponent hand;
    public GameComponent deck;
    public GameComponent field;
    public GameComponent graveyard;


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

    public Player Other()
    {
        if (this == GlobalContext.gameMaster.Player1) return GlobalContext.gameMaster.Player2;
        return GlobalContext.gameMaster.Player1;
    }
}

[System.Serializable]

public abstract class GameComponent
{
    public Player owner;
    public List<Card> cards = new List<Card>();
    public abstract void Push(Card card);
    public abstract Card Pop();
    public abstract void SendBottom(Card card);
    public abstract void Remove(Card card);
    public int Size {get => cards.Count;}
    public void Shuffle()
    {
        for (int i=cards.Count-1;i>0;i--)
        {
            int randomIndex=UnityEngine.Random.Range(0,i+1);
            Card container=cards[i];
            cards[i]=cards[randomIndex];
            cards[randomIndex]=container;
        }
    }
}

[System.Serializable]
public static class GlobalContext
{
    public static StartGame gameMaster;
    public static GameComponent Board{get => gameMaster.board;}   
    public static GameComponent Hand(Player player)
    {
        return player.hand;
    }
    public static GameComponent Deck(Player player)
    {
        return player.deck;
    }
    public static GameComponent Field(Player player)
    {
        return player.field;
    }
    public static GameComponent Graveyard(Player player)
    {
        return player.graveyard;
    }
}
