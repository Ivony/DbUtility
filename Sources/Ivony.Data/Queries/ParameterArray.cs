using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  public class ParameterArray : IParameterizedQueryPartial
  {


    private Array _parameters;
    private string _separator;


    public ParameterArray( Array parameters, string separator = ", " )
    {
      if ( parameters.Rank != 1 )
        throw new ArgumentException( "参数列表必须是一维数组", "parameters" );

      _parameters = parameters;
      _separator = separator;
    }

    public void AppendTo( ParameterizedQueryBuilder builder )
    {

      for ( int i = 0; i < _parameters.Length; i++ )
      {
        builder.AppendParameter( _parameters.GetValue( i ) );

        if ( i < _parameters.Length - 1 )
          builder.AppendText( _separator );

      }
    }
  }
}
