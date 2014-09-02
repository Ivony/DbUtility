using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{
  public class SqlParameterizedQueryExecuteContext : ParameterizedQueryExecuteContext, IDbExecuteContext, IAsyncDbExecuteContext
  {
    private SqlCommand _command;
    private   SqlServerHandler _host;

    internal SqlParameterizedQueryExecuteContext( SqlServerHandler host, SqlCommand command )
    {
      _host = host;
      _command = command;
    }


    public override IDbResult Execute()
    {
      _command.Connection.Open();
      return new SqlDbResult( _command.ExecuteReader() );

    }

    public async Task<IAsyncDbResult> ExecuteAsync( CancellationToken token = default( CancellationToken ) )
    {
      await _command.Connection.OpenAsync();

      return new SqlAsyncDbResult( await _command.ExecuteReaderAsync() );
    }
  }
}
