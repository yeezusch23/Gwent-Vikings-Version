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
                nodes.Add(ParseEffect());
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
    public string StringField(List<TokenType> moduleTypes)
    {
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after name declaration", moduleTypes);
        if (check == null) return null;

        Token name = Stream.Consume(TokenType.StringLiteral, "Expected string in name declaration", moduleTypes);
        if (check == null) return null;

        check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' after name declaration", moduleTypes);
        if (check == null) return null;

        return (string)name.literal;
    }
    ParameterDef ParametersDefinition()
    {
        Dictionary<string, ExpressionType> parameters = new Dictionary<string, ExpressionType>();
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Params construction", EffectDefinition.moduleTypes);
        if (check == null) return null;
        check = Stream.Consume(TokenType.OpenBrace, "Params definition must declare a body", EffectDefinition.moduleTypes);
        if (check == null) return null;
        while (!Stream.Match(TokenType.ClosedBrace))
        {
            Token name = Stream.Consume(TokenType.Identifier, "Invalid parameter name", ParameterDef.moduleTypes);
            if (name == null) continue;
            if (parameters.ContainsKey(name.lexeme))
            {
                Stream.Error(Stream.tokens[Stream.position-1], $"The effect already contains {name.lexeme} parameter", ParameterDef.moduleTypes);
                continue;
            }
            check = Stream.Consume(TokenType.AssignParams, "Expected ':' after parameter name", ParameterDef.moduleTypes);
            if (check == null) continue;
            if (Stream.Match(new List<TokenType>() { TokenType.Number, TokenType.String, TokenType.Bool }))
                parameters[name.lexeme] = Stream.GetStringType(Stream.tokens[Stream.position-1].lexeme);
            else Stream.Error(Stream.Peek(), "Invalid parameter type", ParameterDef.moduleTypes);
        }
        return new ParameterDef(parameters);
    }

       public IStatement Statement()
    {
        Token statementHead=Stream.Peek();
        Token check;

        if (Stream.Check(TokenType.Identifier))
        {
            Token identifier = Stream.Peek();
            IExpression expr = Expression();

            if (Match(TokenType.Equal))
            {
                Token equal = Previous();
                IExpression assignation = Expression();
                check = Consume(TokenType.Semicolon, "Expected ';' after assignation", Block.synchroTypes);
                if (check == null) return null;

                return new Assignation(expr, assignation, equal);
            }
            if (Match(new List<TokenType>() { TokenType.Increment, TokenType.Decrement }))
            {
                Token operation = Previous();
                check = Consume(TokenType.Semicolon, "Expected ';' after assignation", Block.synchroTypes);
                if (check == null) return null;

                return new Increment_Decrement(expr, operation);
            }
            if (Match(new List<TokenType>() { TokenType.MinusEqual, TokenType.PlusEqual, TokenType.StarEqual, TokenType.SlashEqual, TokenType.AtSymbolEqual }))
            {
                Token operation = Previous();
                IExpression assignation = Expression();
                check = Consume(TokenType.Semicolon, "Expected ';' after assignation", Block.synchroTypes);
                if (check == null) return null;
                return new NumericModification(expr, assignation, operation);
            }
            if (Match(TokenType.Dot))
            {
                Token dot = Previous();

                if (Match(new List<TokenType>() { TokenType.Push, TokenType.SendBottom, TokenType.Remove }))
                {
                    Token method = Previous();
                    check = Consume(TokenType.LeftParen, "Expeted '(' after method", Block.synchroTypes);
                    if (check == null) return null;

                    IExpression card = Expression();
                    check = Consume(TokenType.RightParen, "Expected ')' after method", Block.synchroTypes);
                    if (check == null) return null;

                    check = Consume(TokenType.Semicolon, "Expected ';' after method", Block.synchroTypes);
                    if (check == null) return null;

                    if (method.type == TokenType.Push) return new Push(expr, card, dot);
                    if (method.type == TokenType.SendBottom) return new SendBottom(expr, card, dot);
                    return new Remove(expr, card, dot);
                }
                if (Match(TokenType.Shuffle))
                {
                    check = Consume(TokenType.LeftParen, "Expected '(' after method", Block.synchroTypes);
                    if (check == null) return null;

                    check = Consume(TokenType.RightParen, "Expected ')' after method", Block.synchroTypes);
                    if (check == null) return null;

                    check = Consume(TokenType.Semicolon, "Expected ';' after method", Block.synchroTypes);
                    if (check == null) return null;

                    return new Shuffle(expr, dot);
                }


                else Error(Peek(), "Invalid method call", Block.synchroTypes);
                return null;
            }
            if (expr is IStatement){
                check=Consume(TokenType.Semicolon, "Expected ';' after statement", Block.synchroTypes);
                if(check==null) return null;
                return (IStatement)expr;
            } 
            else
            {
                Error(Peek(), "Invalid statement", Block.synchroTypes);
                return null;
            }
        }
        //Parses while loop
        if (Match(TokenType.While))
        {
            check = Consume(TokenType.LeftParen, "Expect '(' after 'while'", Block.synchroTypes);
            IExpression predicate = Expression();
            check = Consume(TokenType.RightParen, "Expect ')' after condition.", Block.synchroTypes);
            List<IStatement> body = null;
            if (Match(TokenType.LeftBrace))
            {
                body = ParseBlock(Block.synchroTypes);
            }
            //One-liner handling
            else body = new List<IStatement>() { Statement() };
            return new While(body, predicate, statementHead);
        }
        //Parses for loop
        if (Match(TokenType.For))
        {
            check = Consume(TokenType.LeftParen, "Expected '('", Block.synchroTypes);
            if (check == null) return null;
            Token variable = Consume(TokenType.Identifier, "Expected identifier in for statement", Block.synchroTypes);
            if (variable == null) return null;
            check = Consume(TokenType.In, "Expected 'in' in for statement", Block.synchroTypes);
            if (check == null) return null;
            IExpression collection = Expression();
            check = Consume(TokenType.RightParen, "Expected ')'", Block.synchroTypes);
            if (check == null) return null;
            List<IStatement> body = null;
            if (Match(TokenType.LeftBrace))
            {
                body = ParseBlock(Block.synchroTypes);
            }
            //One-liner handling
            else body = new List<IStatement>() { Statement() };
            return new Foreach(body, collection, variable, statementHead);
        }
        Error(Peek(), "Invalid statement", Block.synchroTypes);
        return null;
    }

    public List<IStatement> ParseBlock(List<TokenType> moduleTypes)
    {
        List<IStatement> statements = new List<IStatement>();
        while (!Stream.Check(TokenType.ClosedBrace) && !Stream.EOList())
        {
            statements.Add(ParseStatement());
        }
        Token check = Consume(TokenType.RightBrace, "Expected '}' after block", moduleTypes);
        if (check == null) return null;
        return statements;
    }

    //Parses an Action of an Effect Definition
    public Action ParseAction()
    {
        Token action= Stream.tokens[Stream.position-1];
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after 'Action' in Action construction", EffectDefinition.moduleTypes);
        if (check == null) return null;

        check = Stream.Consume(TokenType.OpenParen, "Invalid Action construction, expected '('", EffectDefinition.moduleTypes);
        if (check == null) return null;

        Token targetsID = Stream.Consume(TokenType.Identifier, "Expected targets argument identifier", EffectDefinition.moduleTypes);
        if (check == null) return null;

        check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' between arguments", EffectDefinition.moduleTypes);
        if (check == null) return null;

        Token contextID = Stream.Consume(TokenType.Identifier, "Expected context argument identifier", EffectDefinition.moduleTypes);
        if (check == null) return null;

        check = Stream.Consume(TokenType.ClosedParen, "Invalid Action construction, expected ')'", EffectDefinition.moduleTypes);
        if (check == null) return null;

        check = Stream.Consume(TokenType.Arrow, "Invalid Action construction, expected '=>'", EffectDefinition.moduleTypes);
        if (check == null) return null;

        List<IStatement> body = null;
        if (Stream.Match(TokenType.OpenBrace)) body = ParseBlock(EffectDefinition.moduleTypes);
        else body = new List<IStatement>() { Statement() };
        return new Action(body, contextID, targetsID, action);
    }

    public EffectDefinition ParseEffect()
    {
        Token keyword = Stream.tokens[Stream.position-1];
        Token check = Stream.Consume(TokenType.OpenBrace, "EffectDefinition must declare a body", ProgramNode.moduleTypes);
        if (check == null) return null;
        EffectDefinition definition = new EffectDefinition();
        definition.keyword= keyword;
        while (!Stream.Match(TokenType.ClosedBrace))
        {
            if (Stream.Match(TokenType.Name))
            {
                if (definition.name != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Name was already declared in this effect", EffectDefinition.moduleTypes);
                    continue;
                }
                definition.name = StringField(EffectDefinition.moduleTypes);
                continue;
            }
            if (Stream.Match(TokenType.Params))
            {
                if (definition.parameterdefs != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Params was already declared in this effect", EffectDefinition.moduleTypes);
                    continue;
                }
                definition.parameterdefs = ParametersDefinition();
                continue;
            }
            if (Stream.Match(TokenType.Action))
            {
                if (definition.action != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Action was already declared in this effect", EffectDefinition.moduleTypes);
                    continue;
                }
                definition.action = ParseAction();
                continue;
            }
            Error(Peek(), "Expected effect definition field", EffectDefinition.moduleTypes);
        }
        if (definition.name == null || definition.action == null)
        {
            Error(keyword, "There are missing effect arguments in the ocnstruction", ProgramNode.moduleTypes);
            return null;
        }
        return definition;
    }

}