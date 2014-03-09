using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using csPyon;

namespace testCsPyon
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestStrings()
        {
            List<Token> tokens = Token.tokenize("hello 'universe' \"foo\" bar");
            Assert.AreEqual(tokens[0].ToString(), "Bareword('hello')");
            Assert.AreEqual(tokens[1].ToString(), "Quoted('universe')");
            Assert.AreEqual(tokens[2].ToString(), "Quoted('foo')");
            Assert.AreEqual(tokens[3].ToString(), "Bareword('bar')");
        }


        [TestMethod]
        public void TestNumbers()
        {
            List<Token> tokens = Token.tokenize("45 0xFF -3 -1.7e-2");
            Assert.AreEqual(tokens[0].ToString(), "Number(45)");
            Assert.AreEqual(tokens[1].ToString(), "Number(255)");
            Assert.AreEqual(tokens[2].ToString(), "Number(-3)");
            Assert.AreEqual(tokens[3].ToString(), "Number(-0.017)");
        }


    }
}
