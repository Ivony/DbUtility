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
    public SqlDbUtility( string connectionString )
    {
      ConnectionString = connectionString;
    }

    internal SqlDbUtility( SqlDbTransactionContext transaction )
    {
      TransactionContext = transaction;
      ConnectionString = transaction.Connection.ConnectionString;
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
        return new SqlDbExecuteContext( TransactionContext, command.ExecuteReader() );
      }
      else
      {
        var connection = new SqlConnection( ConnectionString );
        connection.Open();
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
        command.Transaction = TransactionContext.Transaction;
        return new SqlDbExecuteContext( TransactionContext, await command.ExecuteReaderAsync() );
      }
      else
      {
        var connection = new SqlConnection( ConnectionString );
        await connection.OpenAsync();
        command.Connection = connection;
        return new SqlDbExecuteContext( connection, await command.ExecuteReaderAsync() );
      }
    }



    IDbExecuteContext IDbExecutor<ParameterizedQuery>.Execute( ParameterizedQuery query )
    {
      return Execute( CreateCommand( query ) );
    }

    Task<IDbExecuteContext> IAsyncDbExecutor<ParameterizedQuery>.ExecuteAsync( ParameterizedQuery query )
    {
      return ExecuteAsync( CreateCommand( query ) );
    }


    protected SqlCommand CreateCommand( ParameterizedQuery query )
    {
      return query.CreateCommand( new SqlParameterizedQueryParser() );
    }



    /// <summary>
    /// 创建查询参数
    /// </summary>
    /// <param name="name">参数名</param>
    /// <param name="value">参数值</param>
    /// <returns>SQL 查询参数对象</returns>
    public virtual SqlParameter CreateParameter( string name, object value )
    {
      throw new NotImplementedException();
      if ( !name.StartsWith( "@" ) )
        throw new InvalidOperationException( "适用于 SQL Server 的查询参数必须以 '@' 符号开头" );

      return new SqlParameter( name, value );
    }



    IDbExecuteContext IDbExecutor<StoredProcedureQuery>.Execute( StoredProcedureQuery query )
    {
      return Execute( CreateCommand( query ) );
    }

    Task<IDbExecuteContext> IAsyncDbExecutor<StoredProcedureQuery>.ExecuteAsync( StoredProcedureQuery query )
    {
      return ExecuteAsync( CreateCommand( query ) );
    }


    protected SqlCommand CreateCommand( StoredProcedureQuery query )
    {
      var command = new SqlCommand( query.Name );
      command.CommandType = CommandType.StoredProcedure;
      query.Parameters.ForAll( pair => command.Parameters.AddWithValue( pair.Key, pair.Value ) );
      return command;
    }
  }
}
