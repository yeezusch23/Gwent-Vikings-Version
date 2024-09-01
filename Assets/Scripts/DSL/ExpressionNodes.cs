using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public interface IASTNode {}

public interface IExpression : IASTNode
{
    public object Evaluate(Context context, List<Card> targets);
}

public abstract class BinaryOperator : IExpression
{
    public IExpression left;
    public IExpression right;
    public Token operation;

    protected BinaryOperator(IExpression left, IExpression right, Token operation)
    {
        this.left = left;
        this.right = right;
        this.operation = operation;
    }
    public abstract object Evaluate(Context context, List<Card> targets);
}

// Operador de desigualdad
public class Differ : BinaryOperator
{
    public Differ(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return left.Evaluate(context, targets) != right.Evaluate(context, targets);
    }
}

// Operador de igualdaad
public class Equal : BinaryOperator
{
    public Equal(IExpression left, IExpression right, Token token) : base(left, right, token) {}

    public override object Evaluate(Context context, List<Card> targets)
    {
        return left.Evaluate(context, targets) == right.Evaluate(context, targets);
    }
}

public abstract class Atom : IExpression
{
    public static readonly List<TokenType> moduletypes = new List<TokenType>(){
        TokenType.ClosedBracket,TokenType.ClosedBrace, TokenType.Arrow, TokenType.StatementSeparator,
        TokenType.AssignParams,TokenType.ValueSeparator, TokenType.Equal, TokenType.AddEqual, TokenType.MulEqual,
        TokenType.For, TokenType.While, TokenType.SubEqual, TokenType.DivEqual,
    };
    public abstract object Evaluate(Context context, List<Card> targets);
}


public class Literal : Atom
{
    public Literal(object value)
    {
        this.value = value;
    }

    public object value;

    public override object Evaluate(Context context, List<Card> targets)
    {
        return value;
    }
}

public class Variable : Atom
{
    public Variable(Token name)
    {
        this.name = name;
    }

    public Token name;

    public override object Evaluate(Context context, List<Card> targets)
    {
        return context.Get(name);
    }
}

public abstract class PropertyAccess : Atom
{
    public PropertyAccess(IExpression card, Token accessToken)
    {
        this.card = card;
        this.accessToken = accessToken;
    }

    public IExpression card;
    public Token accessToken;

    public abstract void Set(Context context, List<Card> targets, object value);
}

public class RangeAccess : PropertyAccess
{
    public static readonly List<TokenType> synchroTypes = new List<TokenType>() {TokenType.ValueSeparator, TokenType.ClosedBracket, TokenType.ClosedBrace};
    public RangeAccess(IExpression card, Token accessToken) : base(card, accessToken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Card aux = (Card)card.Evaluate(context, targets);

        List<Card.Position> positions = aux.positions;

        List<string> result =new List<string>();
        foreach (Card.Position position in positions){
            string add="";
            switch(position){
                case Card.Position.Melee: add="Close"; break;
                case Card.Position.Ranged: add="Range"; break;
                case Card.Position.Siege: add="Siege"; break;
                //TODO: Imprimr error en consola
                default: throw new ArgumentException("Invalid  position");
            }
            result.Add(add);
        }
        return result;
    }

    public override void Set(Context context, List<Card> targets, object value)
    {
        (card.Evaluate(context, targets) as Card).positions = (List<Card.Position>)value;
    }
}

public class IndexedRange: IExpression{
    public IExpression range;
    public IExpression index;   
    public Token indexedToken;
    public IndexedRange(IExpression range, IExpression index, Token indexedToken){
        this.range = range;
        this.index = index;
        this.indexedToken = indexedToken;
    }

    public object Evaluate(Context context, List<Card> targets){
        return (range.Evaluate(context,targets) as List<Card.Position>)[(int)index.Evaluate(context,targets)];
    }
}

public interface ICardAtom : IExpression
{
    public void Set(Context context, List<Card> targets, Card card);
}

public class IndexedCard : ICardAtom
{
    public IndexedCard(IExpression index, IExpression list, Token indexToken)
    {
        this.index = index;
        this.list = list;
        this.indexToken = indexToken;
    }

    public IExpression index;
    public IExpression list;
    public Token indexToken;

    public object Evaluate(Context context, List<Card> targets)
    {
        var evaluation = list.Evaluate(context, targets) as List<Card>;
        return evaluation[Math.Max(evaluation.Count, (int)index.Evaluate(context, targets))];
    }

    public void Set(Context context, List<Card> targets, Card card)
    {
        var evaluation = list.Evaluate(context, targets) as List<Card>;
        evaluation[Math.Max(evaluation.Count, (int)index.Evaluate(context, targets))] = card;
    }
}

