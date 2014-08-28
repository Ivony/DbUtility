using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Ivony.Data.QuickQuery;

namespace Ivony.Parser.Test
{
  [TestClass]
  public class LexicalAnalyzerTest
  {
    [TestMethod]
    public void Test1()
    {

      var array = new TestLexicalAnlyzer().Analyze( "Test Test\t\nTestTest!" ).ToArray();

      Assert.AreEqual( array.Length, 6 );
      Assert.AreEqual( array[0].ToString(), "Test" );
      Assert.AreEqual( array[1].ToString(), " " );
      Assert.AreEqual( array[2].ToString(), "Test" );
      Assert.AreEqual( array[3].ToString(), "\t\n" );
      Assert.AreEqual( array[4].ToString(), "Test" );
      Assert.AreEqual( array[5].ToString(), "Test" );


    }


    [TestMethod]
    public void Test2()
    {

      var array = new QuickQueryLexicalAnalyzer().Analyze( "abc :> a1 a2 :X123" ).ToArray();


      Assert.AreEqual( array.Length, 10 );
      Assert.AreEqual( array[0].ToString(), "abc" );
      Assert.AreEqual( array[1].ToString(), " " );
      Assert.AreEqual( array[2].ToString(), ":>" );
      Assert.AreEqual( array[3].ToString(), " " );
      Assert.AreEqual( array[4].ToString(), "a1" );
      Assert.AreEqual( array[5].ToString(), " " );
      Assert.AreEqual( array[6].ToString(), "a2" );
      Assert.AreEqual( array[7].ToString(), " " );
      Assert.AreEqual( array[8].ToString(), ":" );
      Assert.AreEqual( array[9].ToString(), "X123" );

    }





    public class TestLexicalAnlyzer : LexicalAnalyzer
    {

      protected static ITextToken Test( TextScaner scaner )
      {

        return MatchLiteral( scaner, "Test" );

      }


      protected static ITextToken WhiteSpace( TextScaner scaner )
      {

        return MatchRegex( scaner, @"\s+" );

      }

    }
  }
}

