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

namespace Ivony.Data
{
  /// <summary>
  /// 用于操作 SQL Server 的数据库访问工具
  /// </summary>
  public class SqlDbUtility : IAsyncDbExecutor<ParameterizedQuery>, IAsyncDbExecutor<StoredProcedureQuery>
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
    public SqlDbUtility( string connectionString )
    {
      ConnectionString = connectionString;
    }

    internal SqlDbUtility( SqlDbTransactionContext transaction )
    {
      ConnectionString = TransactionContext.Connection.ConnectionString;
      TransactionContext = transaction;
    }


    /// <summary>
    /// 创建数据库访问工具
    /// </summary>
    /// <param name="name">连接字符串名称</param>
    /// <returns>数据库访问工具</returns>
    public static SqlDbUtility Create( string name )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return new SqlDbUtility( setting.ConnectionString );
    }


    public SqlDbTransactionContext CreateTransactrion()
    {
      return new SqlDbTransactionContext( ConnectionString );
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
        return new SqlDbExecuteContext( TransactionContext, command.ExecuteReader() );
      }
      else
      {
        var connection = new SqlConnection( ConnectionString );
        command.Connection = connection;

        return new SqlDbExecuteContext( connection, command.ExecuteReader() );
      }
    }


    /// <summary>
    /// 异步执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns>查询执行上下文</returns>
    protected async Task<IDbExecuteContext> ExecuteAsync( SqlCommand command )
    {
      if ( TransactionContext != null )
      {
        command.Connection = TransactionContext.Connection;
        return new SqlDbExecuteContext( TransactionContext, await command.ExecuteReaderAsync() );
      }
      else
      {
        var connection = new SqlConnection( ConnectionString );
        command.Connection = connection;

        return new SqlDbExecuteContext( connection, await command.ExecuteReaderAsync() );
      }
    }



    IDbExecuteContext IDbExecutor<ParameterizedQuery>.Execute( ParameterizedQuery query )
    {
      var command = CreateCommand( query );
      return Execute( command );
    }

    Task<IDbExecuteContext> IAsyncDbExecutor<ParameterizedQuery>.ExecuteAsync( ParameterizedQuery query )
    {
      var command = CreateCommand( query );
      return ExecuteAsync( command );
    }


    private SqlCommand CreateCommand( ParameterizedQuery query )
    {
      var command = query.CreateCommand( new SqlParameterizedQueryParser() );
      var connection = new SqlConnection( ConnectionString );
      command.Connection = connection;

      return command;
    }



    IDbExecuteContext IDbExecutor<StoredProcedureQuery>.Execute( StoredProcedureQuery query )
    {
      throw new NotImplementedException();
    }

    Task<IDbExecuteContext> IAsyncDbExecutor<StoredProcedureQuery>.ExecuteAsync( StoredProcedureQuery query )
    {
      throw new NotImplementedException();
    }

  }
}
