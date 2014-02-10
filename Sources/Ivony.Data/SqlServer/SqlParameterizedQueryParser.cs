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



    private object _sync = new object();
    private SqlDbUtility _dbUtility;

    public SqlParameterizedQueryParser( SqlDbUtility dbUtility )
    {
      _dbUtility = dbUtility;
    }

    public object SyncRoot
    {
      get { return _sync; }
    }


    public string CreateParameterPlacehold( object value )
    {
      if ( _disposed )
        throw new ObjectDisposedException( "SqlParameterizedQueryParser" );


      var name = "@Param" + index++;
      parameterList.Add( CreateParameter( name, value ) );

      return name;
    }

    protected SqlParameter CreateParameter( string name, object value )
    {
      return _dbUtility.CreateParameter( name, value );
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


    public void Dispose()
    {
      _disposed = true;
    }
  }
}
