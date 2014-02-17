using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Ivony.Fluent;

namespace Ivony.Data.SqlServer
{
  public class SqlParameterizedQueryParser : IParameterizedQueryParser<SqlCommand>
  {


    private bool _disposed = false;


    private int index = 0;
    private IDictionary<string, object> parameterList = new Dictionary<string, object>();



    private object _sync = new object();

    public object SyncRoot
    {
      get { return _sync; }
    }


    public string CreateParameterPlacehold( object value )
    {
      if ( _disposed )
        throw new ObjectDisposedException( "SqlParameterizedQueryParser" );


      var name = "@Param" + index++;
      parameterList.Add( name, value );

      return name;
    }


    public SqlCommand CreateCommand( string commandText )
    {
      if ( _disposed )
        throw new ObjectDisposedException( "SqlParameterizedQueryParser" );


      var command = new SqlCommand();
      command.CommandText = commandText;
      parameterList.ForAll( pair => command.Parameters.AddWithValue( pair.Key, pair.Value ) );
      _disposed = true;

      return command;
    }


    public void Dispose()
    {
      _disposed = true;
    }
  }
}
