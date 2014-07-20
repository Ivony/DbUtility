using Ivony.Data.QuickQuery.Tokens;
using Ivony.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{

#if false

  public class QuickQueryTokenizer : ParserBase
  {

    public QuickQueryTokenizer( string text )
    {
      Initialize( text );
    }

    public object NextToken()
    {
      TryMatch( WhiteSpace );

      if ( TryMatch( '?' ).HasValue )
        return MatchFilterExpression();

      if ( TryMatch( '$' ).HasValue )
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
      var nameMatch = TryMatch( CName );
      if ( nameMatch.Success )
      {
        TryMatch( WhiteSpace );

        if ( !TryMatch( '.' ).HasValue )
        {
          TryMatch( WhiteSpace );

          var fieldNameMatch = TryMatch( CName );
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
#endif
}
