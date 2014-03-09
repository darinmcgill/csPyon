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
        EQ_SIGN = 61, COLON = 58
    };

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
                return value_.ToString();
            if (type_ == TokenType.QUOTED)
                return "Quoted('" + value_ + "')";
            return "Syntax('" + (char)(int)type_ + "')";
        }

        private static Token readBareword(List<char> broken)
        {
            Token output = new Token();
            output.type_ = TokenType.BAREWORD;
            StringBuilder builder = new StringBuilder();
            while (broken.Count > 0 && System.Char.IsLetterOrDigit(broken[0]))
            {
                builder.Append(broken[0]);
                broken.RemoveAt(0);
            }
            output.value_ = builder.ToString();
            return output;
        }

        public static List<Token> tokenize(string input)
        {
            List<Token> output = new List<Token>();
            List<char> broken = new List<char>(input.Length);
            for (int i = 0; i < input.Length; i++)
                broken.Add(input[i]);
            while (broken.Count > 0)
            {
                char first = broken[0];
                if (System.Char.IsWhiteSpace(first))
                {
                    broken.RemoveAt(0);
                    continue;
                }
                if (System.Char.IsLetter(first))
                {
                    output.Add(readBareword(broken));
                    continue;
                }
            }
            return output;
        }
    }
}
