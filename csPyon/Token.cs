using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csPyon
{
    public enum TokenType
    {
        END = 0, QUOTED = 1, BAREWORD = 2, NUMBER = 3, COMMA = 44,
        OPEN_PAREN = 40, CLOSE_PAREN = 41,
        OPEN_BRACKET = 91, CLOSE_BRACKET = 93,
        OPEN_CURLY = 123, CLOSE_CURLY = 125,
        EQ_SIGN = 61, COLON = 58, SEMI = 59
    };

    public class LexError : Exception
    {
        public string msg;
        public LexError(string msg_) 
        {
            msg = msg_;
        }
    }

    public class Token
    {
        public TokenType type_;
        public object value_;
        public string ToString()
        {
            if (type_ == TokenType.END) return "End()";
            if (type_ == TokenType.BAREWORD) 
                return "Bareword('" + value_ + "')";
            if (type_ == TokenType.NUMBER)
                return "Number(" + value_.ToString() + ")";
            if (type_ == TokenType.QUOTED)
                return "Quoted('" + value_ + "')";
            return "Syntax('" + (char)(int)type_ + "')";
        }

        private static Token readBareword(List<char> chars)
        {
            // Assumes that the first character of chars is the
            // beginning of a bareword.
            Token output = new Token();
            output.type_ = TokenType.BAREWORD;
            StringBuilder builder = new StringBuilder();
            while (chars.Count > 0 && 
                (System.Char.IsLetterOrDigit(chars[0]) ||
                chars[0] == '_' ||
                chars[0] == '-' ))
            {
                builder.Append(chars[0]);
                chars.RemoveAt(0);
            }
            output.value_ = builder.ToString();
            return output;
        }

        private static Token readQuoted(List<char> chars)
        {
            // Assumes that the first character of chars is the
            // beginning of a quoted string (" or ').
            Token output = new Token();
            output.type_ = TokenType.QUOTED;
            StringBuilder builder = new StringBuilder();
            char quoteChar = chars[0];
            chars.RemoveAt(0);
            //while (chars.Count > 0 && System.Char.IsLetterOrDigit(chars[0]))
            while (true)
            {
                if (chars.Count == 0) 
                    throw new LexError("fell off end inside string");
                char first = chars[0];
                chars.RemoveAt(0);
                if (first == quoteChar) break;
                builder.Append(first);
            }
            output.value_ = builder.ToString();
            return output;
        }



        private static Token readNumber(List<char> chars)
        {
            // Assumes that the first character of chars is a digit or +/-.
            Token output = new Token();
            output.type_ = TokenType.NUMBER;
            if (chars.Count >= 3 && chars[0] == '0' && chars[1] == 'x')
            {
                // read hex numbers like 0xF3A2
                chars.RemoveAt(0);
                chars.RemoveAt(0);
                StringBuilder builder = new StringBuilder();
                while (chars.Count > 0 && System.Char.IsLetterOrDigit(chars[0]))
                {
                    builder.Append(chars[0]);
                    chars.RemoveAt(0);
                }
                output.value_ = Convert.ToInt64(builder.ToString(),16);
                return output;
            }
            int sign = +1;
            if (chars[0] == '+') chars.RemoveAt(0);
            if (chars[0] == '-')
            {
                sign = -1;
                chars.RemoveAt(0);
            }
            bool floating = false;
            UInt64 beforeDecimal = 0;
            double afterDecimal = 0.0;
            int eVal = 0;
            int eSign = +1;
            double multiplier = 0.1;
            while (chars.Count > 0 && System.Char.IsDigit(chars[0]))
            {
                beforeDecimal *= 10;
                beforeDecimal += (ulong) (((int)chars[0]) - 48);
                chars.RemoveAt(0);
            }
            if (chars.Count > 0 && chars[0] == '.')
            {
                floating = true;
                chars.RemoveAt(0);
                while (chars.Count > 0 && System.Char.IsDigit(chars[0]))
                {
                    afterDecimal += multiplier * (((int)chars[0]) - 48);
                    multiplier *= 0.1;
                    chars.RemoveAt(0);
                }
            }
            if (chars.Count > 0 && (chars[0] == 'e' || chars[0] == 'E'))
            {
                floating = true;
                chars.RemoveAt(0);
                if (chars.Count > 0 && chars[0] == '+') chars.RemoveAt(0);
                if (chars.Count > 0 && chars[0] == '-')
                {
                    eSign = -1;
                    chars.RemoveAt(0);
                }
                while (chars.Count > 0 && System.Char.IsDigit(chars[0]))
                {
                    eVal *= 10;
                    eVal += ((int)chars[0]) - 48;
                    chars.RemoveAt(0);
                }
            }
            if (!floating)
            {
                if (sign == -1)
                    output.value_ = -1 * (long)beforeDecimal;
                else
                    output.value_ = beforeDecimal;
                return output;
            }
            double val = sign * (beforeDecimal + afterDecimal);
            while (eVal > 0)
            {
                if (eSign == +1) val *= 10;
                else val *= 0.1;
                eVal -= 1;
            }
            output.value_ = val;
            return output;
        }


        public static List<Token> tokenize(string input)
        {
            List<Token> output = new List<Token>();
            List<char> chars = new List<char>(input.Length);
            for (int i = 0; i < input.Length; i++)
                chars.Add(input[i]);
            while (chars.Count > 0)
            {
                char first = chars[0];
                if (System.Char.IsWhiteSpace(first))
                {
                    chars.RemoveAt(0);
                    continue;
                }
                if (System.Char.IsLetter(first) || first == '_')
                {
                    output.Add(readBareword(chars));
                    continue;
                }
                if (first == '\'' || first == '"')
                {
                    output.Add(readQuoted(chars));
                    continue;
                }
                if (System.Char.IsDigit(first) ||
                    first == '-' || first == '+' || first == '.')
                {
                    output.Add(readNumber(chars));
                    continue;
                }
                if (first == '('
                    || first == ')'
                    || first == '['
                    || first == ']'
                    || first == '{'
                    || first == '}'
                    || first == ':'
                    || first == ';'
                    || first == ','
                    || first == '=')
                {
                    chars.RemoveAt(0);
                    Token syntax = new Token();
                    syntax.type_ = (TokenType)(int)first;
                    output.Add(syntax);
                    continue;
                }
                throw new LexError("don't know what to do with " + first);
            }
            return output;
        }
    }
}
