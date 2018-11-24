using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public enum TokenType
    {
        Comment,
        Keyword,
        Identifier,
        Symbol,
        Error,
        StringLiteral,
        CharLiteral,
        NumberLiteral,
        BooleanLiteral,
        EOF
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
