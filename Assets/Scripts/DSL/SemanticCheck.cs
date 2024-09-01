using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

// Clase responsable de realizar comprobaciones semánticas en el árbol de sintaxis abstracta (AST)
public class SemanticCheck
{
    // Almacena la raíz del árbol de sintaxis abstracta (AST)
    public ProgramNode AST;

    // Constructor de la clase SemanticCheck, que inicializa el diccionario de tipos de expresión y AST
    public SemanticCheck(ProgramNode node)
    {
        AST = node;
        expressiontypes = new Dictionary<IExpression, ExpressionType>();
    }

    // Método para reportar errores semánticos, tomando un token y un mensaje de error
    public static void SemanticError(Token token, string message)
    {
        DSL.Error(token, message);
    }

    // Diccionario para almacenar los tipos de expresiones encontradas durante el análisis
    public Dictionary<IExpression, ExpressionType> expressiontypes;

    public bool MatchType(ExpressionType target, List<ExpressionType> types)
    {
        foreach (var type in types)
        {
            if (target == type) return true;
        }
        return false;
    }

    public void CheckExpression(IExpression expression, Scope scope)
    {
        // Declaración Switch para manejar diferentes tipos de expresiones
        switch (expression)
        {
            case Add:
            case Sub:
            case Product:
            case Division:
            case Power: CheckBinaryOperator((BinaryOperator)expression, ExpressionType.Number, ExpressionType.Number, scope); break;
            case Equal:
            case Differ: CheckEqualityOperator((BinaryOperator)expression, scope); break;
            case GreaterEqual:
            case LessEqual:
            case Less:
            case Greater: CheckBinaryOperator((BinaryOperator)expression, ExpressionType.Number, ExpressionType.Bool, scope); break;
            case And:
            case Or: CheckBinaryOperator((BinaryOperator)expression, ExpressionType.Bool, ExpressionType.Bool, scope); break;
            case SpaceJoin:
            case Join: CheckBinaryOperator((BinaryOperator)expression, ExpressionType.String, ExpressionType.String, scope); break;
            case List list: CheckList(list, scope); break;
            case Negative negative: CheckUnary(negative, ExpressionType.Number, scope); break;
            case Negation negation: CheckUnary(negation, ExpressionType.Bool, scope); break;
            case Literal literal: CheckLiteral(literal); break;
            case Variable variable: CheckVariable(variable, scope); break;
            case ICardAtom card: CheckCardAtom(card, scope); break;
            case IndexedRange indrange: CheckIndexedRange(indrange, scope); break;
            case TriggerPlayer player: CheckTriggerPlayer(player); break;
            case Increment_Decrement indec: CheckIncrement_Decrement(indec, scope); break;
            case PropertyAccess access: CheckPropertyAccess(access, scope); break;
            case null: break;
            default: throw new Exception("Not handled expression types");
        }
    }

    // Comprueba los operadores binarios, garantizando que ambos operandos coincidan con los tipos esperados
    public void CheckBinaryOperator(BinaryOperator binaryOp, ExpressionType operandtypes, ExpressionType operatortype, Scope scope)
    {
        CheckExpression(binaryOp.left, scope);
        CheckExpression(binaryOp.right, scope);
        if (expressiontypes[binaryOp.left] != operandtypes)
            SemanticError(binaryOp.operation, $"Left operand must be a {operandtypes}");
        if (expressiontypes[binaryOp.right] != operandtypes)
            SemanticError(binaryOp.operation, $"Right operand must be a {operandtypes}");
        expressiontypes[binaryOp] = operatortype;
    }


