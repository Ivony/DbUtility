using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

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



    public class TestLexicalAnlyzer : LexicalAnalyzer
    {

      protected static ITextToken Test( TextScaner scaner )
      {

        return MatchLiteral( scaner, "Test" );

      }


      protected static ITextToken WhiteSpace( TextScaner scaner )
      {

        return MatchRegex( scaner, new Regex( @"\G\s+", RegexOptions.Compiled ) );

      }

    }
  }
}

