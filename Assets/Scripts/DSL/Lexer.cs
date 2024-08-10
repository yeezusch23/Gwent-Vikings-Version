using System.Collections.Generic;
using UnityEngine;

public class Lexer
{
    string code;

    private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType> {
        
        //effect
        {"effect", TokenType.effect},
        {"Name", TokenType.Name},
        {"Params", TokenType.Params},
        {"Number", TokenType.Number},
        {"String", TokenType.String},
        {"Bool", TokenType.Bool},
        {"Action", TokenType.Action},
        //{"targets", TokenType.targets},
        //{"context", TokenType.context},
        {"TriggerPlayer", TokenType.TriggerPlayer},
        {"Board", TokenType.Board},
        {"HandOfPlayer", TokenType.HandOfPlayer},
        {"FieldOfPlayer", TokenType.FieldOfPlayer},
        {"GraveyardOfPlayer", TokenType.GraveyardOfPlayer},
        {"DeckOfPlayer", TokenType.DeckOfPlayer},
        {"Hand", TokenType.Hand},
        {"Deck", TokenType.Deck},
        {"Field", TokenType.Field},
        {"Graveyard", TokenType.Graveyard},
        {"Owner", TokenType.Owner},
        {"Find", TokenType.Find},
        {"Push", TokenType.Push},
        {"SendBottom", TokenType.SendBottom},
        {"Pop", TokenType.Pop},
        {"Remove", TokenType.Remove},
        {"Shuffle", TokenType.Shuffle},

        //card
        {"card", TokenType.Card},
        {"Type", TokenType.Type},
        {"Faction", TokenType.Faction},
        {"Power", TokenType.Power},
        {"Range", TokenType.Range},
        {"OnActivation", TokenType.OnActivation},
        {"Effect", TokenType.Effect},
        {"Selector", TokenType.Selector},
        {"Source", TokenType.Source},
        {"Single", TokenType.Single},
        {"Predicate", TokenType.Predicate},
        {"PostAction", TokenType.PostAction},
        
        //
        {"for", TokenType.For},
        {"while", TokenType.While},
        {"in", TokenType.In},
        {"false", TokenType.False},
        {"true", TokenType.True},
    };
    List<Token> tokens = new List<Token>();
    int start = 0;
    int current = 0;
    int line = 1;
    int column = 1;

    //Constructor de la clase
    public Lexer(string code)
    {
        this.code = code;
    }

