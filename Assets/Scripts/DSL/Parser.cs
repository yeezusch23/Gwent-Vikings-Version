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
                nodes.Add(ParseCard());
                continue;
            }
            Stream.Error(Stream.Peek(), "Expected Card or Effect", ProgramNode.moduleTypes);
        }
        return program;
    }

    //Parser Card
    public CardNode ParseCard()
    {
        Token keyword=Stream.tokens[Stream.position-1];
        Token check = Stream.Consume(TokenType.OpenBrace, "Card must declare a body", ProgramNode.moduleTypes);
        if (check is null) return null;
        CardNode card = new CardNode();
        card.keyword=keyword;

        while (!Stream.Match(TokenType.ClosedBrace))
        {
            if (Stream.Match(TokenType.Name))
            {
                if (card.name != null)
                {
                    Stream.Error(Stream.Peek(), "Name was already declared in this Card", CardNode.synchroTypes);
                    continue;
                }
                card.name = StringField(CardNode.synchroTypes);
                continue;
            }
            if (Stream.Match(TokenType.Type))
            {
                if (card.type != null)
                {
                    Stream.Error(Stream.Peek(), "Type was already declared in this Card", CardNode.synchroTypes);
                    continue;
                }
                string aux = StringField(CardNode.synchroTypes);
                card.type = Tools.GetCardType(aux);
                if (card.type == null) Stream.Error(keyword, "Invalid Type", CardNode.synchroTypes);
                continue;
            }
            if (Stream.Match(TokenType.Faction))
            {
                if (card.faction != null)
                {
                    Stream.Error(Stream.Peek(), "Faction was already declared in this Card", CardNode.synchroTypes);
                    continue;
                }
                card.faction = StringField(CardNode.synchroTypes);
                continue;
            }
            if (Stream.Match(TokenType.Power))
            {
                if (card.power != null)
                {
                    Stream.Error(Stream.Peek(), "Power was already declared in this card", CardNode.synchroTypes);
                    continue;
                }
                check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Power declaration", CardNode.synchroTypes);
                if (check is null) continue;
                Token number = Stream.Consume(TokenType.NumberLiteral, "Expected Number in power declaration", CardNode.synchroTypes);
                check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' after Power declaration", CardNode.synchroTypes);
                if (check is null) continue;
                card.power = (int)number.literal;
                continue;
            }
            if (Stream.Match(TokenType.Range))
            {
                if (card.position != null)
                {
                    Stream.Error(Stream.Peek(), "Range was already declared in this Card", CardNode.synchroTypes);
                    continue;
                }
                card.position = ParseRange();
                continue;
            }
            if (Stream.Match(TokenType.OnActivation))
            {
                if (card.activation != null)
                {
                    Stream.Error(Stream.Peek(), "Onactivation was already declared in this card", CardNode.synchroTypes);
                    continue;
                }
                card.activation = ParseOnactivation();
                continue;
            }
            Stream.Error(Stream.Peek(), "Invalid Card field", CardNode.synchroTypes);
        }
        if (card.name == null || card.type == null || card.faction == null || card.activation == null|| card.position==null)
        {
            Stream.Error(Stream.Peek(), "There are missing fields in Card construction", ProgramNode.moduleTypes);
            return null;
        }
        return card;
    }

    //Parse EffectActivation
    public EffectActivation ParseEffectActivation()
    {
        Token check = Stream.Consume(TokenType.OpenBrace, "Expected '{'", Onactivation.synchroTypes);
        if (check == null) return null;
        EffectActivation activation = new EffectActivation();
        while (!Stream.Match(TokenType.ClosedBrace))
        {   
            

            if (Stream.Match(TokenType.Effect))
            {
                if (activation.effect != null) Stream.Error(Stream.tokens[Stream.position-1], "Effect was already declared in this EffectActivation", EffectActivation.synchroTypes);
                activation.effect = ParseActivationEffect();
                if(Stream.tokens[Stream.position].type == TokenType.ValueSeparator) Stream.Next();
                continue;
            }
            if (Stream.Match(TokenType.Selector))
            {
                if (activation.selector != null) Stream.Error(Stream.tokens[Stream.position-1], "Selector was already declared in this EffectActivation", EffectActivation.synchroTypes);
                activation.selector = ParseSelector();
                if(Stream.tokens[Stream.position].type == TokenType.ValueSeparator) Stream.Next();
                continue;
            }
            if (Stream.Match(TokenType.PostAction))
            {
                if (activation.postAction != null) Stream.Error(Stream.tokens[Stream.position-1], "PostAction was already declared in this EffectActivation", EffectActivation.synchroTypes);
                check = Stream.Consume(TokenType.AssignParams, "Expected ':' after PostAction declaration", EffectActivation.synchroTypes);
                if (check == null) continue;
                activation.postAction = ParseEffectActivation();
                if(Stream.tokens[Stream.position].type == TokenType.ValueSeparator) Stream.Next();
                continue;
            }
            Stream.Error(Stream.Peek(), "Expected EffectActivation field", EffectActivation.synchroTypes);
        }
        if (activation.effect == null)
        {
            Stream.Error(Stream.tokens[Stream.position-1], "There are missing fields in EffectActivation", Onactivation.synchroTypes);
            return null;
        }
        return activation;
    }

    //Parses un effect
    public Effect ParseActivationEffect()
    {
        Token keyword = Stream.tokens[Stream.position-1];
        Effect effect = new Effect();
        effect.keyword=keyword;
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Effect declaration", EffectActivation.synchroTypes);
        if (check == null) return null;
        if (Stream.Match(TokenType.StringLiteral))
        {
            effect.definition = (string)Stream.tokens[Stream.position-1].literal;
            check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' after Name declaration", EffectActivation.synchroTypes);
            if (check == null) return null;
            return effect;
        }
        check = Stream.Consume(TokenType.OpenBrace, "Effect must declare a body", EffectActivation.synchroTypes);
        if (check == null) return null;
        while (!Stream.Match(TokenType.ClosedBrace))
        {
            if (Stream.Match(TokenType.Name))
            {
                if (effect.definition != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Params was already declared in this effect", Effect.synchroTypes);
                    continue;
                }
                effect.definition = StringField(Effect.synchroTypes);
                continue;
            }
            if (Stream.Check(TokenType.Identifier))
            {
                if (effect.parameters != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Params was already declared in this effect", Effect.synchroTypes);
                    continue;
                }
                effect.parameters = ParseParameter();
                continue;
            }
            Stream.Error(Stream.Peek(), "Expected effect field", Effect.synchroTypes);
        }
        if (effect.definition == null)
        {
            Stream.Error(keyword, "There are missing effect arguments in the construction", EffectActivation.synchroTypes);
            return null;
        }
        return effect;
    }

        //Analiza los parámetros de un efecto
    public Parameters ParseParameter()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        while (!Stream.Check(TokenType.ClosedBrace) && !Stream.Check(TokenType.Name))
        {
            Token name = Stream.Consume(TokenType.Identifier, "Invalid parameter name", Parameters.synchroTypes);
            if (name == null) continue;
            if (parameters.ContainsKey(name.lexeme))
            {
                Stream.Error(Stream.tokens[Stream.position-1], $"The effect already contains {name.lexeme} parameter", Parameters.synchroTypes);
                continue;
            }
            Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after parameter name", Parameters.synchroTypes);
            if (check == null) continue;
            if (Stream.Match(new List<TokenType>() { TokenType.NumberLiteral, TokenType.True, TokenType.False, TokenType.StringLiteral }))
                parameters[name.lexeme] = Stream.tokens[Stream.position-1].literal;
            else
            {
                Stream.Error(Stream.Peek(), "Invalid parameter type", Parameters.synchroTypes);
                continue;
            }
            check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' after parameter", Parameters.synchroTypes);
            if (check == null) continue;
        }
        return new Parameters(parameters);
    }
    
    //Analiza el campo source de un selector
    public Token Source()
    {
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Source declaration", Selector.synchroTypes);
        if (check == null) return null;

        Token source = Stream.Consume(TokenType.StringLiteral, "Expected string in Source declaration", Selector.synchroTypes);
        if (source == null) return null;

        check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' after Source declaration", Selector.synchroTypes);
        if (check == null) return null;

        return source;
    }

    //Analiza el campo single de un selector
    public bool? Single()
    {
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Single declaration", Selector.synchroTypes);
        if (!Stream.Match(new List<TokenType>() { TokenType.True, TokenType.False }))
        {
            Stream.Error(Stream.Peek(), "Expected Bool in Single declaration", Selector.synchroTypes);
            return null;
        }
        Token boolean = Stream.tokens[Stream.position-1];
        check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' after Single declaration", Selector.synchroTypes);
        return (bool)boolean.literal;
    }

    //Analiza el campo Predicate de un selector
    public (IExpression, Token) ParsePredicate()
    {
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Predicate definition", Selector.synchroTypes);
        if (check == null) return (null, null);

        check = Stream.Consume(TokenType.OpenParen, "Expected '('", Selector.synchroTypes);
        if (check == null) return (null, null);
        Token argument = Stream.Consume(TokenType.Identifier, "Expected Predicate argument", Selector.synchroTypes);
        if (argument == null) return (null, null);

        check = Stream.Consume(TokenType.ClosedParen, "Expected ')'", Selector.synchroTypes);
        if (check == null) return (null, null);

        check = Stream.Consume(TokenType.Arrow, "Invalid Predicate construction, expected '=>'", Selector.synchroTypes);
        if (check == null) return (null, null);

        IExpression predicate = ParseExpression();
        return (predicate, argument);
    }
    public Selector ParseSelector()
    {
        Token keyword = Stream.tokens[Stream.position-1];
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Selector declaration", EffectActivation.synchroTypes);
        if (check == null) return null;
        check = Stream.Consume(TokenType.OpenBrace, "Selector must declare a body", EffectActivation.synchroTypes);
        if (check == null) return null;
        Selector selector = new Selector();
        selector.filtre = new ListFind();
        while (!Stream.Match(TokenType.ClosedBrace))
        {
            if (Stream.Match(TokenType.Source))
            {
                if (selector.source != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Source was already declared in this Selector", Selector.synchroTypes);
                    continue;
                }
                selector.source = Source();
                continue;
            }
            if (Stream.Match(TokenType.Single))
            {
                if (selector.single != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Single was already declared in this Selector", Selector.synchroTypes);
                    continue;
                }
                selector.single = Single();
                continue;
            }
            if (Stream.Match(TokenType.Predicate))
            {
                if (selector.filtre.predicate != null)
                {
                    Stream.Error(Stream.tokens[Stream.position-1], "Predicate was already declared in this Selector", Selector.synchroTypes);
                    continue;
                }
                var aux = ParsePredicate();
                selector.filtre.predicate = aux.Item1;
                selector.filtre.parameter = aux.Item2;
                continue;
            }
            Stream.Error(Stream.Peek(), "Expected effect field", Selector.synchroTypes);
        }
        if (selector.single == null) selector.single = false;
        if (selector.source == null || selector.filtre.parameter == null || selector.filtre.predicate == null)
        {
            Stream.Error(keyword, "There are missing fields in Selector construction", EffectActivation.synchroTypes);
            return null;
        }
        return selector;
    }

    

    // Analiza el campo de Onactivation de una card
    public Onactivation ParseOnactivation()
    {
        List<EffectActivation> activations = new List<EffectActivation>();
        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Onactivation declaration", CardNode.synchroTypes);
        if (check == null) return null;
        check = Stream.Consume(TokenType.OpenBracket, "Expected '['", CardNode.synchroTypes);
        if (check == null) return null;
        while (!Stream.Match(TokenType.ClosedBracket))
        {
            activations.Add(ParseEffectActivation());
            if (!Stream.Check(TokenType.ClosedBracket))
            {
                Stream.Consume(TokenType.ValueSeparator, "Expected ',' between EffectActivations", Onactivation.synchroTypes);
            }
        }
        return new Onactivation(activations);
    }

    // Analiza el campo de range de una card
     public List<string> ParseRange()
    {

        Token check = Stream.Consume(TokenType.AssignParams, "Expected ':' after Range definition", CardNode.synchroTypes);
        if (check == null) return null;
        check = Stream.Consume(TokenType.OpenBracket, "Expected Range list", CardNode.synchroTypes);
        if (check == null) return null;

        List<string> positions = new List<string>();
        bool melee = false, ranged = false, siege = false;
        while (!Stream.Match(TokenType.ClosedBracket))
        {
            Token pos = Stream.Consume(TokenType.StringLiteral, "Expected string in Range list", RangeAccess.synchroTypes);
            if (pos == null) continue;
            switch (pos.literal)
            {
                case "Melee":
                    if (melee)
                    {
                        Stream.Error(Stream.tokens[Stream.position-1], "Melee Range is already in this Range List", RangeAccess.synchroTypes);
                        break;
                    }
                    melee = true; positions.Add((string)pos.literal); break;
                case "Ranged":
                    if (ranged)
                    {
                        Stream.Error(Stream.tokens[Stream.position-1], "Ranged Range is already in this Range List", RangeAccess.synchroTypes);
                        break;
                    }
                    ranged = true; positions.Add((string)pos.literal); break;
                case "Siege":
                    if (siege)
                    {
                        Stream.Error(Stream.tokens[Stream.position-1], "Siege Range is already in this Range List", RangeAccess.synchroTypes);
                        break;
                    }
                    siege = true; positions.Add((string)pos.literal); break;
                default: Stream.Error(Stream.tokens[Stream.position-1], "Invalid Range", RangeAccess.synchroTypes); break;
            }
            if (!Stream.Check(TokenType.ClosedBracket))
            {
                check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' between ranges", RangeAccess.synchroTypes);
                if (check == null) continue;
            }
        }
        check = Stream.Consume(TokenType.ValueSeparator, "Expected ',' after Range declaration", CardNode.synchroTypes);
        if (check == null) return null;
        return positions;
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

    public IExpression ParseEquality()
    {
        IExpression expr = ParseStringy();
        List<TokenType> types = new List<TokenType>() { TokenType.ExclamationEqual, TokenType.EqualEqual };
        while (Stream.Match(types))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = ParseStringy();
            if (operation.type == TokenType.ExclamationEqual) expr = new Differ(expr, right, operation);
            else expr = new Equal(expr, right, operation);
        }
        return expr;
    }
    public IExpression ParseStringy()
    {
        IExpression expr = Comparison();
        List<TokenType> types = new List<TokenType>() { TokenType.Concat, TokenType.ConcatConcat };
        while (Stream.Match(types))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = Comparison();
            if (operation.type == TokenType.Concat) expr = new Join(expr, right, operation);
            else expr = new SpaceJoin(expr, right, operation);
        }
        return expr;
    }

    //Analiza expresiones de comparación
    public IExpression Comparison()
    {
        IExpression expr = Term();
        List<TokenType> types = new List<TokenType>(){
            TokenType.Greater,TokenType.GreaterEqual,
            TokenType.Less,TokenType.LessEqual,
        };
        while (Stream.Match(types))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = Term();
            switch (operation.type)
            {
                case TokenType.Greater: expr = new Greater(expr, right, operation); break;
                case TokenType.GreaterEqual: expr = new GreaterEqual(expr, right, operation); break;
                case TokenType.Less: expr = new Less(expr, right, operation); break;
                case TokenType.LessEqual: expr = new LessEqual(expr, right, operation); break;
            }
        }
        return expr;
    }

    //Analiza expresiones de términos (suma y resta)
    public IExpression Term()
    {
        IExpression expr = Factor();
        List<TokenType> types = new List<TokenType>() { TokenType.Sub, TokenType.Add };
        while (Stream.Match(types))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = Factor();
            if (operation.type == TokenType.Add) expr = new Add(expr, right, operation);
            else expr = new Sub(expr, right, operation);
        }
        return expr;
    }

    //Analiza expresiones factoriales (producto y división)
    public IExpression Factor()
    {
        IExpression expr = Power();
        List<TokenType> types = new List<TokenType>() { TokenType.Div, TokenType.Mul };
        while (Stream.Match(types))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = Unary();
            if (operation.type == TokenType.Div) expr = new Division(expr, right, operation);
            else expr = new Product(expr, right, operation);
        }
        return expr;
    }

    //Analiza expresiones de potencia (exponenciación)
    public IExpression Power()
    {
        IExpression expr = Unary();
        while (Stream.Match(TokenType.Pow))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = Unary();
            expr = new Power(expr, right, operation);
        }
        return expr;
    }

    //Analiza expresiones unarias (negación y NO lógico)
    public IExpression Unary()
    {
        List<TokenType> types = new List<TokenType>() { TokenType.Sub, TokenType.Exclamation };
        if (Stream.Match(types))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = Primary();
            if (operation.type == TokenType.Sub) return new Negative(right, Stream.tokens[Stream.position-1]);
            else return new Negation(right, Stream.tokens[Stream.position-1]);
        }
        return Primary();
    }

    //Analiza expresiones primarias (literales, identificadores y expresiones agrupadas)
    public IExpression Primary()
    {
        if (Stream.Match(TokenType.False)) return new Literal(false);
        if (Stream.Match(TokenType.True)) return new Literal(true);
        List<TokenType> types = new List<TokenType>() { TokenType.NumberLiteral, TokenType.StringLiteral };
        if (Stream.Match(types)) return new Literal(Stream.tokens[Stream.position-1].literal);
        if (Stream.Match(TokenType.OpenParen))
        {
            IExpression expr = ParseExpression();
            Token check = Stream.Consume(TokenType.ClosedParen, "Expect ')' after expression", Atom.moduletypes);
            if (check == null) return null;
            return expr;
        }
        if (Stream.Match(TokenType.Identifier))
        {
            IExpression left = Indexer(new Variable(Stream.tokens[Stream.position-1]));

            if (Stream.Check(TokenType.Dot)) left = Access(left);
            types = new List<TokenType>() { TokenType.Increment, TokenType.Decrement };
            if (Stream.Match(types)) left = new Increment_Decrement(left, Stream.tokens[Stream.position-1]);
            return left;
        }

        if (Stream.Check(TokenType.Dot)) Stream.Error(Stream.Peek(), "Invalid property access", Atom.moduletypes);
        else Stream.Error(Stream.Peek(), "Expect expression", Atom.moduletypes);
        return null;
    }

    
    //Analiza expresiones de acceso (acceso a propiedades y métodos pop)
    public IExpression Access(IExpression left)
    {
        Token check;
        //Almacena el token de punto para informar posibles errores semánticos más adelante en Access

        while (Stream.Match(TokenType.Dot))
        {
            Token dot = Stream.tokens[Stream.position-1];
            List<TokenType> types = new List<TokenType>(){
                TokenType.HandOfPlayer, TokenType.DeckOfPlayer,
                TokenType.GraveyardOfPlayer, TokenType.FieldOfPlayer,
                TokenType.Hand, TokenType.Deck,
                TokenType.Graveyard, TokenType.Field,
            };
            if (Stream.Match(types))
            {
                types = new List<TokenType>(){
                    TokenType.HandOfPlayer, TokenType.DeckOfPlayer,
                    TokenType.GraveyardOfPlayer, TokenType.FieldOfPlayer,
                };

                Token aux = Stream.tokens[Stream.position-1];
                if (Stream.Check(types))
                {

                    Token player = Stream.Consume(TokenType.OpenParen, "Expected Player Argument", Atom.moduletypes);
                    if (player == null) return null;

                    IExpression arg = ParseExpression();
                    check = Stream.Consume(TokenType.ClosedParen, "Expected ')' after Player Argument", Atom.moduletypes);
                    if (check == null) return null;

                    switch (aux.type)
                    {
                        case TokenType.HandOfPlayer: left = Indexer(new HandList(left,arg, dot, player)); break;
                        case TokenType.DeckOfPlayer: left = Indexer(new DeckList(left,arg, dot, player)); break;
                        case TokenType.GraveyardOfPlayer: left = Indexer(new GraveyardList(left, arg, dot, player)); break;
                        case TokenType.FieldOfPlayer: left = Indexer(new FieldList(left, arg, dot, player)); break;
                    }
                }
                else
                {
                    switch (aux.type)
                    {
                        case TokenType.Hand: left = Indexer(new HandList(left, new TriggerPlayer(), dot, aux)); break;
                        case TokenType.Deck: left = Indexer(new DeckList(left, new TriggerPlayer(), dot, aux)); break;
                        case TokenType.Field: left = Indexer(new FieldList(left, new TriggerPlayer(), dot, aux)); break;
                        case TokenType.Graveyard: left = Indexer(new GraveyardList(left, new TriggerPlayer(), dot, aux)); break;
                    }
                }
            }
            else if (Stream.Match(TokenType.Board)) left = Indexer(new BoardList(left, dot));

            else if (Stream.Match(TokenType.Find))
            {
                Token argument = Stream.Consume(TokenType.OpenParen, "Expected '(' after method", Atom.moduletypes);
                if (argument == null) return null;

                check = Stream.Consume(TokenType.OpenParen, "Expected '('", Atom.moduletypes);
                if (check == null) return null;

                Token parameter = Stream.Consume(TokenType.Identifier, "Invalid predicate argument", Atom.moduletypes);
                if (parameter == null) return null;

                check = Stream.Consume(TokenType.ClosedParen, "Expeted ')' after predicate argument", Atom.moduletypes);
                if (check == null) return null;

                check = Stream.Consume(TokenType.Arrow, "Expected predicate function call", Atom.moduletypes);
                if (check == null) return null;

                IExpression predicate = ParseExpression();
                check = Stream.Consume(TokenType.ClosedParen, "Expected ')' after predicate", Atom.moduletypes);
                if (check == null) return null;

                left = Indexer(new ListFind(left, predicate, parameter, dot, argument));
            }
            else if (Stream.Match(TokenType.Pop))
            {
                check = Stream.Consume(TokenType.OpenParen, "Expected '(' after method", Atom.moduletypes);
                if (check == null) return null;

                check = Stream.Consume(TokenType.ClosedParen, "Expected ')' after method", Atom.moduletypes);
                if (check == null) return null;

                left = new Pop(left, dot);
            }
            else if (Stream.Match(TokenType.TriggerPlayer)) left = Indexer(new TriggerPlayer());
           //Acceso a la propiedad de la card
            else if (Stream.Match(TokenType.Name)) left = Indexer(new NameAccess(left, dot));
            else if (Stream.Match(TokenType.Power)) left = Indexer(new PowerAccess(left, dot));
            else if (Stream.Match(TokenType.Faction)) left = Indexer(new FactionAccess(left, dot));
            else if (Stream.Match(TokenType.Type)) left = Indexer(new TypeAccess(left, dot));
            else if (Stream.Match(TokenType.Owner)) left = Indexer(new OwnerAccess(left, dot));
            else if (Stream.Match(TokenType.Range)) left = Indexer(new RangeAccess(left, dot));
            if (Stream.Check(TokenType.Dot)) continue;
            types = new List<TokenType>() { TokenType.Push, TokenType.Remove, TokenType.SendBottom, TokenType.Shuffle };
            if (Stream.tokens[Stream.position-1].type == TokenType.Dot)
            {
                if (Stream.Check(types))
                {
                    Stream.position--;
                    return left;
                }
                else
                {
                    Stream.Error(Stream.Peek(), "Invalid property access", Atom.moduletypes);
                    return null;
                }
            }
        }
        return left;
    }


    //Analiza expresiones del indexador (indexación de lista)
    public IExpression Indexer(IExpression list)
    {
        if (Stream.Match(TokenType.OpenBracket))
        {
            Token indexToken = Stream.tokens[Stream.position-1];
            IExpression index = ParseExpression();
            Token check = Stream.Consume(TokenType.ClosedBracket, "Expected ']' after List Indexing", Atom.moduletypes);
            if (check == null) return null;
            if (list is RangeAccess)
            {
                return new IndexedRange(list, index, indexToken);
            }
            else return new IndexedCard(index, list, indexToken);
        }
        else return list;
    }
  
    public IExpression ParseExpression()
    {
        IExpression expr = ParseEquality();
        List<TokenType> types = new List<TokenType>() { TokenType.And, TokenType.Or };
        while (Stream.Match(types))
        {
            Token operation = Stream.tokens[Stream.position-1];
            IExpression right = ParseEquality();
            if (operation.type == TokenType.And) expr = new And(expr, right, operation);
            else expr = new Or(expr, right, operation);
        }
        return expr;
    }

    public IStatement ParseStatement()
    {
        Token statementHead=Stream.Peek();
        Token check;

        if (Stream.Check(TokenType.Identifier))
        {
            Token identifier = Stream.Peek();
            IExpression expr = ParseExpression();

            if (Stream.Match(TokenType.Equal))
            {
                Token equal = Stream.tokens[Stream.position-1];
                IExpression assignation = ParseExpression();
                check = Stream.Consume(TokenType.StatementSeparator, "Expected ';' after assignation", Block.moduleTypes);
                if (check == null) return null;

                return new Assignation(expr, assignation, equal);
            }
            if (Stream.Match(new List<TokenType>() { TokenType.Increment, TokenType.Decrement }))
            {
                Token operation = Stream.tokens[Stream.position-1];
                check = Stream.Consume(TokenType.StatementSeparator, "Expected ';' after assignation", Block.moduleTypes);
                if (check == null) return null;

                return new Increment_Decrement(expr, operation);
            }
            if (Stream.Match(new List<TokenType>() { TokenType.SubEqual, TokenType.AddEqual, TokenType.MulEqual, TokenType.DivEqual, TokenType.ConcatEqual }))
            {
                Token operation = Stream.tokens[Stream.position-1];
                IExpression assignation = ParseExpression();
                check = Stream.Consume(TokenType.StatementSeparator, "Expected ';' after assignation", Block.moduleTypes);
                if (check == null) return null;
                return new NumericModification(expr, assignation, operation);
            }
            if (Stream.Match(TokenType.Dot))
            {
                Token dot = Stream.tokens[Stream.position-1];

                if (Stream.Match(new List<TokenType>() { TokenType.Push, TokenType.SendBottom, TokenType.Remove }))
                {
                    Token method = Stream.tokens[Stream.position-1];
                    check = Stream.Consume(TokenType.OpenParen, "Expeted '(' after method", Block.moduleTypes);
                    if (check == null) return null;

                    IExpression card = ParseExpression();
                    check = Stream.Consume(TokenType.ClosedParen, "Expected ')' after method", Block.moduleTypes);
                    if (check == null) return null;

                    check = Stream.Consume(TokenType.StatementSeparator, "Expected ';' after method", Block.moduleTypes);
                    if (check == null) return null;

                    if (method.type == TokenType.Push) return new Push(expr, card, dot);
                    if (method.type == TokenType.SendBottom) return new SendBottom(expr, card, dot);
                    return new Remove(expr, card, dot);
                }
                if (Stream.Match(TokenType.Shuffle))
                {
                    check = Stream.Consume(TokenType.OpenParen, "Expected '(' after method", Block.moduleTypes);
                    if (check == null) return null;

                    check = Stream.Consume(TokenType.ClosedParen, "Expected ')' after method", Block.moduleTypes);
                    if (check == null) return null;

                    check = Stream.Consume(TokenType.StatementSeparator, "Expected ';' after method", Block.moduleTypes);
                    if (check == null) return null;

                    return new Shuffle(expr, dot);
                }

                else Stream.Error(Stream.Peek(), "Invalid method call", Block.moduleTypes);
                return null;
            }
            if (expr is IStatement){
                check=Stream.Consume(TokenType.StatementSeparator, "Expected ';' after statement", Block.moduleTypes);
                if(check==null) return null;
                return (IStatement)expr;
            } 
            else
            {
                Stream.Error(Stream.Peek(), "Invalid statement", Block.moduleTypes);
                return null;
            }
        }
        //Analiza el bucle while
        if (Stream.Match(TokenType.While))
        {
            check = Stream.Consume(TokenType.OpenParen, "Expect '(' after 'while'", Block.moduleTypes);
            IExpression predicate = ParseExpression();
            check = Stream.Consume(TokenType.ClosedParen, "Expect ')' after condition.", Block.moduleTypes);
            List<IStatement> body = null;
            if (Stream.Match(TokenType.OpenBrace))
            {
                body = ParseBlock(Block.moduleTypes);
            }

            else body = new List<IStatement>() { ParseStatement() };
            return new While(body, predicate, statementHead);
        }
        //Analiza el bucle for
        if (Stream.Match(TokenType.For))
        {
            check = Stream.Consume(TokenType.OpenParen, "Expected '('", Block.moduleTypes);
            if (check == null) return null;
            Token variable = Stream.Consume(TokenType.Identifier, "Expected identifier in for statement", Block.moduleTypes);
            if (variable == null) return null;
            check = Stream.Consume(TokenType.In, "Expected 'in' in for statement", Block.moduleTypes);
            if (check == null) return null;
            IExpression collection = ParseExpression();
            check = Stream.Consume(TokenType.ClosedParen, "Expected ')'", Block.moduleTypes);
            if (check == null) return null;
            List<IStatement> body = null;
            if (Stream.Match(TokenType.OpenBrace))
            {
                body = ParseBlock(Block.moduleTypes);
            }
            else body = new List<IStatement>() { ParseStatement() };
            return new Foreach(body, collection, variable, statementHead);
        }
        Stream.Error(Stream.Peek(), "Invalid statement", Block.moduleTypes);
        return null;
    }

    public List<IStatement> ParseBlock(List<TokenType> moduleTypes)
    {
        List<IStatement> statements = new List<IStatement>();
        while (!Stream.Check(TokenType.ClosedBrace) && !Stream.EOList())
        {
            statements.Add(ParseStatement());
        }
        Token check = Stream.Consume(TokenType.ClosedBrace, "Expected '}' after block", moduleTypes);
        if (check == null) return null;
        return statements;
    }

    //Analiza una acción de una definición de efecto
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
        else body = new List<IStatement>() { ParseStatement() };
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
                if(Stream.tokens[Stream.position].type == TokenType.ValueSeparator) Stream.Next();
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
            Stream.Error(Stream.Peek(), "Expected effect definition field", EffectDefinition.moduleTypes);
        }
        if (definition.name == null || definition.action == null)
        {
            Stream.Error(keyword, "There are missing effect arguments in the ocnstruction", ProgramNode.moduleTypes);
            return null;
        }
        return definition;
    }

}