// Clase abstracta para listas específicas de un jugador
public abstract class IndividualList : List
{
    //Este campo no se utiliza en el método de evaluación, es solo para la comprobación semántica.
    //Es por eso que en los casos en los que no se necesita una comprobación semántica, tendrá un valor nulo.
    public IExpression context;
    public Token playertoken;
    public IExpression player;
    public IndividualList(IExpression context, IExpression player, Token accessToken, Token playertoken) : base(accessToken)
    {
        this.context=context;
        this.player = player;
        this.playertoken = playertoken;
    }
}
// Clase abstracta para listas de cards
public abstract class List : Atom {
    public List(Token accesToken){
        this.accessToken = accesToken;
    }
    public Token accessToken;
    public GameComponent gameComponent;
}

// Lista de cartas en el mazo de un jugador
public class DeckList : IndividualList
{
    public DeckList(IExpression context,IExpression player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Player targetPlayer = (Player)player.Evaluate(context, targets);
        gameComponent = GlobalContext.Deck(targetPlayer);
        return gameComponent.cards;
    }
}

// Lista de cartas en el cementerio de un jugador
public class GraveyardList : IndividualList
{
    public GraveyardList(IExpression context,IExpression player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Player targetPlayer = (Player)player.Evaluate(context, targets);
        gameComponent = GlobalContext.Graveyard(targetPlayer);
        return gameComponent.cards;    
    }
}

// Lista de cartas en el campo de un jugador
public class FieldList : IndividualList
{
    public FieldList(IExpression context,IExpression player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Player targetPlayer = (Player)player.Evaluate(context, targets);
        gameComponent = GlobalContext.Field(targetPlayer);
        return gameComponent.cards;
    }
}

// Lista de cartas en la mano de un jugador
public class HandList : IndividualList
{
    public HandList(IExpression context, IExpression player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Player targetPlayer = (Player)player.Evaluate(context, targets);
        gameComponent = GlobalContext.Hand(targetPlayer);
        return gameComponent.cards;
    }
}

public class TriggerPlayer : Atom
{
    public override object Evaluate(Context context, List<Card> targets)
    {
        return context.triggerplayer;
    }
}

// Lista de cartas en el tablero
public class BoardList : List
{
    public BoardList(IExpression context, Token accessToken) : base(accessToken){
        this.context=context;
    }
    public IExpression context;
    public override object Evaluate(Context context, List<Card> targets)
    {
        return GlobalContext.Board.cards;
    }
}

// Lista de tarjetas filtradas por un predicado
public class ListFind : List
{
    public ListFind() : base(null){ }

    public ListFind(IExpression list, IExpression predicate, Token parameter, Token accessToken, Token argumentToken) : base(accessToken)
    {
        this.list = list;
        this.predicate = predicate;
        this.parameter = parameter;
        this.argumentToken = argumentToken;
    }

    public IExpression list;
    public IExpression predicate;
    public Token parameter;
    public Token argumentToken;

    public override object Evaluate(Context context, List<Card> targets)
    {
       //Guardar el valor de la variable si existe en el contexto
        object card = 0;
        List<Card> result = new List<Card>();
        bool usedvariable = false;
        if (context.variables.ContainsKey(parameter.lexeme))
        {
            card = context.variables[parameter.lexeme];
            usedvariable = true;
        }

        // Evalúa el predicado de cada carta de la lista
        foreach (Card listcard in (List<Card>)list.Evaluate(context, targets))
        {
            context.Set(parameter, listcard);
            if ((bool)predicate.Evaluate(context, targets)) result.Add(listcard);
        }

        // Restaurar el valor de la variable original si se utilizó
        if (usedvariable) context.Set(parameter, card);
        else context.variables.Remove(parameter.lexeme);

        return result;
    }
}

// Propiedad del nombre de la card de acceso
public class NameAccess : PropertyAccess
{
    public NameAccess(IExpression card, Token accessToken) : base(card, accessToken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Card aux = (Card)card.Evaluate(context, targets);
        return aux.name;
    }

    public override void Set(Context context, List<Card> targets, object value)
    {
        (card.Evaluate(context, targets) as Card).name = (string)value;
    }
}

// Propiedad power de la card de acceso
public class PowerAccess : PropertyAccess
{
    public PowerAccess(IExpression card, Token accessToken) : base(card, accessToken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Card aux = (Card)card.Evaluate(context, targets);
        if (aux is FieldCard)
        {
            return (card.Evaluate(context, targets) as FieldCard).powers[3];
        }
        else throw new InvalidOperationException("Card doesn't contain power field");
    }

