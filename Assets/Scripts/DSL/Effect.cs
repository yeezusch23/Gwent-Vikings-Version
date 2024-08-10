using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

[Serializable]
public class EffectDefinition : IASTNode
{
    public static readonly List<TokenType> moduleTypes= new List<TokenType>() {
        TokenType.Name, TokenType.Params,
        TokenType.Action, TokenType.ClosedBrace
    };
    public string name;
    public ParameterDef parameterdefs;
    public Action action;
    public Token keyword;

    public EffectDefinition() {}
    public void Execute()
    {
        action.Execute(action.context, action.targets);
    }
}

public class ParameterDef : IASTNode{
    public static readonly List<TokenType> moduleTypes= new List<TokenType>() {TokenType.Identifier, TokenType.ClosedBrace};
    public Dictionary<string, ExpressionType> parameters;
    public ParameterDef(Dictionary<string, ExpressionType> parameters){
        this.parameters=parameters;
    }
}

public enum ExpressionType
{
    Number, Bool, String, Card, List, RangeList, Player, Context, Targets, Null,
}

public class Action : Block
{
    public Action(List<IStatement> statements, Token contextID, Token targetsID, Token keyword) : base(statements, keyword)
    {
        this.statements = statements;
        this.contextID = contextID;
        this.targetsID = targetsID;
    }

    public List<Card> targets;
    public Token contextID;
    public Token targetsID;

    public override void Execute(Context context, List<Card> targets)
    {
        context.variables[targetsID.lexeme] = targets;
        foreach (IStatement statement in statements)
        {
            statement.Execute(context, targets);
        }
    }
}