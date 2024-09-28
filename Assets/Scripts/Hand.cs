using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : GameComponent
{
    public override void Push(Card card)
    {
        cards.Add(card);
    }

    public override void Remove(Card card)
    {
        cards.Remove(card);
    }

    public override Card Pop()
    {
        Card removed = cards[Size - 1];
        cards.RemoveAt(Size - 1);
        return removed;
    }

    public override void SendBottom(Card card)
    {
        cards.Insert(0, card);
    }
}