    public void CheckEqualityOperator(BinaryOperator binaryOp, Scope scope)
    {
        List<ExpressionType> types = new List<ExpressionType>() { ExpressionType.Number, ExpressionType.String, ExpressionType.Bool, ExpressionType.Player};
        CheckExpression(binaryOp.left, scope);
        CheckExpression(binaryOp.right, scope);
        if (!MatchType(expressiontypes[binaryOp.left], types))
            SemanticError(binaryOp.operation, $"Left operand must be Player, String, Number or Bool");
        if (!MatchType(expressiontypes[binaryOp.right], types))
            SemanticError(binaryOp.operation, $"Right operand must be Player, String, Number or Bool");
        if (expressiontypes[binaryOp.left] != expressiontypes[binaryOp.right])
            SemanticError(binaryOp.operation, "Invalid comparation, operands must be of the same type");
        expressiontypes[binaryOp] = ExpressionType.Bool;
    }

    // Comprueba listas y maneja casos específicos como IndividualList y ListFind
    public void CheckList(List list, Scope scope)
    {
        expressiontypes[list] = ExpressionType.List;
        if (list is IndividualList indlist)
        {
            CheckExpression(indlist.context, scope);
            if (expressiontypes[indlist.context] != ExpressionType.Context)
                SemanticError(indlist.accessToken, "Invalid individual list, must be called by context");
            CheckExpression(indlist.player, scope);
            if (expressiontypes[indlist.player] != ExpressionType.Player)
                SemanticError(indlist.playertoken, "Individual list argument must be a player");
            return;
        }
        if (list is ListFind find)
        {
            scope.Set(find.parameter, ExpressionType.Card);
            CheckExpression(find.list, scope);
            CheckExpression(find.predicate, scope);
            if (expressiontypes[find.list] != ExpressionType.List && expressiontypes[find.list] != ExpressionType.Targets)
                SemanticError(find.accessToken, "Find method must be called by a list or targets");
            if (expressiontypes[find.predicate] != ExpressionType.Bool)
                SemanticError(find.argumentToken, "Find method predicate must be a boolean expression");
            return;
        }

    }

    // Comprueba las operaciones unarias, garantizando que el operando correcto coincida con el tipo esperado
    public void CheckUnary(Unary unary, ExpressionType type, Scope scope)
    {
        CheckExpression(unary.right, scope);
        if (expressiontypes[unary.right] != type)
            SemanticError(unary.operation, $"Right operand must be a {type}");
        expressiontypes[unary] = type;
    }

    // Comprueba literales y establece su tipo en función del valor.
    public void CheckLiteral(Literal literal)
    {
        expressiontypes[literal] = Tools.GetValueType(literal.value);
    }

    // Comprueba las variables contra el alcance
    public void CheckVariable(Variable variable, Scope scope)
    {
        expressiontypes[variable] = scope.Get(variable.name);
    }

    // Comprueba los átomos de las tarjetas y maneja casos especiales como IndexedCard y Pop
    public void CheckCardAtom(ICardAtom card, Scope scope)
    {
        expressiontypes[card] = ExpressionType.Card;
        if (card is IndexedCard indexed)
        {
            CheckExpression(indexed.list, scope);
            CheckExpression(indexed.index, scope);
            if (expressiontypes[indexed.list] != ExpressionType.List && expressiontypes[indexed.list] != ExpressionType.Targets)
                SemanticError(indexed.indexToken, $"Cannot apply indexing to a non-list expression");
            if (expressiontypes[indexed.index] != ExpressionType.Number)
                SemanticError(indexed.indexToken, $"Indexer must be a Number");
            return;
        }
        if (card is Pop pop)
        {
            CheckExpression(pop.list, scope);
            if (expressiontypes[pop.list] != ExpressionType.List && expressiontypes[pop.list] != ExpressionType.Targets)
                SemanticError(pop.accessToken, $"Pop method must be called by a List or targets");
            return;
        }
    }

