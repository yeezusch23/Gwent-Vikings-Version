using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;


public class TokenStream : IEnumerable<Token>
{   
    public List<Token> tokens;
    public int position;
    public int Position { get { return position; } }   
    string CurrentTokenName { get { return tokens[position].lexeme; } } 

    public TokenStream(List<Token> tokens)
    {
        this.tokens = tokens;
        position = 0;
    }

    public Token Next()
    {
        if (!EOList()) position++;
        return tokens[position - 1];
    }
    public bool Check(TokenType type)
    {
        if (EOList()) return false;
        return Peek().type == type;
    }
    public bool Match(TokenType type)
    {
        if (Check(type))
        {
            Next();
            return true;
        }
        return false;
    }
    public Token Peek()
    {   
        return tokens[position];
    }
    public bool EOList()
    {
        return Peek().type == TokenType.EOF;
    }
    //------------------------------------
    //Manejo de errores
    public bool Check(List<TokenType> types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type)) return true;
        }
        return false;
    }
    void FlowReset(List<TokenType> moduleTypes)
    {
        if (moduleTypes == null) return;
        while (!EOList())
        {
            if (Check(moduleTypes)) return;
            Next();
            if (moduleTypes.Equals(Block.moduleTypes) && tokens[position - 1].type == TokenType.StatementSeparator) return;
        }
    }
    public void Error(Token token, string message, List<TokenType> moduleTypes)
    {
        FlowReset(moduleTypes);
        DSL.Error(token, message);
    }
    public Token Consume(TokenType type, string message, List<TokenType> moduleTypes)
    {
        if (Check(type)) return Next();
        Error(Peek(), message, moduleTypes);
        return null;
    }

    public bool Match(List<TokenType> types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Next();
                return true;
            }
        }
        return false;
    }

    public ExpressionType GetStringType(string s)
    {   
        if(s == "Number") return ExpressionType.Number;
        if(s == "String") return ExpressionType.String;
        if(s == "Bool") return ExpressionType.Bool;
        return ExpressionType.Null;
    }

    //     //Raises an error and reports it
    

    // //Tries to match the current token with a list of types and advances if it matches with any of the tokens in the list
    

    // //Returns the next token
    // Token PeekNext()
    // {
    //     if (IsAtEnd()) return tokens[current];
    //     else return tokens[current + 1];
    // }

    

    
    public IEnumerator<Token> GetEnumerator()
    {
        for (int i = position; i < tokens.Count; i++)
            yield return tokens[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}