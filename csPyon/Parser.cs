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
            return parser.ReadValue(true);
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
                buffer.Add(this.ReadValue(false));
            }
        }
        private object ReadDict()
        {
            Hashtable hashTable = new Hashtable();
            while (true)
            {
                sanity -= 1;
                if (sanity <= 0)
                    throw new SanityError();
                if (tokens.Count == 0)
                    throw new ParseError();
                if (tokens[0].type_ == TokenType.CLOSE_CURLY)
                {
                    tokens.RemoveAt(0);
                    return hashTable;
                }
                if (tokens[0].type_ == TokenType.COMMA)
                {
                    tokens.RemoveAt(0);
                    continue;
                }
                if (tokens.Count < 3)
                    throw new ParseError();
                if (!(tokens[1].type_ == TokenType.COLON || 
                      tokens[1].type_ == TokenType.EQ_SIGN))
                    throw new ParseError();
                if (!(tokens[0].type_ == TokenType.QUOTED ||
                      tokens[0].type_ == TokenType.BAREWORD))
                    throw new ParseError();
                string key = tokens[0].value_.ToString();
                tokens.RemoveAt(0);
                tokens.RemoveAt(0);
                object value = ReadValue(false);
                hashTable[key] = value;
            }
        }
        private object ReadPyob(string head)
        {
            List<object> ordered = new List<object>();
            Hashtable hashTable = new Hashtable();
            while (true)
            {
                if (tokens.Count == 0)
                    throw new ParseError();
                if (tokens[0].type_ == TokenType.CLOSE_PAREN)
                {
                    tokens.RemoveAt(0);
                    return new Pyob(
                        head,ordered.ToArray(),hashTable);
                }
                if (tokens.Count >= 4 &&
                    tokens[0].type_ == TokenType.BAREWORD &&
                    tokens[1].type_ == TokenType.EQ_SIGN)
                {
                    string key = tokens[0].value_.ToString();
                    tokens.RemoveAt(0);
                    tokens.RemoveAt(0);
                    hashTable[key] = ReadValue(false);
                    continue;
                }
                if (tokens[0].type_ == TokenType.COMMA)
                {
                    tokens.RemoveAt(0);
                    continue;
                }
                ordered.Add(ReadValue(false));
            }
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
        public object ReadValue(bool top)
        {
            while (true)
            {
                if (tokens.Count == 0)
                    throw new ParseError();
                Token popped = tokens[0];
                tokens.RemoveAt(0);
                if (popped.type_ == TokenType.SEMI)
                {
                    if (!top) throw new ParseError();
                    continue;
                }
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
                    return ReadPyob(popped.value_.ToString());
                }
                return InterpretBareword(popped.value_.ToString());
            }
        }
    }
}