    // Verifica los accesos a la propiedad, asegurándose de que la carta a la que se accede sea del tipo correcto.
    public void CheckPropertyAccess(PropertyAccess access, Scope scope)
    {
        CheckExpression(access.card, scope);
        if (expressiontypes[access.card] != ExpressionType.Card)
            SemanticError(access.accessToken, $"Card property access must be called by a Card");
        ExpressionType type = ExpressionType.Null;
        switch (access)
        {
            case PowerAccess: type = ExpressionType.Number; break;
            case TypeAccess:
            case FactionAccess:
            case NameAccess: type = ExpressionType.String; break;
            case RangeAccess: type = ExpressionType.RangeList; break;
            case OwnerAccess: type = ExpressionType.Player; break;
        }
        expressiontypes[access] = type;
    }

    // Comprueba los rangos indexados, garantizando que el rango y el índice sean del tipo correcto
    public void CheckIndexedRange(IndexedRange indexedRange, Scope scope)
    {
        CheckExpression(indexedRange.range, scope);
        CheckExpression(indexedRange.index, scope);
        if (expressiontypes[indexedRange.range] != ExpressionType.RangeList)
            SemanticError(indexedRange.indexedToken, $"Cannot apply range indexing to non-range expression");
        if (expressiontypes[indexedRange.index] != ExpressionType.Number)
            SemanticError(indexedRange.indexedToken, $"Indexer must be a Number");
        expressiontypes[indexedRange] = ExpressionType.String;
    }

    // Comprueba los triggerplayernodes y establece su tipo en Jugador
    public void CheckTriggerPlayer(TriggerPlayer player)
    {
        expressiontypes[player] = ExpressionType.Player;
    }

    public void CheckIncrement_Decrement(Increment_Decrement incdec, Scope scope)
    {
        expressiontypes[incdec] = ExpressionType.Number;
        CheckExpression(incdec.operand, scope);
        if (expressiontypes[incdec.operand] != ExpressionType.Number)
            SemanticError(incdec.operation, "Increment or decrement must be aplied to Number operand");
    }


    // Verifica declaraciones, manejando diferentes tipos de declaraciones como Asignación, Método y Bucles
    public void CheckStatement(IStatement statement, Scope scope)
    {
        switch (statement)
        {
            case Assignation assignation: CheckAssignation(assignation, scope); break;
            case Method method: CheckMethod(method, scope); break;
            case While loop: CheckWhile(loop, scope); break;
            case Foreach loop: CheckForEach(loop, scope); break;
        }
    }

    // Verifica las asignaciones, asegurándose de que los lados izquierdo y derecho coincidan en tipo y respalden la operación.
    public void CheckAssignation(Assignation assignation, Scope scope)
    {
        CheckExpression(assignation.operand, scope);
        CheckExpression(assignation.assignation, scope);
        switch (assignation.operand)
        {
            case Variable: CheckVariableAssignation(assignation, scope); break;
            case PropertyAccess: CheckPropertyAssignation(assignation, scope); break;
            case ICardAtom: CheckCardAssignation(assignation, scope); break;
            default: SemanticError(assignation.operation, "Invalid assignation operand"); break;
        }
    }

    // Verifica las asignaciones de tarjetas, asegurando que la operación sea válida y el valor sea una tarjeta.
    public void CheckCardAssignation(Assignation assignation, Scope scope)
    {
        if (assignation.operation.type != TokenType.Equal)
            SemanticError(assignation.operation, "Invalid card assignation operation");
        if (assignation.assignation == null || expressiontypes[assignation.assignation] != ExpressionType.Card)
            SemanticError(assignation.operation, "Invalid assignation, assignation value must be card");
    }