    //Obteniendo lista de tokens
    public List <Token> GetTokens()
    {
        while (!EOF())
        {
            start = current;
            GetToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null!, line, column));
        return tokens;
    }

    //Obtener un token
    void GetToken()
    {
        char currentCharacter = Next();
        switch (currentCharacter)
        {
            case '(': AddToken(TokenType.OpenParen, null); break;
            case ')': AddToken(TokenType.ClosedParen, null); break;
            case '[': AddToken(TokenType.OpenBracket, null); break;
            case ']': AddToken(TokenType.ClosedBracket, null); break;
            case '{': AddToken(TokenType.OpenBrace, null); break;
            case '}': AddToken(TokenType.ClosedBrace, null); break;
            case ',': AddToken(TokenType.ValueSeparator, null); break;
            case '.': AddToken(TokenType.Dot, null); break;
            case ';': AddToken(TokenType.StatementSeparator, null); break;
            case ':': AddToken(TokenType.AssignParams, null); break;
            case '^': AddToken(TokenType.Pow, null); break;
            case '%': AddToken(TokenType.Mod, null); break;
            case '!':
                AddToken(Match('=') ? TokenType.ExclamationEqual : TokenType.Exclamation, null);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : Match('>') ? TokenType.Arrow : TokenType.Equal, null);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less, null);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater, null);
                break;
            case '+':
                AddToken(Match('=') ? TokenType.AddEqual : Match('+') ? TokenType.Increment : TokenType.Add, null);
                break;
            case '-':
                AddToken(Match('=') ? TokenType.SubEqual : Match('-') ? TokenType.Decrement : TokenType.Sub, null);
                break;
            case '*':
                AddToken(Match('=') ? TokenType.MulEqual :  TokenType.Mul, null);
                break;
            case '@':
                AddToken(Match('=') ? TokenType.ConcatEqual : Match('@') ? TokenType.ConcatConcat : TokenType.Concat, null);
                break;
            case '&':
                if (Match('&')) AddToken(TokenType.And, null);
                else DSL.Report(line, column, "", "Unexpected character: " + currentCharacter);
                break;
            case '|':
                if (Match('|')) AddToken(TokenType.Or, null);
                else DSL.Report(line, column, "", "Unexpected character: " + currentCharacter);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !EOF()) Next();
                }
                else
                {
                    AddToken(Match('=') ? TokenType.DivEqual : TokenType.Div, null);
                }
                break;
            case '"': GetString(); break;
            case ' ':
            case '\t':
            case '\r': break;
            case '\n':
                line++;
                column = 1;
                break;
            default:
                if (IsDigit(currentCharacter)) GetNumber();
                else if (IsAlpha(currentCharacter)) GetIdentifier();
                else
                {
                    DSL.Report(line, column, "", "Unexpected character: " + currentCharacter);
                }
                break;
        }
    }

    
    // Obtener un numero
    void GetNumber()
    {
        while (IsDigit(Peek())) Next();
        object value = int.Parse(code.Substring(start, current - start));
        AddToken(TokenType.NumberLiteral, value);
    }

    // Obtener un identificador
    void GetIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Next();
        string lexeme = code.Substring(start, current - start);
        if (keywords.ContainsKey(lexeme)){
            if(lexeme == "false") AddToken(TokenType.False, false);
            else if(lexeme=="true") AddToken(TokenType.True, true);
            else AddToken(keywords[lexeme], null);
        }
        else AddToken(TokenType.Identifier, lexeme);
    }
    bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    //Obtener String
    private void GetString()
    {   
        //Avanzar hasta el final del string
        while (Peek() != '"' && Peek() != '\n' && !EOF()) Next();
        
        if (EOF() || Peek() == '\n')
        {
            DSL.Report(line, column, "", "String no terminado");
            if (Peek() == '\n')
            {
                line++;
                column = 1;
                Next();
            }
            return;
        }
        Next();
        object value = code.Substring(start + 1, current - start - 2);
        AddToken(TokenType.StringLiteral, value);
    }

    //Deveulve el caracter actual
    char Peek()
    {
        if (EOF()) return '\0';
        return code[current];
    }

    //Comprobar si coincide con el caracter esperado
    bool Match(char c)
    {
        if (EOF()) return false;
        if (code[current] != c) return false;
        current++;
        return true;
    }

    //Agregar un token a la lista
    void AddToken(TokenType type, object value)
    {
        string lexeme = code.Substring(start, current - start);
        tokens.Add(new Token(type, lexeme, value, line, column - lexeme.Length));
    }

    //Avanzar el puntero y Obtener el caracter en la posicion anteriror
    char Next()
    {
        column++;
        current++;
        return code[current-1];
    }
    public bool EOF()
    {
        return current >= code.Length;
    }

}


public enum TokenType
{
    OpenParen, // ( 
    ClosedParen, // )
    OpenBracket, // [
    ClosedBracket, // ]
    OpenBrace, // {
    ClosedBrace, // }
    ValueSeparator, // , 
    Dot, // .
    AssignParams, // :
    StatementSeparator, // ;
    Mod, // %
    Pow, // ^    
    Exclamation, // !
    ExclamationEqual, // !=
    Add, // +
    AddEqual, // +=
    Increment, // ++
    Sub, // -
    SubEqual, // -=
    Decrement, // --
    Mul, // *
    MulEqual, // *=
    Div, // / 
    DivEqual, // /=
    Equal, // =
    EqualEqual, // ==
    Arrow, // =>
    Greater, // >
    GreaterEqual, // >=
    Less, // <
    LessEqual, // <=
    Concat, // @ 
    ConcatConcat, // @@ 
    ConcatEqual, // @=
    Or, // ||
    And, // &&

    // Literales
    Identifier, 
    StringLiteral, 
    NumberLiteral, 
    True, 
    False,

    // Keywords
    //effect
    effect,
    Name,         
    Params,         
    Number,         
    String,         
    Bool,         
    Action,            
    TriggerPlayer,         
    Board,         
    HandOfPlayer,         
    FieldOfPlayer,         
    GraveyardOfPlayer,         
    DeckOfPlayer,         
    Hand,         
    Deck,         
    Field,         
    Graveyard,         
    Owner,         
    Find,         
    Push,         
    SendBottom,         
    Pop,         
    Remove,         
    Shuffle,        

    //card
    Card,         
    Type,         
    Faction,       
    Power,         
    Range,         
    OnActivation,   
    Effect,         
    Selector,       
    Source,         
    Single,         
    Predicate,         
    PostAction,         
    
    //-------------
    For,
    While,
    In,
    EOF
}

public class Token
{
    public TokenType type;
    public string lexeme;
    public object literal;
    public int line;
    public int column;

    // Constructor
    public Token(TokenType type, string lexeme, object literal, int line, int column)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
        this.column = column;
    }
    
    public override string ToString()
    {
        string res = $"{type.ToString()} {lexeme}";
        if (literal != null) res += " " + literal.ToString();
        res += $" [ln {line}, col {column}]";
        return res;
    }
}