    public override void Set(Context context, List<Card> targets, object value)
    {
        (card.Evaluate(context, targets) as FieldCard).powers[1] = (int)value;
    }
}

public class FactionAccess : PropertyAccess
{
    public FactionAccess(IExpression card, Token accessToken) : base(card, accessToken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Card aux = (Card)card.Evaluate(context, targets);
        return aux.faction;
    }

    public override void Set(Context context, List<Card> targets, object value)
    {
        (card.Evaluate(context, targets) as Card).faction = (string)value;
    }
}

public class TypeAccess : PropertyAccess
{
    public TypeAccess(IExpression card, Token accessToken) : base(card, accessToken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Card aux = (Card)card.Evaluate(context, targets);
        return aux.type;
    }

    public override void Set(Context context, List<Card> targets, object value)
    {   
        (card.Evaluate(context, targets) as Card).type = Tools.GetCardType((string)value);
    }
}

public class OwnerAccess : PropertyAccess
{
    public OwnerAccess(IExpression card, Token accessToken) : base(card, accessToken) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        Card aux = (Card)card.Evaluate(context, targets);
        return aux.owner;
    }

    public override void Set(Context context, List<Card> targets, object value){}
}

// Clase abstracta para operadores unarios en expresiones
public abstract class Unary : Atom
{
    public Token operation;
    public IExpression right;
    public Unary(IExpression right, Token operation)
    {
        this.right = right;
        this.operation = operation;
    }
}

// Operador de negación lógica
public class Negation : Unary
{
    public Negation(IExpression right, Token operation) : base(right, operation) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return !(bool)right.Evaluate(context, targets);
    }
}

// Operador de negación aritmética
public class Negative : Unary
{
    public Negative(IExpression right, Token operation) : base(right, operation) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return -(int)right.Evaluate(context, targets);
    }
}

// Operador de potencia (exponenciación)
public class Power : BinaryOperator
{
    public Power(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return OptimizedPower((int)left.Evaluate(context, targets), (int)right.Evaluate(context, targets));
    }

    // Método auxiliar para calcular la potencia de manera eficiente
    static int OptimizedPower(int argument, int power)
    {
        int result = 1;
        for (; power >= 0; power /= 2, argument = argument * argument)
        {
            if (power % 2 == 1) result = result * argument;
        }
        return result;
    }
}

// Operador de producto
public class Product : BinaryOperator
{
    public Product(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) * (int)right.Evaluate(context, targets);
    }
}

// Operador de división
public class Division : BinaryOperator
{
    public Division(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) / (int)right.Evaluate(context, targets);
    }
}

// Operador de adición
public class Add : BinaryOperator
{
    public Add(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) + (int)right.Evaluate(context, targets);
    }
}

// Operador de resta
public class Sub : BinaryOperator
{
    public Sub(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) - (int)right.Evaluate(context, targets);
    }
}

// Mayor que el operador
public class Greater : BinaryOperator
{
    public Greater(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) > (int)right.Evaluate(context, targets);
    }
}

// Operador menor o igual
public class GreaterEqual : BinaryOperator
{
    public GreaterEqual(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) <= (int)right.Evaluate(context, targets);
    }
}

// Menos que operador
public class Less : BinaryOperator
{
    public Less(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) < (int)right.Evaluate(context, targets);
    }
}

// Operador menor o igual
public class LessEqual : BinaryOperator
{
    public LessEqual(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (int)left.Evaluate(context, targets) >= (int)right.Evaluate(context, targets);
    }
}

// Operador de concatenación de cadenas
public class Join : BinaryOperator
{
    public Join(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (string)left.Evaluate(context, targets) + (string)right.Evaluate(context, targets);
    }
}

// Concatenación de cadenas con operador de espacio
public class SpaceJoin : BinaryOperator
{
    public SpaceJoin(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (string)left.Evaluate(context, targets) + " " + (string)right.Evaluate(context, targets);
    }
}

// Operador lógico OR
public class Or : BinaryOperator
{
    public Or(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (bool)left.Evaluate(context, targets) || (bool)right.Evaluate(context, targets);
    }
}

// Operador lógico AND
public class And : BinaryOperator
{
    public And(IExpression left, IExpression right, Token token) : base(left, right, token) { }

    public override object Evaluate(Context context, List<Card> targets)
    {
        return (bool)left.Evaluate(context, targets) && (bool)right.Evaluate(context, targets);
    }
}

// Operaciones de modificación numérica (por ejemplo, +=, -=, etc.)
public class NumericModification : Assignation
{
    public NumericModification(IExpression operand, IExpression assignation, Token operation) : base(operand, assignation ,operation){}

