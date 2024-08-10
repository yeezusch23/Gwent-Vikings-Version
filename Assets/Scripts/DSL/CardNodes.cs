using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
public class ProgramNode : IASTNode
{
    public static readonly List<TokenType> moduleTypes = new List<TokenType>() {TokenType.effect, TokenType.Card , TokenType.EOF};
    public List<IASTNode> nodes;
    public ProgramNode(List<IASTNode> nodes)
    {
        this.nodes = nodes;
    }
}
