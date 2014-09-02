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




    protected static TextToken? ReverseAlias( TextScaner scaner )
    {

      return MatchLiteral( scaner, ":>" );

    }

    protected static TextToken? Alias( TextScaner scaner )
    {

      return MatchLiteral( scaner, ":" );

    }




    protected static TextToken? TableName( TextScaner scaner )
    {

      return MatchRegex( scaner, @"@" + CNameRegex, "TableName" );

    }

    protected static TextToken? Name( TextScaner scaner )
    {

      return MatchRegex( scaner, CNameRegex, "Name" );

    }


    protected static TextToken? WhiteSpace( TextScaner scaner )
    {

      return MatchRegex( scaner, WhitespaceRegex, "Whitespace" );

    }


  }
}
