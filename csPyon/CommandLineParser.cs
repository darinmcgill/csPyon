using System;
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
            throw new Exception("not implemented");
        }
        public static Pyob Parse(string[] args)
        {
            var keyed = new Hashtable();
            var ordered = new List<object>();
            foreach (string arg in args)
            {
                if (arg.Contains('='))
                {
                    var parts = arg.Split('=');
                    keyed[parts[0]] = Parser.Parse(parts[1]);
                }
                else
                {
                }

            }
            throw new Exception("not implemented");
        }
    }
}
