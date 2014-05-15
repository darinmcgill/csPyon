using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using csPyon;

namespace testCsPyon
{

    [TestClass]
    public class UnitTest1
    {
        public static bool Same(object a, object b)
        {
            if (a == null || b == null)
                return (a == null && b == null);
            if (a.GetType() != b.GetType())
                return false;
            if (a.GetType() == typeof(System.Double))
                return Math.Abs(Convert.ToDouble(a) - Convert.ToDouble(b)) < 1e-10;
            if (a.GetType() == typeof(System.Int32))
                return Convert.ToInt32(a) == Convert.ToInt32(b);
            return a == b;
        }
        public static void AssertMatches(object[] a, object[] b)
        {
            if (a == null || b == null) 
                throw new Exception("argument is null");
            if (a.Length != b.Length)
                throw new Exception("lengths don't match");
            for (int i = 0; i < a.Length; i++)
                if (!Same(a[i],b[i]))
                    throw new Exception("not the same: " 
                        + a[i].ToString() 
                        + " " 
                        + b[i].ToString()
                        + " "
                        + a[i].GetType().ToString()
                        + " "
                        + b[i].GetType().ToString()
                        );
        }

        [TestMethod]
        public void TestStrings()
        {
            List<Token> tokens = Token.Tokenize("hello 'universe' \"foo\" bar");
            Assert.AreEqual(tokens[0].ToString(), "Bareword('hello')");
            Assert.AreEqual(tokens[1].ToString(), "Quoted('universe')");
            Assert.AreEqual(tokens[2].ToString(), "Quoted('foo')");
            Assert.AreEqual(tokens[3].ToString(), "Bareword('bar')");
        }


        [TestMethod]
        public void TestNumbers()
        {
            List<Token> tokens = Token.Tokenize("45 0xFF -3 -1.7e-2");
            Assert.AreEqual(tokens[0].ToString(), "Number(45)");
            Assert.AreEqual(tokens[1].ToString(), "Number(255)");
            Assert.AreEqual(tokens[2].ToString(), "Number(-3)");
            Assert.AreEqual(tokens[3].ToString(), "Number(-0.017)");
        }


        [TestMethod]
        public void TestSyntax()
        {
            List<Token> tokens = Token.Tokenize("( ) ; = : [ ] ");
            Assert.AreEqual(tokens[0].ToString(), "Syntax('(')");
            Assert.AreEqual(tokens[1].ToString(), "Syntax(')')");
            Assert.AreEqual(tokens[2].ToString(), "Syntax(';')");
            Assert.AreEqual(tokens[3].ToString(), "Syntax('=')");
        }


        [TestMethod]
        public void TestPyob()
        {
            Pyob pyob = new Pyob("foo");
            Assert.AreEqual("foo()", pyob.ToString());
            pyob.keyed["cheese"] = "fries";
            Assert.AreEqual("foo(cheese='fries')", pyob.ToString());
        }

        [TestMethod]
        public void TestSimple()
        {
            Assert.AreEqual(true, Parser.Parse(" True false"));
            Assert.AreEqual(3.54, Parser.Parse("3.54"));
        }

        [TestMethod]
        public void TestRepr()
        {
            object[] ar1 = new object[3];
            ar1[0] = null;
            ar1[1] = 5;
            ar1[2] = "foo";
            Hashtable ht = new Hashtable();
            ht["cheese"] = "fries";
            object[] ar2 = new object[3];
            ar2[0] = false;
            ar2[1] = ar1;
            ar2[2] = ht;
            string repr = Pyob.Repr(ar2);
            string exp = "[False,[null,5,'foo'],{'cheese':'fries'}]";
            Assert.AreEqual(exp,repr);
        }

        [TestMethod]
        public void TestFancy()
        {
            string input = "{'foo':17,'bar':[5,19]}";
            object got = Parser.Parse(input);
            Assert.AreEqual("Hashtable", got.GetType().Name);
            Hashtable hashtable = (Hashtable)got;
            Assert.AreEqual(17, hashtable["foo"]);
            object[] arr = (object[])hashtable["bar"];
            Assert.AreEqual(19, arr[1]);
        }

        [TestMethod]
        public void TestParsePyob()
        {
            string input = "foo(3,'cheese',bar=99,ate=[])";
            object got = Parser.Parse(input);
            Assert.AreEqual("Pyob", got.GetType().Name);
            Pyob pyob = (Pyob)got;
            Assert.AreEqual("foo",pyob.head);
            Assert.AreEqual(2, pyob.ordered.Length);
            Assert.AreEqual("cheese", pyob.ordered[1]);
            Assert.AreEqual(99, pyob.keyed["bar"]);
            Assert.IsTrue(pyob.keyed["ate"].GetType().IsArray);
        }

        [TestMethod]
        public void TestDouble()
        {
            string x = "[13505, w(s(asks=1,bids=1),em=0,exTpv=2,pnl=0.0,)]";
            object got = Parser.Parse(x);
            Assert.IsTrue(got.GetType().IsArray);
            object[] arr = (object[])got;
            Pyob w = (Pyob) arr[1];
            Pyob s = (Pyob)w.ordered[0];
            Assert.AreEqual(1, s.keyed["bids"]);
        }

        [TestMethod]
        public void TestIndexer()
        {
            string x = "Foo('cheese','fries',tasty='yyy',run='walk')";
            var got = (Pyob) Parser.Parse(x);
            Assert.AreEqual("fries",got[1]);
            Assert.AreEqual("yyy",got["tasty"]);
        }

        [TestMethod]
        public void TestCmdParse()
        {
            string[] args = new string[] { "cheese", "3.9", "fries=4", "bar=", "baz=chicken", "7" };
            var pyob = CommandLineParser.Parse(args);
            AssertMatches(pyob.ordered, new object[] { "cheese", 3.9, 7 });
            Assert.AreEqual(4, pyob["fries"]);
            Assert.AreEqual(3.9, pyob[1]);
        }

        [TestMethod]
        public void TestMulti()
        {
            string[] args = new string[] { "bar=", "baz=chicken,", "testMe", "nice=17,beef" };
            var pyob = CommandLineParser.Parse(args);
            Assert.AreEqual(null, pyob["bar"]);
            Assert.AreEqual("testMe", pyob[0]);
            Assert.AreEqual(17, ((object[]) pyob["nice"])[0]);
            Assert.AreEqual("beef", ((object[])pyob["nice"])[1]);
        }
    }
}
