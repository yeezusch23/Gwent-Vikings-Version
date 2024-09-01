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
            //Debug.Log(card.name + ", id, " +  card.faction + ", " + Tools.GetCardTypeString(card.type) + ", " + (int)card.power + ", efecto");

            List<Card.Position> rows = Tools.GetCardPositions(card.position);      
            bool melee = false;
            foreach(Card.Position pos in rows){
                if(pos == Card.Position.Melee) melee = true;
                // Debug.Log(pos);
            }
            bool ranged = false;
            foreach(Card.Position pos in rows){
                if(pos == Card.Position.Ranged) ranged = true;
                // Debug.Log(pos);
            }
            bool siege = false;
            foreach(Card.Position pos in rows){
                if(pos == Card.Position.Siege) siege = true;
                // Debug.Log(pos);
            }
            string row = "";
            if(melee && ranged && siege) {
                row = "all";
            }else if(melee && ranged){
                row = "close_range";
            }else if(melee && siege){
                row = "close_siege";
            }else if(ranged && siege){
                row = "range_siege";
            }else if(melee){
                row = "close";
            }else if(ranged){
                row = "range";
            }else if(siege){
                row = "siege";
            }
        
        // lastKingdomDeck.AddCard(new CardGame(card.name, 100, card.faction, card.type, (int)card.power, , 100));
        // vikingsDeck.AddCard(new CardGame(card.name, 100, card.faction, card.type, (int)card.power, , 100));
        // controller.CreatedCards.AddCard(new CardGame(card.name, 100, card.faction, card.type, (int)card.power, row, 100));
        DataManager.myStringList.Add(new CardGame(card.name, 100, card.faction, card.type, (int)card.power, row, 15));
        // CardGame(nombre, id, faction, Card.Type.Decoy, 0, "Señuelo", 14)
        
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