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

// Represents an effect in the AST
public class Effect : IASTNode
{
    public static readonly List<TokenType> synchroTypes= new List<TokenType>() {TokenType.Identifier, TokenType.Name, TokenType.ClosedBrace, TokenType.ClosedBracket};
    public string definition;
    public Parameters parameters;
    public Token keyword;

    public void Execute(Player triggerplayer)
    {
        Dictionary<string, object> copy = Tools.CopyDictionary(parameters.parameters);
        Context rootContext = new Context(triggerplayer, null, copy);
        GlobalEffects.effects[definition].action.context = new Context(triggerplayer, rootContext, new Dictionary<string, object>());
        GlobalEffects.effects[definition].Execute();
    }
}

public class Parameters{
    public static readonly List<TokenType> synchroTypes= new List<TokenType>() {TokenType.Identifier, TokenType.ClosedBrace};
    public Dictionary<string, object> parameters;
    public Parameters(Dictionary<string,object> parameters){
        this.parameters=parameters;
    }
}

public static class GlobalEffects
{
    public static Dictionary<string, EffectDefinition> effects= new Dictionary<string, EffectDefinition>();
}

