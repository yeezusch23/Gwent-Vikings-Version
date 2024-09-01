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

// Clase abstracta para métodos de lista
public abstract class Method : IStatement
{
    public Method(IExpression list, Token accessToken)
    {
        this.list = list;
        this.accessToken = accessToken;
    }

    public Token accessToken;
    public IExpression list;
    public abstract void Execute(Context context, List<Card> targets);
}

// Operación pop en listas
public class Pop : Method, ICardAtom
{
    public Pop(IExpression list, Token accessToken) : base(list, accessToken) {}

    public object Evaluate(Context context, List<Card> targets)
    {
        List<Card> evaluation = list.Evaluate(context, targets) as List<Card>;
        if (evaluation.Count == 0) throw new Exception("Cannot Apply Pop method to empty list");
        Card result = evaluation[evaluation.Count - 1];
        Execute(context, targets);
        return result;
    }

    public override void Execute(Context context, List<Card> targets)
    {
        list.Evaluate(context, targets);
        (list as List).gameComponent.Pop();
    }

    public void Set(Context context, List<Card> targets, Card card) {}
}



// Operaciones de incremento y decremento
public class Increment_Decrement : Assignation, IExpression
{
    public Increment_Decrement(IExpression operand, Token operation) : base(operand, null, operation){}
    
    public object Evaluate(Context context, List<Card> targets)
    {
        int result = (int)operand.Evaluate(context, targets);
        Execute(context, targets);
        return result;
    }

    public override void Execute(Context context, List<Card> targets)
    {
        int result = 0;
        if (operation.type == TokenType.Increment) result = (int)operand.Evaluate(context, targets) + 1;
        else result = (int)operand.Evaluate(context, targets) - 1;
        if (operand is PowerAccess) (operand as PowerAccess).Set(context, targets, result);
        else if (operand is Variable) context.Set((operand as Variable).name, result);
    }
}

// Declaración de asignación
public class Assignation : IStatement
{
    public Assignation(IExpression operand, IExpression assignation, Token operation)
    {
        this.operand = operand;
        this.assignation = assignation;
        this.operation = operation;
    }

    public IExpression operand;
    public IExpression assignation;
    public Token operation;

    public virtual void Execute(Context context, List<Card> targets)
    {
        if (operand is ICardAtom) (operand as ICardAtom).Set(context, targets, assignation.Evaluate(context, targets) as Card);
        else if (operand is PropertyAccess) (operand as PropertyAccess).Set(context, targets, assignation.Evaluate(context,targets));
        else if (operand is Variable) context.Set((operand as Variable).name, assignation.Evaluate(context, targets));
    }
}

public abstract class ArgumentMethod: Method{
    public ArgumentMethod(IExpression list,IExpression card, Token accessToken) : base(list,accessToken){
        this.card = card;
    }
    public IExpression card;
}

// Método Push (agrega tarjeta a la lista)
public class Push : ArgumentMethod
{
    public Push(IExpression list, IExpression card, Token accessToken) : base(list,card,accessToken){}

    public override void Execute(Context context, List<Card> targets)
    {
        list.Evaluate(context, targets);
        (list as List).gameComponent.Push((Card)card.Evaluate(context, targets));
    }
}

// Método SendBottom (agrega la tarjeta al final de la lista)
public class SendBottom : ArgumentMethod
{
    public SendBottom(IExpression list, IExpression card, Token accessToken) : base(list,card,accessToken){}


    public override void Execute(Context context, List<Card> targets)
    {
        list.Evaluate(context, targets);
        (list as List).gameComponent.SendBottom((Card)card.Evaluate(context, targets));
    }
}

// Método Remove (elimina la tarjeta de la lista)
public class Remove : ArgumentMethod
{
    public Remove(IExpression list, IExpression card, Token accessToken) : base(list,card,accessToken){}


    public override void Execute(Context context, List<Card> targets)
    {
        list.Evaluate(context, targets);
        (list as List).gameComponent.Remove((Card)card.Evaluate(context, targets));
    }
}

// Método Shuffle (baraja la lista de cartas)
public class Shuffle : Method
{
    public Shuffle(IExpression list, Token accessToken) : base(list,accessToken) {}

    public override void Execute(Context context, List<Card> targets)
    {
        list.Evaluate(context, targets);
        (list as List).gameComponent.Shuffle();
    }
}

// Sentencia de bucle While
public class While : Block
{
    public While(List<IStatement> statements, IExpression predicate, Token keyword) : base(statements, keyword)
    {
        this.predicate = predicate;
    }

    public IExpression predicate;

    public override void Execute(Context context, List<Card> targets)
    {
        this.context = new Context(context.triggerplayer, context, new Dictionary<string, object>());
        while ((bool)predicate.Evaluate(context, targets))
        {
            foreach (IStatement statement in statements)
            {
                statement.Execute(this.context, targets);
            }
        }
    }
}

// Sentencia de bucle foreach
public class Foreach : Block
{
    public Foreach(List<IStatement> statements, IExpression collection, Token variable, Token keyword) : base(statements, keyword)
    {
        this.collection = collection;
        this.variable = variable;
    }

    public Token variable;
    public IExpression collection;

    public override void Execute(Context context, List<Card> targets)
    {
        this.context = new Context(context.triggerplayer, context, new Dictionary<string, object>());

        foreach (Card card in (List<Card>)collection)
        {
            this.context.Set(variable, card);
            foreach (IStatement statement in statements)
            {
                statement.Execute(this.context, targets);
            }
        }
    }
}
