using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IStatement : IASTNode
{
    public void Execute(Context context, List<Card> targets);
}

public abstract class Block : IStatement
{
    public readonly static List<TokenType> moduleTypes = new List<TokenType>() {TokenType.For, TokenType.While, TokenType.ClosedBrace};
    public Block(List<IStatement> statements, Token keyword)
    {
        this.statements = statements;
        this.keyword = keyword;
    }
    public Token keyword;
    public Context context;
    public List<IStatement> statements;
    public abstract void Execute(Context context, List<Card> targets);
}