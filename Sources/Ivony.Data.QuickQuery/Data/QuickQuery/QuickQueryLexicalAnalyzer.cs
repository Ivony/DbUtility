using Ivony.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{
  public class QuickQueryLexicalAnalyzer : LexicalAnalyzer
  {




    protected static ITextToken ReverseAlias( TextScaner scaner )
    {

      return MatchLiteral( scaner, ":>" );

    }

    protected static ITextToken Alias( TextScaner scaner )
    {

      return MatchLiteral( scaner, ":" );

    }




    protected static ITextToken TableName( TextScaner scaner )
    {

      return MatchRegex( scaner, @"@" + CNameRegex, "TableName" );

    }

    protected static ITextToken Name( TextScaner scaner )
    {

      return MatchRegex( scaner, CNameRegex, "Name" );

    }


    protected static ITextToken WhiteSpace( TextScaner scaner )
    {

      return MatchRegex( scaner, WhitespaceRegex, "Whitespace" );

    }


  }
}
