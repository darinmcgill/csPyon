﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace csPyon
{
    public class CommandLineParser
    {
        public static object ReadValue(string stuff)
        {
            if (stuff.Length == 0) return null;
            char first = stuff[0];
            if (!(System.Char.IsDigit(first) || first == '.'))
                return stuff;
            var tokens = Token.Tokenize(stuff);
            if (tokens[0].type_ != TokenType.NUMBER)
                throw new Exception("argument");
            return tokens[0].value_;
        }
        public static object ReadMany(string stuff)
        {
            var output = new List<object>();
            var parts = stuff.Split(new char[] { '+' });
            foreach (string item in parts)
                output.Add(ReadValue(item));
            return output.ToArray();
        }
        public static Pyob Parse(string[] args)
        {
            var keyed = new Hashtable();
            var ordered = new List<object>();
            foreach (string arg in args)
            {
                if (arg.Contains('='))
                {
                    var splitOn = new char[] { '=' };
                    var parts = arg.Split(splitOn,2,StringSplitOptions.None);
                    if (parts[1].Contains('+'))
                        keyed[parts[0]] = ReadMany(parts[1]);
                    else
                        keyed[parts[0]] = ReadValue(parts[1]);
                }
                else
                {
                    ordered.Add(ReadValue(arg));
                }
            }
            return new Pyob("args", ordered.ToArray(), keyed);
        }
    }
}
