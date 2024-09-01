using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public static class Tools 
{
    public static Card.Type GetCardType(string s){
        switch (s){
            case "Oro": return Card.Type.Golden;
            case "Plata": return Card.Type.Silver;
            case "Clima": return Card.Type.Weather;
            case "Aumento": return Card.Type.Boost;
            case "Líder": return Card.Type.Leader;
            case "Señuelo": return Card.Type.Decoy;
            case "Despeje": return Card.Type.Clear;
            default: return Card.Type.Null;
        }
    }    

    public static string GetCardTypeString(Card.Type? s){
        switch (s){
            case Card.Type.Golden: return "Oro";
            case Card.Type.Silver: return "Plata";
            case Card.Type.Weather: return "Clima";
            case Card.Type.Boost: return "Aumento";
            case Card.Type.Leader: return "Lider";
            case Card.Type.Decoy: return "Señuelo";
            case Card.Type.Clear: return "Despeje";
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

    public static ExpressionType GetValueType(object value)
    {
        if (value is int) return ExpressionType.Number;
        if (value is string) return ExpressionType.String;
        if (value is bool) return ExpressionType.Bool;
        if (value is Card) return ExpressionType.Card;
        if (value is List<Card>) return ExpressionType.List;
        return ExpressionType.Null;
    }

    public static List<Card.Position> GetCardPositions(List<string> positions){
        List<Card.Position> result =new List<Card.Position>();
        foreach (string position in positions.OrderBy(p => p)){
            switch(position){
                case "Melee": result.Add(Card.Position.Melee); break;
                case "Ranged": result.Add(Card.Position.Ranged); break;;
                case "Siege": result.Add(Card.Position.Siege); break;
                default: throw new ArgumentException("Invalid string position");
            }
        }
        return result;
    }
}