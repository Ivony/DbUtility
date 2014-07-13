using Ivony.Data.QuickQuery.Tokens;
using Ivony.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{
  public class QuickQueryTokenizer : TokenizerBase
  {

    public QuickQueryTokenizer( string text )
    {
      Initialize( text );
    }

    public object NextToken()
    {
      Match( WhiteSpace );

      if ( Match( '?' ).HasValue )
        return MatchFilterExpression();

      if ( Match( '$' ).HasValue )
        return MatchSortExpression();

      var field = MatchField();
      if ( field.HasValue )
        return field;


    }
    private FilterItem MatchFilterExpression()
    {
      throw new NotImplementedException();
    }


    private SortItem MatchSortExpression()
    {
      throw new NotImplementedException();
    }

    private FieldToken? MatchField()
    {
      var nameMatch = Match( CName );
      if ( nameMatch.Success )
      {
        Match( WhiteSpace );

        if ( !Match( '.' ).HasValue )
        {
          Match( WhiteSpace );

          var fieldNameMatch = Match( CName );
          if ( !fieldNameMatch.Success )
            throw FormatError();

          return new FieldToken( nameMatch.Value, fieldNameMatch.Value );

        }

        else
          return new FieldToken( nameMatch.Value );

      }

      else
        return null;

    }
  }
}
