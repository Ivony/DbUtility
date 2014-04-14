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
    public SqlDbUtility( string connectionString, IDbTracing tracing = null )
    {
      ConnectionString = connectionString;
      Tracing = tracing;
    }

    internal SqlDbUtility( SqlDbTransactionContext transaction )
    {
      TransactionContext = transaction;
      ConnectionString = transaction.Connection.ConnectionString;
    }



    protected IDbTracing Tracing
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建数据库访问工具
    /// </summary>
    /// <param name="name">连接字符串名称</param>
    /// <returns>数据库访问工具</returns>
    public static SqlDbUtility Create( string name, IDbTracing tracing = null )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return new SqlDbUtility( setting.ConnectionString );
    }


    /// <summary>
    /// 创建数据库事务上下文
    /// </summary>
    /// <returns>数据库事务上下文</returns>
    public SqlDbTransactionContext CreateTransaction()
    {
      return new SqlDbTransactionContext( ConnectionString );
    }


    IDbTransactionContext<SqlDbUtility> IDbTransactionProvider<SqlDbUtility>.CreateTransaction()
    {
      return CreateTransaction();
    }



    /// <summary>
    /// 当前所处的事务，如果有的话。
    /// </summary>
    protected SqlDbTransactionContext TransactionContext
    {
      get;
      private set;
    }

    /// <summary>
    /// 执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns>查询执行上下文</returns>
    protected IDbExecuteContext Execute( SqlCommand command )
    {

      if ( TransactionContext != null )
      {
        command.Connection = TransactionContext.Connection;
        command.Transaction = TransactionContext.Transaction;
        return new SqlDbExecuteContext( TransactionContext, command.ExecuteReader(), Tracing );
      }
      else
      {
        var connection = new SqlConnection( ConnectionString );
        connection.Open();
        command.Connection = connection;
        return new SqlDbExecuteContext( connection, command.ExecuteReader(), Tracing );
      }
    }


    /// <summary>
    /// 异步执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询执行上下文</returns>
    protected async Task<IAsyncDbExecuteContext> ExecuteAsync( SqlCommand command, CancellationToken token )
    {
      if ( TransactionContext != null )
      {
        command.Connection = TransactionContext.Connection;
        command.Transaction = TransactionContext.Transaction;
        return new SqlDbExecuteContext( TransactionContext, await command.ExecuteReaderAsync( token ), Tracing );
      }
      else
      {
        var connection = new SqlConnection( ConnectionString );
        await connection.OpenAsync( token );
        command.Connection = connection;
        return new SqlDbExecuteContext( connection, await command.ExecuteReaderAsync( token ), Tracing );
      }
    }



    IDbExecuteContext IDbExecutor<ParameterizedQuery>.Execute( ParameterizedQuery query )
    {
      return Execute( CreateCommand( query ) );
    }

    Task<IAsyncDbExecuteContext> IAsyncDbExecutor<ParameterizedQuery>.ExecuteAsync( ParameterizedQuery query, CancellationToken token )
    {
      return ExecuteAsync( CreateCommand( query ), token );
    }


    /// <summary>
    /// 从参数化查询创建查询命令对象
    /// </summary>
    /// <param name="query">参数化查询对象</param>
    /// <returns>SQL 查询命令对象</returns>
    protected SqlCommand CreateCommand( ParameterizedQuery query )
    {
      return query.CreateCommand( new SqlParameterizedQueryParser() );
    }



    IDbExecuteContext IDbExecutor<StoredProcedureQuery>.Execute( StoredProcedureQuery query )
    {
      return Execute( CreateCommand( query ) );
    }

    Task<IAsyncDbExecuteContext> IAsyncDbExecutor<StoredProcedureQuery>.ExecuteAsync( StoredProcedureQuery query, CancellationToken token )
    {
      return ExecuteAsync( CreateCommand( query ), token );
    }


    /// <summary>
    /// 通过存储过程查询创建 SqlCommand 对象
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    protected SqlCommand CreateCommand( StoredProcedureQuery query )
    {
      var command = new SqlCommand( query.Name );
      command.CommandType = CommandType.StoredProcedure;
      query.Parameters.ForAll( pair => command.Parameters.AddWithValue( pair.Key, pair.Value ) );
      return command;
    }
  }
}
