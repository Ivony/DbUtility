using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Ivony.Data.SqlServer
{
  public class SqlParameterizedQueryParser : IParameterizedQueryParser<SqlCommand>
  {


    private bool _disposed = false;


    private int index = 0;
    private IList<SqlParameter> parameterList = new List<SqlParameter>();

    public string CreateParameterPlacehold( object parameterValue )
    {
      if ( _disposed )
        throw new ObjectDisposedException( "SqlParameterizedQueryParser" );


      var name = "Param" + index++;
      var parameter = new SqlParameter( name, parameterValue );

      return name;
    }

    public SqlCommand CreateCommand( string commandText )
    {
      if ( _disposed )
        throw new ObjectDisposedException( "SqlParameterizedQueryParser" );


      var command = new SqlCommand();
      command.CommandText = commandText;
      command.Parameters.AddRange( parameterList.ToArray() );

      _disposed = true;

      return command;
    }
  }
}