    // Comprueba las asignaciones de propiedades, garantizando que la operación sea válida y que el valor coincida con el tipo de propiedad.
    public void CheckPropertyAssignation(Assignation assignation, Scope scope)
    {
        switch (assignation.operand)
        {
            case NameAccess:
            case FactionAccess:
                if (assignation.operation.type != TokenType.Equal && assignation.operation.type != TokenType.ConcatEqual)
                    SemanticError(assignation.operation, "Invalid String property assignation");
                if (assignation.assignation == null || expressiontypes[assignation.assignation] != ExpressionType.String)
                    SemanticError(assignation.operation, "Invalid assignation, assignation value must be a String");
                break;
            case TypeAccess:
                if (assignation.operation.type != TokenType.Equal)
                    SemanticError(assignation.operation, "Invalid Type property assignation");
                if (assignation.assignation == null || expressiontypes[assignation.assignation] != ExpressionType.String)
                    SemanticError(assignation.operation, "Invalid assignation, assignation value must be a String");
                break;
            case PowerAccess:
                if (assignation.operation.type == TokenType.ConcatEqual)
                    SemanticError(assignation.operation, "Invalid Power property assignation");
                if (!(assignation is Increment_Decrement) && (assignation.assignation == null || expressiontypes[assignation.assignation] != ExpressionType.Number))
                    SemanticError(assignation.operation, "Invalid assignation, assignation value must be a Number");
                break;
            case RangeAccess: SemanticError(assignation.operation, "Range access cannot be an assignment operand"); break;
            default: break;
        }
    }

    //Comprueba las asignaciones de variables, garantizando que los tipos coincidan y manejando las declaraciones.
    public void CheckVariableAssignation(Assignation assignation, Scope scope)
    {
        if (expressiontypes[assignation.operand] == ExpressionType.Null)
        {
            if (assignation.operation.type != TokenType.Equal)
            {
                SemanticError(assignation.operation, "Invalid operation in non-declared variable");
                return;
            }
            scope.Set((assignation.operand as Variable).name, expressiontypes[assignation.assignation]);
        }
        else
        {
            switch (assignation.operation.type)
            {
                case TokenType.Increment:
                case TokenType.Decrement:
                    if (expressiontypes[assignation.operand] != ExpressionType.Number)
                        SemanticError(assignation.operation, "Increment or decrement operand must be number");
                    break;
                case TokenType.Concat:
                case TokenType.ConcatConcat:
                    if (expressiontypes[assignation.operand] != ExpressionType.String)
                        SemanticError(assignation.operation, "Invalid assignation, operand must be String");
                    if (expressiontypes[assignation.assignation] != ExpressionType.String)
                        SemanticError(assignation.operation, "Invalid assignation, assignation value must be String");
                    break;
                case TokenType.Equal:
                    if (expressiontypes[assignation.operand] != expressiontypes[assignation.assignation])
                        SemanticError(assignation.operation, $"Invalid assignation, types mismatch, expected {expressiontypes[assignation.operand]} value");
                    break;
                default:
                    if (expressiontypes[assignation.operand] != ExpressionType.Number)
                        SemanticError(assignation.operation, "Invalid assignation, operand must be Number");
                    if (expressiontypes[assignation.assignation] != ExpressionType.Number)
                        SemanticError(assignation.operation, "Invalid assignation, assignation value must be Number");
                    break;
            }
        }
    }

    // Comprueba las listas de métodos y argumentos y garantiza que tengan el tipo correcto
    public void CheckMethod(Method method, Scope scope)
    {
        CheckExpression(method.list, scope);
        if (!(method.list is IndividualList || method.list is BoardList))
            SemanticError(method.accessToken, "Method must be called by a Context List (Hand, Deck, Field, Graveyard, Board)");
        if (method is ArgumentMethod argMethod)
        {
            CheckExpression(argMethod.card, scope);
            if (expressiontypes[argMethod.card] != ExpressionType.Card)
                SemanticError(method.accessToken, "Invalid method argument, expected card argument");
        }
    }

