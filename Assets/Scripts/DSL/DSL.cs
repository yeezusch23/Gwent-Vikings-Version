using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
public static class DSL
{   
    static DSLManager console = GameObject.Find("ButtonCompile").GetComponent<DSLManager>();
    // static StartGame controller = GameObject.Find("StartGame").GetComponent<StartGame>();
    // static DataManager Instance;

    static bool hasError = false;
    public static void AddToConsole(string message)
    {   
        console.Text.text += message + "\n";
    }
    public static void BreakCompilation()
    {
        Debug.LogError("Invalid code\n");
        AddToConsole("Invalid code");
    }
    public static void Compile(string code)
    {   
        console = GameObject.Find("ButtonCompile").GetComponent<DSLManager>();
     
        console.Text.text = "";

        hasError = false;

        if (code == "")
        {
            Debug.LogError("Empty code");
            AddToConsole("Empty code");
            BreakCompilation();
            return;
        }

        Lexer lexer = new Lexer(code);

        var tokens = lexer.GetTokens();

        // foreach (var token in tokens)
        // {
        //     // Debug.Log(token);
        // }

        if(hasError)
        {
            BreakCompilation();
            return;
        }

        Parser parser = new Parser(tokens);
        
        var nodes = parser.ParseProgram();
        if(hasError){
            BreakCompilation();
            return;
        }

        SemanticCheck check = new SemanticCheck(nodes);
        check.CheckProgram(check.AST);
        if(hasError){
            BreakCompilation();
            return;
        }

        Debug.Log("Successfull Compilation");

        foreach(var effect in nodes.nodes.Where(n => n is EffectDefinition).Select(n => (EffectDefinition)n)) {
            
            GlobalEffects.effects[effect.name] = effect;
            
        }

        foreach(var card in nodes.nodes.Where(n => n is CardNode).Select(n => (CardNode)n)){
            Card newcard = null;
            switch(card.type){
                case Card.Type.Silver:
                case Card.Type.Golden:
                    newcard = new Unit(50 + DataManager.myStringList.Count, null, card.name, Resources.Load<Sprite>("DefaultImage"), card.type, card.activation.activations[0].effect.definition, card.faction, Tools.GetCardPositions(card.position), card.activation, (int)card.power, Tools.GetCardRow(Tools.GetCardPositions(card.position)), 100);
                    break;
                case Card.Type.Decoy:
                    newcard = new Decoy(50 + DataManager.myStringList.Count, null, card.name, Resources.Load<Sprite>("DefaultImage"), card.type, card.activation.activations[0].effect.definition, card.faction, Tools.GetCardPositions(card.position), card.activation, 0, Tools.GetCardRow(Tools.GetCardPositions(card.position)), 100);
                    break;
                case Card.Type.Boost:
                    newcard = new Boost(50 + DataManager.myStringList.Count, null, card.name, Resources.Load<Sprite>("DefaultImage"), card.type, card.activation.activations[0].effect.definition, card.faction, Tools.GetCardPositions(card.position), card.activation, 0, Tools.GetCardRow(Tools.GetCardPositions(card.position)), 100);
                    break;
                case Card.Type.Weather:
                    newcard = new Weather(50 + DataManager.myStringList.Count, null, card.name, Resources.Load<Sprite>("DefaultImage"), card.type, card.activation.activations[0].effect.definition, card.faction, Tools.GetCardPositions(card.position), card.activation, 0, Tools.GetCardRow(Tools.GetCardPositions(card.position)), 100);
                    break;
                case Card.Type.Leader:
                    newcard = new Leader(50 + DataManager.myStringList.Count, null, card.name, Resources.Load<Sprite>("DefaultImage"), card.type, card.activation.activations[0].effect.definition, card.faction, Tools.GetCardPositions(card.position), card.activation, 0, Tools.GetCardRow(Tools.GetCardPositions(card.position)), 100);
                    break;
                case Card.Type.Clear:
                    newcard = new Clear(50 + DataManager.myStringList.Count, null, card.name, Resources.Load<Sprite>("DefaultImage"), card.type, card.activation.activations[0].effect.definition, card.faction, Tools.GetCardPositions(card.position), card.activation, 0, Tools.GetCardRow(Tools.GetCardPositions(card.position)), 100);
                    break;
            }
            if(card.activation.activations[0].effect.definition != "DrawCard"){
                if(card.activation.activations[0].effect.parameters.parameters.Count > 0)
                {
                    int x = (int)card.activation.activations[0].effect.parameters.parameters["Amount"];
                    DataManager.myAmountList[card.activation.activations[0].effect.definition] = x;
                }
            }
            DataManager.myStringList.Add(newcard);
        }
    }

    
    public static void Report(int line, int column, string where, string message)
    {
        Debug.LogError($"[Line {line}, Column {column}] {where} Error: " + message);
        AddToConsole($"[Line {line}, Column {column}] {where} Error: " + message);
        hasError = true;
    }
    public static void Error(Token token, string message)
    {
        if (token.type == TokenType.EOF) Report(token.line, token.column, "at end", message);
        else Report(token.line, token.column, $"at'{token.lexeme}'", message);
    }
}
