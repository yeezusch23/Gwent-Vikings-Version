using System.Collections.Generic;
using UnityEngine;


public class Parser
{
    List<Token> tokens;
    public TokenStream Stream {get; private set;}
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.Stream = new TokenStream(tokens);
    }
    public ProgramNode ParseProgram()
    {
        List<IASTNode> nodes = new List<IASTNode>();
        ProgramNode program = new ProgramNode(nodes);
        while (!Stream.EOList())
        {

            if (Stream.Match(TokenType.effect))
            {
                // nodes.Add(ParseEffect());
                continue;
            }
            if (Stream.Match(TokenType.Card))
            {
                // nodes.Add(ParseCard());
                continue;
            }
            Stream.Error(Stream.Peek(), "Expected Card or Effect", ProgramNode.moduleTypes);
        }
        return program;
    }
}