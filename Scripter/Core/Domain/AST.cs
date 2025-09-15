using System;
using System.Collections.Generic;

namespace Scripter.Core.Domain
{
    // Base class for all AST nodes
    public abstract class ASTNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    // Expression nodes
    public abstract class Expression : ASTNode { }

    public class NumberLiteral : Expression
    {
        public readonly double Value;
        public NumberLiteral(double value) { Value = value; }
    }

    public class StringLiteral : Expression
    {
        public readonly string Value;
        public StringLiteral(string value) { Value = value; }
    }

    public class BooleanLiteral : Expression
    {
        public readonly bool Value;
        public BooleanLiteral(bool value) { Value = value; }
    }

    public class Identifier : Expression
    {
        public readonly string Name;
        public Identifier(string name) { Name = name; }
    }

    public class BinaryExpression : Expression
    {
        public Expression Left { get; set; }
        public TokenType Operator { get; set; }
        public Expression Right { get; set; }
    }

    public class UnaryExpression : Expression
    {
        public TokenType Operator { get; set; }
        public Expression Operand { get; set; }
    }

    public class FunctionCall : Expression
    {
        public Expression Function { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();
    }

    public class MemberAccess : Expression
    {
        public Expression Object { get; set; }
        public string Member { get; set; }
    }

    public class AssignmentExpression : Expression
    {
        public Expression Left { get; set; }
        public TokenType Operator { get; set; }
        public Expression Right { get; set; }
    }

    // Statement nodes
    public abstract class Statement : ASTNode { }

    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; set; }
    }

    public class VariableDeclaration : Statement
    {
        public string Name { get; set; }
        public Expression Initializer { get; set; }
    }

    public class ReturnStatement : Statement
    {
        public Expression Value { get; set; }
    }

    public class IfStatement : Statement
    {
        public Expression Condition { get; set; }
        public Statement ThenStatement { get; set; }
        public Statement ElseStatement { get; set; }
    }

    public class WhileStatement : Statement
    {
        public Expression Condition { get; set; }
        public Statement Body { get; set; }
    }

    public class ForStatement : Statement
    {
        public Statement Initializer { get; set; }
        public Expression Condition { get; set; }
        public Statement Increment { get; set; }
        public Statement Body { get; set; }
    }

    public class BlockStatement : Statement
    {
        public List<Statement> Statements { get; set; } = new List<Statement>();
    }

    // Function and class nodes
    public class FunctionDeclaration : Statement
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();
        public BlockStatement Body { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPublic { get; set; }
    }

    public class ClassDeclaration : Statement
    {
        public string Name { get; set; }
        public List<Statement> Members { get; set; } = new List<Statement>();
        public bool IsStatic { get; set; }
    }

    public class PropertyDeclaration : Statement
    {
        public string Name { get; set; }
        public Expression Initializer { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPublic { get; set; }
    }

    // Program node
    public class Program : ASTNode
    {
        public List<Statement> Statements { get; set; } = new List<Statement>();
    }
}