    public void CheckForEach(Foreach loop, Scope scope)
    {
        Scope newScope = new Scope(scope);
        if (scope.types.ContainsKey(loop.variable.lexeme))
            SemanticError(loop.variable, "Loop variable was already declared in the scope");
        CheckExpression(loop.collection, scope);
        if (expressiontypes[loop.collection] == ExpressionType.List || expressiontypes[loop.collection] == ExpressionType.Targets)
            newScope.Set(loop.variable, ExpressionType.Card);
        else if (expressiontypes[loop.collection] == ExpressionType.RangeList)
            newScope.Set(loop.variable, ExpressionType.String);
        else SemanticError(loop.keyword, "Invalid collection, expected card range or card list");
        foreach (IStatement statement in loop.statements)
        {
            if (statement is Foreach || statement is While) CheckStatement(statement, newScope);
            else CheckStatement(statement, scope);
        }
    }
    public void CheckWhile(While loop, Scope scope)
    {
        CheckExpression(loop.predicate, scope);
        if (expressiontypes[loop.predicate] != ExpressionType.Bool)
            SemanticError(loop.keyword, "Invalid predicate, expected Bool");
        foreach (IStatement statement in loop.statements)
        {
            if (statement is Foreach || statement is While) CheckStatement(statement, new Scope(scope));
            else CheckStatement(statement, scope);
        }
    }

    public void CheckAction(Action action, Scope scope)
    {
        scope.Set(action.contextID, ExpressionType.Context);
        scope.Set(action.targetsID, ExpressionType.Targets);
        foreach (IStatement statement in action.statements)
        {
            if (statement is Foreach || statement is While) CheckStatement(statement, new Scope(scope));
            else CheckStatement(statement, scope);
        }
    }

    public void CheckEffectDefinition(EffectDefinition definition)
    {
        Scope scope = new Scope(null);
        if (definition.parameterdefs != null)
        {
            foreach (var pair in definition.parameterdefs.parameters)
            {
                scope.types.Add(pair.Key, pair.Value);
            }
        }
        CheckAction(definition.action, scope);
    }

    public void CheckCardNode(CardNode card, Dictionary<string, EffectDefinition> effects)
    {
        switch (card.type)
        {
            case Card.Type.Boost:
                if (card.power != null) SemanticError(card.keyword, "Invalid card, boost card cannot have power field");
                if (card.position.Count == 0) SemanticError(card.keyword, "Invalid card, boost card position field cannot be empty");
                break;
            case Card.Type.Clear:
                if (card.power != null) SemanticError(card.keyword, "Invalid card, clear card cannot have power field");
                if (card.position.Count != 0) SemanticError(card.keyword, "Invalid card, clear card position field must be empty");
                break;
            case Card.Type.Decoy:
                if (card.power != 0) SemanticError(card.keyword, "Invalid card, decoy must have 0 power");
                if (card.position.Count == 0) SemanticError(card.keyword, "Invalid card, boost card position field cannot be empty");
                break;
            case Card.Type.Golden:
            case Card.Type.Silver:
                if (card.power == null) SemanticError(card.keyword, "Invalid card, unit card must have power field");
                if (card.position.Count == 0) SemanticError(card.keyword, "Invalid card, unit card position field cannot be empty");
                break;
            case Card.Type.Leader:
                if (card.power != null) SemanticError(card.keyword, "Invalid card, leader card cannot have power field");
                if (card.position.Count != 0) SemanticError(card.keyword, "Invalid card, leader card position field must be empty");
                break;
            case Card.Type.Weather:
                if (card.power != null) SemanticError(card.keyword, "Invalid card, weather card cannot have power field");
                if (card.position.Count == 0) SemanticError(card.keyword, "Invalid card, weather card position field cannot be empty");
                break;
        }
        foreach (EffectActivation activation in card.activation.activations)
        {
            CheckEffectActivation(activation, effects, true);
        }

    }

