using System;

namespace Scripter.Core.Domain
{
    public enum TokenType
    {
        // Keywords
        Var, Function, Class, Static, Public, Private, Return, If, Else, While, For,
        True, False, Null, New, This, Base,
        
        // Identifiers and literals
        Identifier, Number, String, Boolean,
        
        // Operators
        Plus, Minus, Multiply, Divide, Modulo,
        Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual,
        And, Or, Not,
        Assignment, PlusAssignment, MinusAssignment,
        
        // Delimiters
        LeftParen, RightParen, LeftBrace, RightBrace, LeftBracket, RightBracket,
        Semicolon, Comma, Dot, Colon,
        
        // Special
        EndOfFile, NewLine, Whitespace
    }

    public readonly struct Token
    {
        public readonly TokenType Type;
        public readonly string Value;
        public readonly int Line;
        public readonly int Column;

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"{Type}({Value}) at {Line}:{Column}";
        }
    }
}