    public override void Execute(Context context, List<Card> targets)
    {
        object result = null;
        switch (operation.type)
        {
            case TokenType.AddEqual: result = (int)operand.Evaluate(context, targets) + (int)assignation.Evaluate(context, targets); break;
            case TokenType.SubEqual: result = (int)operand.Evaluate(context, targets) - (int)assignation.Evaluate(context, targets); break;
            case TokenType.DivEqual: result = (int)operand.Evaluate(context, targets) * (int)assignation.Evaluate(context, targets); break;
            case TokenType.MulEqual: result = (int)operand.Evaluate(context, targets) / (int)assignation.Evaluate(context, targets); break;
            case TokenType.ConcatEqual: result = (string)operand.Evaluate(context, targets) + (string)assignation.Evaluate(context, targets); break;
        }
        if (operand is PowerAccess) (operand as PowerAccess).Set(context, targets, result);
        else if (operand is Variable) context.Set((operand as Variable).name, result);
    }
}

// Representa una tarjeta en el AST
public class CardNode : IASTNode
{
    public static readonly List<TokenType> synchroTypes = new List<TokenType>() {
        TokenType.Name, TokenType.Type , TokenType.Faction, TokenType.Power,
        TokenType.Range, TokenType.OnActivation, TokenType.ClosedBrace
    };

    public string name;
    public string faction;
    public Card.Type? type;
    public int? power;
    public List<string> position;
    public Onactivation activation;
    public Token keyword;
}

[Serializable]
public class Onactivation : IASTNode
{
    public static readonly List<TokenType> synchroTypes= new List<TokenType>() {TokenType.OpenBrace, TokenType.ClosedBracket, TokenType.ValueSeparator};
    public Onactivation(List<EffectActivation> activations)
    {
        this.activations = activations;
    }

    public List<EffectActivation> activations;

    public void Execute(Player triggerplayer)
    {
        foreach (EffectActivation activation in activations)
        {
            activation.Execute(triggerplayer);
        }
    }
}

// Representa una activación de efecto en el AST
[Serializable]
public class EffectActivation : IASTNode
{
    public static readonly List<TokenType> synchroTypes= new List<TokenType>() {TokenType.Effect, TokenType.Selector, TokenType.PostAction, TokenType.ClosedBrace, TokenType.ClosedBracket};
    public Effect effect;
    public Selector selector;
    public EffectActivation postAction;

    public void Execute(Player triggerplayer)
    {
        if(selector != null){
            switch (selector.source.literal)
            {
                case "board": selector.filtre.list = new BoardList(null,null); break;
                case "hand": selector.filtre.list = new HandList(null,new Literal(triggerplayer), null, null); break;
                case "otherHand": selector.filtre.list = new HandList(null,new Literal(triggerplayer.Other()), null, null); break;
                case "deck": selector.filtre.list = new DeckList(null,new Literal(triggerplayer), null, null); break;
                case "otherDeck": selector.filtre.list = new DeckList(null,new Literal(triggerplayer.Other()), null, null); break;
                case "graveyard": selector.filtre.list = new GraveyardList(null,new Literal(triggerplayer), null, null); break;
                case "otherGraveyard": selector.filtre.list = new GraveyardList(null,new Literal(triggerplayer.Other()), null, null); break;
                case "field": selector.filtre.list = new FieldList(null,new Literal(triggerplayer), null, null); break;
                case "otherField": selector.filtre.list = new FieldList(null,new Literal(triggerplayer.Other()), null, null); break;
            }
            if (postAction.selector == null) postAction.selector = selector;
            else if ((string)postAction.selector.source.literal == "parent") postAction.selector.filtre.list = selector.filtre;
            var temp = selector.Select(triggerplayer);
            if ((bool)selector.single && temp.Count > 0)
            {
                List<Card> singlecard = new List<Card>() { temp[0] };
                GlobalEffects.effects[effect.definition].action.targets = singlecard;
            }
            else GlobalEffects.effects[effect.definition].action.targets = temp;
        }
        else GlobalEffects.effects[effect.definition].action.targets=new List<Card>();
        effect.Execute(triggerplayer);
        postAction.Execute(triggerplayer);
    }
}

// Se utilizó el objeto ListFind con el método Evaluate de selección basado en predicados
public class Selector : IASTNode
{
    public static readonly List<TokenType> synchroTypes = new List<TokenType> {TokenType.Source, TokenType.Single, TokenType.Predicate, TokenType.ClosedBrace, TokenType.OpenBracket};
    public Selector() { }
    public Token source;
    public bool? single;
    public ListFind filtre;

    public List<Card> Select(Player triggerplayer)
    {
        return (List<Card>)filtre.Evaluate(new Context(), new List<Card>());
    }
}