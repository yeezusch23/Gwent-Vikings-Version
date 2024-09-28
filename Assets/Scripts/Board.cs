using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board: GameComponent{

    public Dictionary<string, List<Card>> cardField = new Dictionary<string, List<Card>>();
    public override void Push(Card card)
    {
        cards.Add(card);
        if(!cardField.ContainsKey(card.field))
            cardField[card.field] = new List<Card>();
        cardField[card.field].Add(card);
    }

    public override Card Pop()
    {
        Card removed = cards[Size - 1];
        cards.RemoveAt(Size - 1);
        cardField[removed.field].Remove(removed);
        return removed;
    }

    public override void Remove(Card card)
    {
        cards.Remove(card);
        cardField[card.field].Remove(card);
    }

    public override void SendBottom(Card card)
    {
        cardField[card.field].Insert(0, card);
        cards.Insert(0, card);
    }
}