using System;
using System.Collections.Generic;
using System.Text;
using Scripter.Core.Domain;

namespace Scripter.Core
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;
        private int _line;
        private int _column;
        private readonly Dictionary<string, TokenType> _keywords;

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
            _line = 1;
            _column = 1;
            _keywords = InitializeKeywords();
        }

        private Dictionary<string, TokenType> InitializeKeywords()
        {
            return new Dictionary<string, TokenType>
            {
                {"var", TokenType.Var},
                {"function", TokenType.Function},
                {"class", TokenType.Class},
                {"static", TokenType.Static},
                {"public", TokenType.Public},
                {"private", TokenType.Private},
                {"return", TokenType.Return},
                {"if", TokenType.If},
                {"else", TokenType.Else},
                {"while", TokenType.While},
                {"for", TokenType.For},
                {"true", TokenType.True},
                {"false", TokenType.False},
                {"null", TokenType.Null},
                {"new", TokenType.New},
                {"this", TokenType.This},
                {"base", TokenType.Base}
            };
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (_position < _input.Length)
            {
                var token = NextToken();
                if (token.Type != TokenType.Whitespace)
                {
                    tokens.Add(token);
                }
            }

            tokens.Add(new Token(TokenType.EndOfFile, "", _line, _column));
            return tokens;
        }

        private Token NextToken()
        {
            if (_position >= _input.Length)
                return new Token(TokenType.EndOfFile, "", _line, _column);

            char current = _input[_position];

            // Skip whitespace
            if (char.IsWhiteSpace(current))
            {
                return ReadWhitespace();
            }

            // Comments
            if (current == '/' && _position + 1 < _input.Length && _input[_position + 1] == '/')
            {
                return ReadComment();
            }

            // Numbers
            if (char.IsDigit(current))
            {
                return ReadNumber();
            }

            // Strings
            if (current == '"')
            {
                return ReadString();
            }

            // Identifiers and keywords
            if (char.IsLetter(current) || current == '_')
            {
                return ReadIdentifier();
            }

            // Operators and delimiters
            return ReadOperatorOrDelimiter();
        }

        private Token ReadWhitespace()
        {
            var start = _position;
            while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
            {
                if (_input[_position] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
                _position++;
            }
            return new Token(TokenType.Whitespace, _input.Substring(start, _position - start), _line, _column);
        }

        private Token ReadComment()
        {
            var start = _position;
            while (_position < _input.Length && _input[_position] != '\n')
            {
                _position++;
                _column++;
            }
            return new Token(TokenType.Whitespace, _input.Substring(start, _position - start), _line, _column);
        }

        private Token ReadNumber()
        {
            var start = _position;
            while (_position < _input.Length && (char.IsDigit(_input[_position]) || _input[_position] == '.'))
            {
                _position++;
                _column++;
            }
            return new Token(TokenType.Number, _input.Substring(start, _position - start), _line, _column);
        }

        private Token ReadString()
        {
            var start = _position;
            _position++; // Skip opening quote
            _column++;

            while (_position < _input.Length && _input[_position] != '"')
            {
                if (_input[_position] == '\\' && _position + 1 < _input.Length)
                {
                    _position += 2; // Skip escape sequence
                    _column += 2;
                }
                else
                {
                    _position++;
                    _column++;
                }
            }

            if (_position < _input.Length)
            {
                _position++; // Skip closing quote
                _column++;
            }

            return new Token(TokenType.String, _input.Substring(start, _position - start), _line, _column);
        }

        private Token ReadIdentifier()
        {
            var start = _position;
            while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
            {
                _position++;
                _column++;
            }

            var value = _input.Substring(start, _position - start);
            var type = _keywords.ContainsKey(value) ? _keywords[value] : TokenType.Identifier;

            return new Token(type, value, _line, _column);
        }

        private Token ReadOperatorOrDelimiter()
        {
            var current = _input[_position];
            var next = _position + 1 < _input.Length ? _input[_position + 1] : '\0';

            switch (current)
            {
                case '+':
                    _position++;
                    _column++;
                    if (next == '=')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.PlusAssignment, "+=", _line, _column);
                    }
                    return new Token(TokenType.Plus, "+", _line, _column);

                case '-':
                    _position++;
                    _column++;
                    if (next == '=')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.MinusAssignment, "-=", _line, _column);
                    }
                    return new Token(TokenType.Minus, "-", _line, _column);

                case '*':
                    _position++;
                    _column++;
                    return new Token(TokenType.Multiply, "*", _line, _column);

                case '/':
                    _position++;
                    _column++;
                    return new Token(TokenType.Divide, "/", _line, _column);

                case '%':
                    _position++;
                    _column++;
                    return new Token(TokenType.Modulo, "%", _line, _column);

                case '=':
                    _position++;
                    _column++;
                    if (next == '=')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.Equal, "==", _line, _column);
                    }
                    return new Token(TokenType.Assignment, "=", _line, _column);

                case '!':
                    _position++;
                    _column++;
                    if (next == '=')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.NotEqual, "!=", _line, _column);
                    }
                    return new Token(TokenType.Not, "!", _line, _column);

                case '<':
                    _position++;
                    _column++;
                    if (next == '=')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.LessThanOrEqual, "<=", _line, _column);
                    }
                    return new Token(TokenType.LessThan, "<", _line, _column);

                case '>':
                    _position++;
                    _column++;
                    if (next == '=')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.GreaterThanOrEqual, ">=", _line, _column);
                    }
                    return new Token(TokenType.GreaterThan, ">", _line, _column);

                case '&':
                    _position++;
                    _column++;
                    if (next == '&')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.And, "&&", _line, _column);
                    }
                    break;

                case '|':
                    _position++;
                    _column++;
                    if (next == '|')
                    {
                        _position++;
                        _column++;
                        return new Token(TokenType.Or, "||", _line, _column);
                    }
                    break;

                case '(':
                    _position++;
                    _column++;
                    return new Token(TokenType.LeftParen, "(", _line, _column);

                case ')':
                    _position++;
                    _column++;
                    return new Token(TokenType.RightParen, ")", _line, _column);

                case '{':
                    _position++;
                    _column++;
                    return new Token(TokenType.LeftBrace, "{", _line, _column);

                case '}':
                    _position++;
                    _column++;
                    return new Token(TokenType.RightBrace, "}", _line, _column);

                case '[':
                    _position++;
                    _column++;
                    return new Token(TokenType.LeftBracket, "[", _line, _column);

                case ']':
                    _position++;
                    _column++;
                    return new Token(TokenType.RightBracket, "]", _line, _column);

                case ';':
                    _position++;
                    _column++;
                    return new Token(TokenType.Semicolon, ";", _line, _column);

                case ',':
                    _position++;
                    _column++;
                    return new Token(TokenType.Comma, ",", _line, _column);

                case '.':
                    _position++;
                    _column++;
                    return new Token(TokenType.Dot, ".", _line, _column);

                case ':':
                    _position++;
                    _column++;
                    return new Token(TokenType.Colon, ":", _line, _column);
            }

            // Unknown character
            _position++;
            _column++;
            return new Token(TokenType.Identifier, current.ToString(), _line, _column);
        }
    }
}
