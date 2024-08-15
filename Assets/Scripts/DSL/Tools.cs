using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public static class Tools 
{
    public static Card.Type? GetCardType(string s){
        switch (s){
            case "Oro": return Card.Type.Golden;
            case "Plata": return Card.Type.Silver;
            case "Clima": return Card.Type.Weather;
            case "Aumento": return Card.Type.Boost;
            case "Líder": return Card.Type.Leader;
            case "Señuelo": return Card.Type.Decoy;
            case "Despeje": return Card.Type.Clear;
            default: return null;
        }
    }    

    public static Dictionary<TKey, TValue> CopyDictionary<TKey,TValue>(Dictionary<TKey, TValue> dict){
        Dictionary<TKey,TValue> copy = new Dictionary<TKey,TValue>();
        foreach(var pair in dict){
            copy.Add(pair.Key, pair.Value);
        }
        return copy;
    }
}