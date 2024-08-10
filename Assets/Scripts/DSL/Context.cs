using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class Context : IASTNode
{
    public Context() { }

    public Context(Player triggerplayer, Context scope, Dictionary<string, object> variables)
    {
        this.triggerplayer = triggerplayer;
        this.scope = scope;
        this.variables = variables;
    }

    public Player triggerplayer;
    public Context scope;
    public Dictionary<string, object> variables;

    // obtener variables del context
    public object Get(Token key)
    {
        if (variables.ContainsKey(key.lexeme))
        {
            return variables[key.lexeme];
        }
        if (scope != null) return scope.Get(key);
        //TODO: Hacer que imprima el error en la consola
        throw new Exception("variable was not found context");
    }

    // agragar variables al context
    public void Set(Token key, object value)
    {
        if (variables.ContainsKey(key.lexeme))
        {
            variables[key.lexeme] = value;
            return;
        }
        if (scope.variables.ContainsKey(key.lexeme))
        {
            scope.variables[key.lexeme] = value;
            return;
        }
        variables[key.lexeme] = value;
    }
}