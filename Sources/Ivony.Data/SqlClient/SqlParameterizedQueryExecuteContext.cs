using Ivony.Data.Common;
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
  public class SqlParameterizedQueryExecuteContext : IParameterizedQueryExecuteContext, IDbExecuteContext, IAsyncDbExecuteContext
  {
    private ParameterizedQuery _query;
    private SqlServerHandler _handler;
    private IDbTracing _tracing;

    internal SqlParameterizedQueryExecuteContext( SqlServerHandler handler, ParameterizedQuery query, IDbTracing tracing )
    {
      _handler = handler;
      _query = query;
      _tracing = tracing;
    }





    public ParameterizedQuery Query
    {
      get { return _query; }

    }


    public IDbResult Execute()
    {


      var command = CreateCommand( Query );

      if ( _tracing != null )
      {
        _tracing.OnExecuting( command );

        SqlDbResult result;

        try
        {
          result = ExecuteCore( command );
        }
        catch ( Exception e )
        {
          _tracing.OnException( e );
          throw;
        }

        _tracing.OnLoadingData( result );
        return result;
      }

      else
        return ExecuteCore( command );
    }

    public async Task<IAsyncDbResult> ExecuteAsync()
    {


      var command = CreateCommand( Query );

      if ( _tracing != null )
      {
        _tracing.OnExecuting( command );

        SqlAsyncDbResult result;

        try
        {
          result = await ExecuteAsyncCore( command );
        }
        catch ( Exception e )
        {
          _tracing.OnException( e );
          throw;
        }

        _tracing.OnLoadingData( result );
        return result;
      }

      else
        return await ExecuteAsyncCore( command );
    }

    /// <summary>
    /// 从参数化查询创建查询命令对象
    /// </summary>
    /// <param name="query">参数化查询对象</param>
    /// <returns>SQL 查询命令对象</returns>
    protected SqlCommand CreateCommand( ParameterizedQuery query )
    {
      return new SqlParameterizedQueryParser().Parse( query );
    }


    protected SqlDbResult ExecuteCore( SqlCommand command )
    {
      command.Connection.Open();
      return new SqlDbResult( command.ExecuteReader() );

    }

    public async Task<SqlAsyncDbResult> ExecuteAsyncCore( SqlCommand command, CancellationToken token = default( CancellationToken ) )
    {
      await command.Connection.OpenAsync();
      return new SqlAsyncDbResult( await command.ExecuteReaderAsync() );

    }
  }
}
