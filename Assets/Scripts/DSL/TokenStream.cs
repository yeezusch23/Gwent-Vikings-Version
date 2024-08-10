using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
public class TokenStream : IEnumerable<Token>
{   
    private List<Token> tokens;
    private int position;
    public int Position { get { return position; } }   
    string CurrentTokenName { get { return tokens[position].lexeme; } } 

    public TokenStream(IEnumerable<Token> tokens)
    {
        this.tokens = new List<Token>();
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
    bool Check(List<TokenType> types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type)) return true;
        }
        return false;
    }
    void Synchronize(List<TokenType> moduleTypes)
    {
        if (moduleTypes == null) return;
        while (!EOList())
        {
            if (Check(moduleTypes)) return;
            Next();
            if (moduleTypes.Equals(Block.synchroTypes) && tokens[position - 1].type == TokenType.StatementSeparator) return;
        }
    }

    public void Error(Token token, string message, List<TokenType> synchroTypes)
    {
        Synchronize(synchroTypes);
        DSL.Error(token, message);
    }

    //     //Raises an error and reports it
    

    // //Tries to match the current token with a list of types and advances if it matches with any of the tokens in the list
    // bool Match(List<TokenType> types)
    // {
    //     foreach (TokenType type in types)
    //     {
    //         if (Check(type))
    //         {
    //             Advance();
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    // //Tries to match the current token with the given type and advances if it matches
    

    // //Checks if the current token matches the given type
    

    // //TChecks if the current token matches an element from a list of types 


    // //Advances to the next token
    

    



    // //Returns the next token
    // Token PeekNext()
    // {
    //     if (IsAtEnd()) return tokens[current];
    //     else return tokens[current + 1];
    // }

    // //Returns the previous token
    // Token Previous()
    // {
    //     return tokens[current - 1];
    // }

    // //Consumes the current token if it matches the given type, otherwise thwros an error
    // Token Consume(TokenType type, string message, List<TokenType> synchroTypes)
    // {
    //     if (Check(type)) return Advance();
    //     Error(Peek(), message, synchroTypes);
    //     return null;
    // }

    // //Sinchronizes the parser state after an error based on a given list
    // //of tokens that determine where to stop synchronization



    
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