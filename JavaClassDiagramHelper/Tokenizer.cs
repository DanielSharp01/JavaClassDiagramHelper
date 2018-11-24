using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Tokenizer
    {
        private static readonly string[] keywords = new string[]
        {
            "abstract", "assert", "break", "case", "catch", "class", "const", "continue", "default", "do", "else", "enum", "extends", "final", "finally",
            "for", "goto", "if", "implements", "import", "instanceof", "interface", "native", "new", "package", "private", "protected", "public", "return",
            "static", "strictfp", "super", "switch", "synchronized", "this", "throw", "throws", "transient", "try", "volatile", "while", "true", "false", "null",
            "char", "boolean", "byte", "double", "float", "int", "long", "short", "void" // Primitive types
        };

        private static readonly string[] symbols = new string[]
        {
            "=", "+", "-", "*", "/", "%", "++", "--", "!", "==", "!=", "<=", ">=", "<", ">", "&", "&&", "|", "||", "?", ":", ",", ";", "~", "<<", ">>", ">>>",
            "^", "(", ")", "[", "]", "{", "}", ".", "@"
        };

        private int currentIndex = 0;

        private string str;
        public Tokenizer(string str)
        {
            this.str = str;
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            Token token;
            while ((token = NextToken()).Type != TokenType.EOF)
            {
                tokens.Add(token);
            }

            return tokens;
        }

        public Token NextToken()
        {
            while (currentIndex < str.Length && char.IsWhiteSpace(str[currentIndex])) currentIndex++;

            if (currentIndex >= str.Length) return new Token(TokenType.EOF, null);

            if (char.IsLetter(str[currentIndex]) || str[currentIndex] == '_')
            {
                return NextIdentifier();
            }
            else if (char.IsDigit(str[currentIndex]))
            {
                return NextNumber();
            }
            else if (str[currentIndex] == '.')
            {
                return NextDotStart();
            }
            else if (str[currentIndex] == '/')
            {
                return NextSlashStart();
            }
            else if (str[currentIndex] == '"')
            {
                return NextStringLiteral();
            }
            else if (str[currentIndex] == '\'')
            {
                return NextCharLiteral();
            }
            else if (symbols.Any(s => s.StartsWith(str[currentIndex])))
            {
                return NextSymbol();
            }
            else
            {
                return new Token(TokenType.Error, "" + str[currentIndex]);
            }
        }

        private Token NextIdentifier()
        {
            Token ret = new Token(TokenType.Identifier, "" + str[currentIndex++]);
            while (currentIndex  < str.Length && (char.IsLetterOrDigit(str[currentIndex]) || str[currentIndex] == '_'))
            {
                ret.Value += str[currentIndex++];
            }

            if (keywords.Contains(ret.Value))
            {
                ret.Type = TokenType.Keyword;
            }

            return ret;
        }

        private Token NextNumber(Token starting = null)
        {
            Token ret = starting ?? new Token(TokenType.NumberLiteral, "" + str[currentIndex++]);
            while (currentIndex < str.Length && (char.IsDigit(str[currentIndex]) || str[currentIndex] == '.' || char.ToLower(str[currentIndex]) == 'x' || char.ToLower(str[currentIndex]) == 'e'))
            {
                if (str[currentIndex] == 'x' && (ret.Value[0] != '0' || ret.Value.Length != 1))
                {
                    return ret;
                }
                else if (str[currentIndex] == '.' && ret.Value.Contains('.'))
                {
                    return ret;
                }
                else if (str[currentIndex] == 'e' && (ret.Value.Contains('e') || !char.IsDigit(ret.Value.Last())))
                {
                    return ret;
                }
                else
                {
                    ret.Value += str[currentIndex++];
                }
            }

            return ret;
        }

        private Token NextDotStart()
        {
            Token ret = new Token(TokenType.Symbol, ".");
            currentIndex++;
            if (char.IsDigit(str[currentIndex]))
            {
                ret.Type = TokenType.NumberLiteral;
                return NextNumber(ret);
            }

            return ret;
        }

        private Token NextSlashStart()
        {
            Token ret = new Token(TokenType.Symbol, "/");
            currentIndex++;
            if (str[currentIndex] == '/')
            {
                ret.Type = TokenType.Comment;
                while (currentIndex < str.Length && str[currentIndex] != '\n' && str[currentIndex] != '\r')
                {
                    ret.Value += str[currentIndex++];
                }
            }
            else if (str[currentIndex] == '*')
            {
                ret.Type = TokenType.Comment;
                while (currentIndex < str.Length && (str[currentIndex] != '/' || ret.Value.Last() != '*'))
                {
                    ret.Value += str[currentIndex++];
                }

                if (currentIndex < str.Length)
                    ret.Value += str[currentIndex++];
            }

            return ret;
        }

        private Token NextStringLiteral()
        {
            Token ret = new Token(TokenType.StringLiteral, "\"");
            currentIndex++;
            while (currentIndex < str.Length && (str[currentIndex] != '"' || ret.Value.Last() == '\\'))
            {
                ret.Value += str[currentIndex++];
            }

            if (currentIndex < str.Length)
            {
                ret.Value += str[currentIndex++];
            }
            else
            {
                ret.Type = TokenType.Error;
            }

            return ret;
        }

        private Token NextCharLiteral()
        {
            Token ret = new Token(TokenType.StringLiteral, "\'");
            currentIndex++;
            while (currentIndex < str.Length && (str[currentIndex] != '\'' || ret.Value.Last() == '\\'))
            {
                ret.Value += str[currentIndex++];
            }

            if (currentIndex < str.Length)
            {
                ret.Value += str[currentIndex++];
            }
            else
            {
                ret.Type = TokenType.Error;
            }

            return ret;
        }

        private Token NextSymbol()
        {
            Token ret = new Token(TokenType.Symbol, "" + str[currentIndex++]);
            while (currentIndex < str.Length && symbols.Any(s => s.StartsWith(ret.Value + str[currentIndex])))
            {
                ret.Value += str[currentIndex++];
            }

            return ret;
        }
    }
}
