using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public static class DSL
{   
    static DSLManager console = GameObject.Find("ButtonCompile").GetComponent<DSLManager>();
    
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
        // if(hadError){
        //     CancelCompilation();
        //     return;
        // }

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