using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections;
using System.Configuration;
using System.Threading.Tasks;
using Ivony.Data.Queries;
using Ivony.Data.SqlServer;
using Ivony.Fluent;

using System.Linq;
using System.Threading;
using System.Data.Common;

namespace Ivony.Data
{
  /// <summary>
  /// 用于操作 SQL Server 的数据库访问工具
  /// </summary>
  public class SqlDbUtility : IAsyncDbExecutor<ParameterizedQuery>, IAsyncDbExecutor<StoredProcedureQuery>, IDbTransactionProvider<SqlDbUtility>
  {



    /// <summary>
    /// 获取当前连接字符串
    /// </summary>
    protected string ConnectionString
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建 SqlDbUtility 实例
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public SqlDbUtility( string connectionString, IDbTraceService traceService = null )
    {
      ConnectionString = connectionString;
      TraceService = traceService ?? BlankTraceService.Instance;
    }


    protected IDbTraceService TraceService
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建数据库访问工具
    /// </summary>
    /// <param name="name">连接字符串名称</param>
    /// <returns>数据库访问工具</returns>
    public static SqlDbUtility Create( string name, IDbTraceService traceService = null )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return new SqlDbUtility( setting.ConnectionString, traceService );
    }


    /// <summary>
    /// 创建数据库事务上下文
    /// </summary>
    /// <returns>数据库事务上下文</returns>
    public SqlDbTransactionContext CreateTransaction()
    {
      return new SqlDbTransactionContext( ConnectionString, TraceService );
    }


    IDbTransactionContext<SqlDbUtility> IDbTransactionProvider<SqlDbUtility>.CreateTransaction()
    {
      return CreateTransaction();
    }



    /// <summary>
    /// 执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <param name="tracing">用于追踪查询过程的追踪器</param>
    /// <returns>查询执行上下文</returns>
    protected virtual IDbExecuteContext Execute( SqlCommand command, IDbTracing tracing = null )
    {

      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );


        var connection = new SqlConnection( ConnectionString );
        connection.Open();
        command.Connection = connection;

        var reader = command.ExecuteReader();
        var context = new SqlDbExecuteContext( connection, reader, tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;

      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
    }


    /// <summary>
    /// 异步执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <param name="token">取消指示</param>
    /// <param name="tracing">用于追踪查询过程的追踪器</param>
    /// <returns>查询执行上下文</returns>
    protected virtual async Task<IAsyncDbExecuteContext> ExecuteAsync( SqlCommand command, CancellationToken token, IDbTracing tracing = null )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

        var connection = new SqlConnection( ConnectionString );
        await connection.OpenAsync( token );
        command.Connection = connection;


        var reader = await command.ExecuteReaderAsync( token );
        var context = new SqlDbExecuteContext( connection, reader, tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
    }



    /// <summary>
    /// 尝试执行查询追踪器的一个追踪方法，此方法会自动判断追踪器是否存在以及对调用中出现的异常进行异常屏蔽。
    /// </summary>
    /// <param name="tracing">查询追踪器，如果有的话</param>
    /// <param name="action">要执行的追踪操作</param>
    protected void TryExecuteTracing( IDbTracing tracing, Action<IDbTracing> action )
    {
      if ( tracing == null )
        return;

      try
      {
        action( tracing );
      }
      catch
      {

      }
    }




    IDbExecuteContext IDbExecutor<ParameterizedQuery>.Execute( ParameterizedQuery query )
    {
      return Execute( CreateCommand( query ), TraceService.CreateTracing( this, query ) );
    }

    Task<IAsyncDbExecuteContext> IAsyncDbExecutor<ParameterizedQuery>.ExecuteAsync( ParameterizedQuery query, CancellationToken token )
    {
      return ExecuteAsync( CreateCommand( query ), token, TraceService.CreateTracing( this, query ) );
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



    IDbExecuteContext IDbExecutor<StoredProcedureQuery>.Execute( StoredProcedureQuery query )
    {
      return Execute( CreateCommand( query ), TraceService.CreateTracing( this, query ) );
    }

    Task<IAsyncDbExecuteContext> IAsyncDbExecutor<StoredProcedureQuery>.ExecuteAsync( StoredProcedureQuery query, CancellationToken token )
    {
      return ExecuteAsync( CreateCommand( query ), token, TraceService.CreateTracing( this, query ) );
    }


    /// <summary>
    /// 通过存储过程查询创建 SqlCommand 对象
    /// </summary>
    /// <param name="query">存储过程查询对象</param>
    /// <returns>SQL 查询命令对象</returns>
    protected SqlCommand CreateCommand( StoredProcedureQuery query )
    {
      var command = new SqlCommand( query.Name );
      command.CommandType = CommandType.StoredProcedure;
      query.Parameters.ForAll( pair => command.Parameters.AddWithValue( pair.Key, pair.Value ) );
      return command;
    }
  }



  internal class SqlDbUtilityWithTransaction : SqlDbUtility
  {
    public SqlDbUtilityWithTransaction( SqlDbTransactionContext transaction, IDbTraceService traceService = null )
      : base( transaction.Connection.ConnectionString, traceService )
    {
      TransactionContext = transaction;
    }


    /// <summary>
    /// 当前所处的事务
    /// </summary>
    protected SqlDbTransactionContext TransactionContext
    {
      get;
      private set;
    }


    /// <summary>
    /// 重写 ExecuteAsync 方法，在事务中异步执行查询
    /// </summary>
    /// <param name="command">要执行的查询命令</param>
    /// <param name="token">取消指示</param>
    /// <param name="tracing">用于追踪的追踪器</param>
    /// <returns>查询执行上下文</returns>
    protected sealed override async Task<IAsyncDbExecuteContext> ExecuteAsync( SqlCommand command, CancellationToken token, IDbTracing tracing = null )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

        command.Connection = TransactionContext.Connection;
        command.Transaction = TransactionContext.Transaction;

        var reader = await command.ExecuteReaderAsync( token );
        var context = new SqlDbExecuteContext( TransactionContext, reader, tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }

    }


    /// <summary>
    /// 执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <param name="tracing">用于追踪查询过程的追踪器</param>
    /// <returns>查询执行上下文</returns>
    protected sealed override IDbExecuteContext Execute( SqlCommand command, IDbTracing tracing = null )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

        command.Connection = TransactionContext.Connection;
        command.Transaction = TransactionContext.Transaction;

        var reader =  command.ExecuteReader();
        var context =  new SqlDbExecuteContext( TransactionContext, reader, tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
    }

  }

}
