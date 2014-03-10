using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace csPyon
{
    public class ParseError : Exception { }
    public class NotImplemented : Exception { }
    public class SanityError : Exception { }
    public class Parser
    {
        public int sanity = 10000;
        private List<Token> tokens;
        Parser(string input)
        {
            if (input == null) throw new ParseError();
            this.tokens = Token.Tokenize(input);
        }
        public static object Parse(string text)
        {
            Parser parser = new Parser(text);
            return parser.ReadValue();
        }
        private object ReadList()
        {
            ArrayList buffer = new ArrayList();
            while (true)
            {
                sanity -= 1;
                if (sanity <= 0)
                    throw new SanityError();
                if (tokens.Count == 0)
                    throw new ParseError();
                if (tokens[0].type_ == TokenType.CLOSE_BRACKET)
                {
                    tokens.RemoveAt(0);
                    return buffer.ToArray();
                }
                if (tokens[0].type_ == TokenType.COMMA)
                {
                    tokens.RemoveAt(0);
                    continue;
                }
                buffer.Add(this.ReadValue());
            }
        }
        private object ReadDict()
        {
            throw new NotImplemented();
        }
        private object ReadPyob()
        {
            throw new NotImplemented();
        }
        private object InterpretBareword(string word)
        {
            string lower = word.ToLower();
            if (lower == "true") return true;
            if (lower == "false") return false;
            if (lower == "null") return null;
            if (lower == "none") return null;
            if (lower == "nan") return System.Double.NaN;
            if (lower == "inf") return System.Double.PositiveInfinity;
            throw new ParseError();
        }
        public object ReadValue()
        {
            while (true)
            {
                if (tokens.Count == 0)
                    throw new ParseError();
                Token popped = tokens[0];
                tokens.RemoveAt(0);
                if (popped.type_ == TokenType.SEMI) continue;
                if (popped.type_ == TokenType.NUMBER || 
                    popped.type_ == TokenType.QUOTED)
                    return popped.value_;
                if (popped.type_ == TokenType.OPEN_BRACKET)
                    return ReadList();
                if (popped.type_ == TokenType.OPEN_CURLY)
                    return ReadDict();
                if (popped.type_ != TokenType.BAREWORD)
                    throw new ParseError();
                if (tokens.Count > 0 &&
                    tokens[0].type_ == TokenType.OPEN_PAREN)
                {
                    tokens.RemoveAt(0);
                    return ReadPyob();
                }
                return InterpretBareword(popped.value_.ToString());
            }
        }
    }
}
