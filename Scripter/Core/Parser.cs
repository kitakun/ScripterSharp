using System;
using System.Collections.Generic;
using Scripter.Core.Domain;

namespace Scripter.Core
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _current = 0;
        }

        public Program Parse()
        {
            var program = new Program();

            while (!IsAtEnd())
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    program.Statements.Add(statement);
                }
            }

            return program;
        }

        private Statement ParseStatement()
        {
            if (Match(TokenType.Var))
                return ParseVariableDeclaration();
            if (Match(TokenType.Class))
                return ParseClassDeclaration();
            if (Match(TokenType.Function))
                return ParseFunctionDeclaration(false, false);
            if (Match(TokenType.Return))
                return ParseReturnStatement();
            if (Match(TokenType.If))
                return ParseIfStatement();
            if (Match(TokenType.While))
                return ParseWhileStatement();
            if (Match(TokenType.For))
                return ParseForStatement();
            if (Match(TokenType.LeftBrace))
                return ParseBlockStatement();

            // If we're at the end of file or at a right brace, return null
            if (IsAtEnd() || Check(TokenType.RightBrace))
                return null;

            try
            {
                return ParseExpressionStatement();
            }
                catch (Exception ex)
                {
                    // Log error silently for now
                    return null;
                }
        }

        private VariableDeclaration ParseVariableDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected variable name").Value;
            var varDecl = new VariableDeclaration
            {
                Name = name,
                Line = Previous().Line,
                Column = Previous().Column
            };

            if (Match(TokenType.Assignment))
            {
                varDecl.Initializer = ParseExpression();
            }

            Consume(TokenType.Semicolon, "Expected ';' after variable declaration");
            return varDecl;
        }

        private ClassDeclaration ParseClassDeclaration()
        {
            var isStatic = Previous().Type == TokenType.Static;
            if (isStatic)
            {
                Consume(TokenType.Class, "Expected 'class' after 'static'");
            }

            var name = Consume(TokenType.Identifier, "Expected class name").Value;
            var classDecl = new ClassDeclaration
            {
                Name = name,
                IsStatic = isStatic,
                Line = Previous().Line,
                Column = Previous().Column
            };

            Consume(TokenType.LeftBrace, "Expected '{' after class name");

            while (!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                var member = ParseClassMember();
                if (member != null)
                {
                    classDecl.Members.Add(member);
                }
            }

            Consume(TokenType.RightBrace, "Expected '}' after class body");
            return classDecl;
        }

        private Statement ParseClassMember()
        {
            if (Match(TokenType.Function))
                return ParseFunctionDeclaration(false, false);
            if (Match(TokenType.Public, TokenType.Private))
            {
                var accessModifier = Previous();
                if (Match(TokenType.Function))
                    return ParseFunctionDeclaration(accessModifier.Type == TokenType.Public);
                if (Match(TokenType.Identifier))
                {
                    var name = Previous().Value;
                    var property = new PropertyDeclaration
                    {
                        Name = name,
                        IsPublic = accessModifier.Type == TokenType.Public,
                        Line = Previous().Line,
                        Column = Previous().Column
                    };

                    if (Match(TokenType.Assignment))
                    {
                        property.Initializer = ParseExpression();
                    }
                    Consume(TokenType.Semicolon, "Expected ';' after property declaration");
                    return property;
                }
            }
            if (Match(TokenType.Static))
            {
                if (Match(TokenType.Function))
                    return ParseFunctionDeclaration(false, true);
                if (Match(TokenType.Identifier))
                {
                    var name = Previous().Value;
                    var property = new PropertyDeclaration
                    {
                        Name = name,
                        IsStatic = true,
                        Line = Previous().Line,
                        Column = Previous().Column
                    };

                    if (Match(TokenType.Assignment))
                    {
                        property.Initializer = ParseExpression();
                    }
                    Consume(TokenType.Semicolon, "Expected ';' after static property declaration");
                    return property;
                }
            }

            return ParseExpressionStatement();
        }

        private FunctionDeclaration ParseFunctionDeclaration(bool isPublic = false, bool isStatic = false)
        {
            var name = Consume(TokenType.Identifier, "Expected function name").Value;
            var function = new FunctionDeclaration
            {
                Name = name,
                IsPublic = isPublic,
                IsStatic = isStatic,
                Line = Previous().Line,
                Column = Previous().Column
            };

            Consume(TokenType.LeftParen, "Expected '(' after function name");

            if (!Check(TokenType.RightParen))
            {
                do
                {
                    function.Parameters.Add(Consume(TokenType.Identifier, "Expected parameter name").Value);
                } while (Match(TokenType.Comma));
            }

            Consume(TokenType.RightParen, "Expected ')' after parameters");
            function.Body = ParseBlockStatement();
            return function;
        }

        private ReturnStatement ParseReturnStatement()
        {
            var returnStmt = new ReturnStatement
            {
                Line = Previous().Line,
                Column = Previous().Column
            };

            if (!Check(TokenType.Semicolon) && !Check(TokenType.RightBrace))
            {
                returnStmt.Value = ParseExpression();
            }

            if (Check(TokenType.Semicolon))
            {
                Consume(TokenType.Semicolon, "Expected ';' after return statement");
            }
            
            return returnStmt;
        }

        private IfStatement ParseIfStatement()
        {
            var ifStmt = new IfStatement
            {
                Line = Previous().Line,
                Column = Previous().Column
            };

            Consume(TokenType.LeftParen, "Expected '(' after 'if'");
            ifStmt.Condition = ParseExpression();
            Consume(TokenType.RightParen, "Expected ')' after if condition");

            ifStmt.ThenStatement = ParseStatement();

            if (Match(TokenType.Else))
            {
                ifStmt.ElseStatement = ParseStatement();
            }

            return ifStmt;
        }

        private WhileStatement ParseWhileStatement()
        {
            var whileStmt = new WhileStatement
            {
                Line = Previous().Line,
                Column = Previous().Column
            };

            Consume(TokenType.LeftParen, "Expected '(' after 'while'");
            whileStmt.Condition = ParseExpression();
            Consume(TokenType.RightParen, "Expected ')' after while condition");

            whileStmt.Body = ParseStatement();
            return whileStmt;
        }

        private ForStatement ParseForStatement()
        {
            var forStmt = new ForStatement
            {
                Line = Previous().Line,
                Column = Previous().Column
            };

            Consume(TokenType.LeftParen, "Expected '(' after 'for'");

            if (!Check(TokenType.Semicolon))
            {
                forStmt.Initializer = ParseStatement();
            }
            else
            {
                Advance();
            }

            if (!Check(TokenType.Semicolon))
            {
                forStmt.Condition = ParseExpression();
            }
            Consume(TokenType.Semicolon, "Expected ';' after loop condition");

            if (!Check(TokenType.RightParen))
            {
                forStmt.Increment = ParseExpressionStatement();
            }
            Consume(TokenType.RightParen, "Expected ')' after for clauses");

            forStmt.Body = ParseStatement();
            return forStmt;
        }

        private BlockStatement ParseBlockStatement()
        {
            var block = new BlockStatement
            {
                Line = Previous().Line,
                Column = Previous().Column
            };

            while (!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                try
                {
                    var statement = ParseStatement();
                    if (statement != null)
                    {
                        block.Statements.Add(statement);
                    }
                }
                catch (Exception ex)
                {
                    // Log error silently for now
                    // Try to recover by skipping to the next statement
                    Synchronize();
                }
            }

            if (Check(TokenType.RightBrace))
            {
                Advance(); // Consume the right brace
            }
            else if (!IsAtEnd())
            {
                // Warning: Expected '}' after block
            }

            return block;
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var expr = ParseExpression();
            Consume(TokenType.Semicolon, "Expected ';' after expression");
            return new ExpressionStatement { Expression = expr };
        }

        private Expression ParseExpression()
        {
            return ParseAssignment();
        }

        private Expression ParseAssignment()
        {
            var expr = ParseOr();

            if (Match(TokenType.Assignment, TokenType.PlusAssignment, TokenType.MinusAssignment))
            {
                var equals = Previous();
                var value = ParseAssignment();

                if (expr is Identifier identifier)
                {
                    return new AssignmentExpression
                    {
                        Left = identifier,
                        Operator = equals.Type,
                        Right = value,
                        Line = equals.Line,
                        Column = equals.Column
                    };
                }

                if (expr is MemberAccess memberAccess)
                {
                    return new AssignmentExpression
                    {
                        Left = memberAccess,
                        Operator = equals.Type,
                        Right = value,
                        Line = equals.Line,
                        Column = equals.Column
                    };
                }

                throw new Exception($"Invalid assignment target at line {equals.Line}");
            }

            return expr;
        }

        private Expression ParseOr()
        {
            var expr = ParseAnd();

            while (Match(TokenType.Or))
            {
                var operator_ = Previous();
                var right = ParseAnd();
                expr = new BinaryExpression
                {
                    Left = expr,
                    Operator = operator_.Type,
                    Right = right,
                    Line = operator_.Line,
                    Column = operator_.Column
                };
            }

            return expr;
        }

        private Expression ParseAnd()
        {
            var expr = ParseEquality();

            while (Match(TokenType.And))
            {
                var operator_ = Previous();
                var right = ParseEquality();
                expr = new BinaryExpression
                {
                    Left = expr,
                    Operator = operator_.Type,
                    Right = right,
                    Line = operator_.Line,
                    Column = operator_.Column
                };
            }

            return expr;
        }

        private Expression ParseEquality()
        {
            var expr = ParseComparison();

            while (Match(TokenType.NotEqual, TokenType.Equal))
            {
                var operator_ = Previous();
                var right = ParseComparison();
                expr = new BinaryExpression
                {
                    Left = expr,
                    Operator = operator_.Type,
                    Right = right,
                    Line = operator_.Line,
                    Column = operator_.Column
                };
            }

            return expr;
        }

        private Expression ParseComparison()
        {
            var expr = ParseTerm();

            while (Match(TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.LessThan, TokenType.LessThanOrEqual))
            {
                var operator_ = Previous();
                var right = ParseTerm();
                expr = new BinaryExpression
                {
                    Left = expr,
                    Operator = operator_.Type,
                    Right = right,
                    Line = operator_.Line,
                    Column = operator_.Column
                };
            }

            return expr;
        }

        private Expression ParseTerm()
        {
            var expr = ParseFactor();

            while (Match(TokenType.Minus, TokenType.Plus))
            {
                var operator_ = Previous();
                var right = ParseFactor();
                expr = new BinaryExpression
                {
                    Left = expr,
                    Operator = operator_.Type,
                    Right = right,
                    Line = operator_.Line,
                    Column = operator_.Column
                };
            }

            return expr;
        }

        private Expression ParseFactor()
        {
            var expr = ParseUnary();

            while (Match(TokenType.Divide, TokenType.Multiply, TokenType.Modulo))
            {
                var operator_ = Previous();
                var right = ParseUnary();
                expr = new BinaryExpression
                {
                    Left = expr,
                    Operator = operator_.Type,
                    Right = right,
                    Line = operator_.Line,
                    Column = operator_.Column
                };
            }

            return expr;
        }

        private Expression ParseUnary()
        {
            if (Match(TokenType.Not, TokenType.Minus))
            {
                var operator_ = Previous();
                var right = ParseUnary();
                return new UnaryExpression
                {
                    Operator = operator_.Type,
                    Operand = right,
                    Line = operator_.Line,
                    Column = operator_.Column
                };
            }

            return ParseCall();
        }

        private Expression ParseCall()
        {
            var expr = ParsePrimary();

            while (true)
            {
                if (Match(TokenType.LeftParen))
                {
                    expr = FinishCall(expr);
                }
                else if (Match(TokenType.Dot))
                {
                    var name = Consume(TokenType.Identifier, "Expected property name after '.'").Value;
                    expr = new MemberAccess
                    {
                        Object = expr,
                        Member = name,
                        Line = Previous().Line,
                        Column = Previous().Column
                    };
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expression FinishCall(Expression callee)
        {
            var arguments = new List<Expression>();

            if (!Check(TokenType.RightParen))
            {
                do
                {
                    arguments.Add(ParseExpression());
                } while (Match(TokenType.Comma));
            }

            var paren = Consume(TokenType.RightParen, "Expected ')' after arguments");

            return new FunctionCall
            {
                Function = callee,
                Arguments = arguments,
                Line = paren.Line,
                Column = paren.Column
            };
        }

        private Expression ParsePrimary()
        {
            if (Match(TokenType.False)) return new BooleanLiteral(false);
            if (Match(TokenType.True)) return new BooleanLiteral(true);
            if (Match(TokenType.Null)) return new BooleanLiteral(false); // Represent null as false

            if (Match(TokenType.Number))
            {
                var value = double.Parse(Previous().Value);
                return new NumberLiteral(value);
            }

            if (Match(TokenType.String))
            {
                var value = Previous().Value;
                // Remove quotes
                if (value.Length >= 2)
                {
                    value = value.Substring(1, value.Length - 2);
                }
                return new StringLiteral(value);
            }

            if (Match(TokenType.New))
            {
                var className = Consume(TokenType.Identifier, "Expected class name after 'new'").Value;
                return new Identifier(className); // For now, just return identifier, we'll handle instantiation in interpreter
            }

            if (Match(TokenType.This))
            {
                return new Identifier("this");
            }

            if (Match(TokenType.Identifier))
            {
                return new Identifier(Previous().Value);
            }

            if (Match(TokenType.LeftParen))
            {
                var expr = ParseExpression();
                Consume(TokenType.RightParen, "Expected ')' after expression");
                return expr;
            }

            throw new Exception($"Unexpected token at line {Peek().Line}: {Peek().Value}");
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EndOfFile;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw new Exception($"{message} at line {Peek().Line}");
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon) return;

                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Function:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Return:
                        return;
                }

                Advance();
            }
        }
    }
}

