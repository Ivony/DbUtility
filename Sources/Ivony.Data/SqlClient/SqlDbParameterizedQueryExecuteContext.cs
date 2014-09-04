using Ivony.Data.Common;
using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{


  /// <summary>
  /// SQL Server 参数化查询执行上下文
  /// </summary>
  public class SqlDbParameterizedQueryExecuteContext : IParameterizedQueryExecuteContext, IDbExecuteContext, IAsyncDbExecuteContext
  {
    private ParameterizedQuery _query;
    private SqlDbHandler _handler;
    private IDbTracing _tracing;

    internal SqlDbParameterizedQueryExecuteContext( SqlDbHandler handler, ParameterizedQuery query, IDbTracing tracing )
    {
      _handler = handler;
      _query = query;
      _tracing = tracing;
    }





    /// <summary>
    /// 正在执行的参数化查询对象
    /// </summary>
    public ParameterizedQuery Query
    {
      get { return _query; }

    }


    /// <summary>
    /// 执行查询并获得结果
    /// </summary>
    /// <returns></returns>
    public IDbResult Execute()
    {
      return _tracing.TryExecuteWithTracing( CreateCommand( Query ), ExecuteCore );
    }


    /// <summary>
    /// 异步执行查询并获得结果
    /// </summary>
    /// <returns></returns>
    public async Task<IAsyncDbResult> ExecuteAsync( CancellationToken token = default( CancellationToken ) )
    {

      return await _tracing.TryExecuteAsyncWithTracing( CreateCommand( Query ), ExecuteAsyncCore, token );

    }





    [ThreadStatic]
    private static SqlDbParameterizedQueryParser parser;


    /// <summary>
    /// 从参数化查询创建查询命令对象
    /// </summary>
    /// <param name="query">参数化查询对象</param>
    /// <returns>SQL 查询命令对象</returns>
    protected SqlCommand CreateCommand( ParameterizedQuery query )
    {

      if ( parser == null )
        parser = new SqlDbParameterizedQueryParser();

      var command = parser.Parse( query );
      _handler.ApplyConnectionAndSettings( command );

      return command;
    }


    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="command">查询命令对象</param>
    /// <returns>查询结果</returns>
    protected virtual SqlDbResult ExecuteCore( SqlCommand command, IDbTracing tracing = null )
    {
      if ( command.Connection.State == ConnectionState.Closed )
        command.Connection.Open();


      var reader = command.ExecuteReader();

      if ( command.Transaction == null )
        return new SqlDbResult( reader, tracing, command.Connection );

      else
        return new SqlDbResult( reader, tracing );
    }

    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="command">查询命令对象</param>
    /// <param name="token">取消标识</param>
    /// <returns>查询结果</returns>
    public virtual async Task<SqlDbAsyncResult> ExecuteAsyncCore( SqlCommand command, IDbTracing tracing = null, CancellationToken token = default( CancellationToken ) )
    {
      if ( command.Connection.State == ConnectionState.Closed )
        await command.Connection.OpenAsync();

      var reader = await command.ExecuteReaderAsync();

      if ( command.Transaction == null )
        return new SqlDbAsyncResult( reader, tracing, command.Connection );

      else
        return new SqlDbAsyncResult( reader, tracing );
    }
  }
}