    public void CheckEffectActivation(EffectActivation activation, Dictionary<string, EffectDefinition> effects, bool IsRoot = false)
    {
        EffectDefinition acteffect = GetEffect(effects, activation.effect.definition, activation.effect.keyword);
        if (acteffect != null)
        {
            if (activation.effect.parameters != null)
            {
                if (acteffect.parameterdefs == null)
                    SemanticError(activation.effect.keyword, "Effect definition have no parameters, but effect activation does");
                else
                {
                    bool correct = true;
                    var parameters = activation.effect.parameters.parameters;
                    foreach (var pair in acteffect.parameterdefs.parameters)
                    {
                        if (parameters.ContainsKey(pair.Key) && Tools.GetValueType(parameters[pair.Key]) == pair.Value) continue;
                        correct = false; break;
                    }
                    if (!correct) SemanticError(activation.effect.keyword, "Effects parameters differ");
                }
            }
            else if (acteffect.parameterdefs != null)
                SemanticError(activation.effect.keyword, "Effect activation have no parameters, but effect definition does");
        }
        if (activation.selector != null)
        {
            switch (activation.selector.source.literal)
            {
                case "board":
                case "hand":
                case "otherHand":
                case "deck":
                case "otherDeck":
                case "graveyard":
                case "otherGraveyard":
                case "field":
                case "otherField": break;
                case "parent":
                    //If its the root of the activation hierarchy its source can't be parent
                    if (IsRoot) SemanticError(activation.selector.source, "Invalid source, an effect activation can't have \"parent\" source");
                    break;
                default: SemanticError(activation.selector.source, "Invalid source"); break;
            }
            Scope scope = new Scope(null);
            scope.Set(activation.selector.filtre.parameter, ExpressionType.Card);
            CheckExpression(activation.selector.filtre.predicate, scope);
            if (expressiontypes[activation.selector.filtre.predicate] != ExpressionType.Bool)
                SemanticError(activation.selector.filtre.argumentToken, "Invalid predicate, predicate must be bool");
        }
        else if (activation.postAction != null && activation.postAction.selector != null && (string)activation.postAction.selector.source.literal == "parent")
            SemanticError(activation.postAction.selector.source, "Invalid source, activation parent have no selector");
        if (activation.postAction != null) CheckEffectActivation(activation.postAction, effects);
    }

    public void CheckProgram(ProgramNode program)
    {
        Dictionary<string, EffectDefinition> effects = new Dictionary<string, EffectDefinition>();
        foreach (EffectDefinition definition in program.nodes.Where(n => n is EffectDefinition))
        {
            if (GlobalEffects.effects.ContainsKey(definition.name) || effects.ContainsKey(definition.name))
                SemanticError(definition.keyword, "There is an effect with the same name");
            else effects[definition.name] = definition;
            CheckEffectDefinition(definition);
        }

        foreach (CardNode card in program.nodes.Where(n => n is CardNode))
        {
            CheckCardNode(card, effects);
        }
    }

    public EffectDefinition GetEffect(Dictionary<string, EffectDefinition> effects, string name, Token errorPointer)
    {
        if (effects.ContainsKey(name)) return effects[name];
        if (GlobalEffects.effects.ContainsKey(name)) return GlobalEffects.effects[name];
        SemanticError(errorPointer, "There aren't any existing effects with this name");
        return null;
    }

}




// Clase que representa un ámbito dentro del DSL, rastreando tipos de variables
public class Scope
{
    public Scope(Scope enclosing)
    {
        this.enclosing = enclosing;
        types = new Dictionary<string, ExpressionType>();
    }
    Scope enclosing;
    public Dictionary<string, ExpressionType> types;

    // Obtiene el tipo de una variable del ámbito
    public ExpressionType Get(Token key)
    {
        if (types.ContainsKey(key.lexeme))
        {
            return types[key.lexeme];
        }
        if (enclosing != null) return enclosing.Get(key);
        return ExpressionType.Null;
    }

    // Establece el tipo de una variable en el ámbito
    public void Set(Token key, ExpressionType type)
    {
        key.literal = type;
        if (types.ContainsKey(key.lexeme) && types[key.lexeme] != type) SemanticCheck.SemanticError(key, "Assignation type differs from variable type");
        if (enclosing != null && enclosing.types.ContainsKey(key.lexeme))
        {
            if (enclosing.types[key.lexeme] != type) SemanticCheck.SemanticError(key, "Assignation type differs from variable type");
            else enclosing.types[key.lexeme] = type;
            return;
        }
        types[key.lexeme] = type;
    }
}







