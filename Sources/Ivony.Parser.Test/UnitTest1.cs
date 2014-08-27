using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Parser.Test
{
  [TestClass]
  public class LexicalAnalyzerTest
  {
    [TestMethod]
    public void Test1()
    {

      var array = new TestLexicalAnlyzer().Analyze( "Test" ).ToArray();

    }



    public class TestLexicalAnlyzer : LexicalAnalyzer
    {

      protected static ITextToken Test( TextScaner scaner )
      {
        var literal = "Test";
        if ( scaner.SubString( scaner.Offset, literal.Length ) == literal )
          return CreateToken( scaner, literal.Length, "Test" );

        else
          return null;

      }


    }
  }
}

