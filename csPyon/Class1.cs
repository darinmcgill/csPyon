using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csPyon
{
    public class Speaker
    {
        public static String getType(object obj)
        {
            return obj.GetType().ToString();
        }
        public static void speak(string name)
        {
               Console.WriteLine("Hello, " + name);
        }
    }
}
