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
using System.Linq;

namespace Ivony.Data
{
  /// <summary>
  /// 用于操作 SQL Server 的数据库访问工具
  /// </summary>
  [Serializable]
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





    IDbExecuteContext IDbExecutor<ParameterizedQuery>.Execute( ParameterizedQuery query )
    {
      return CreateExecuteContext( CreateCommand( query ) );
    }

    Task<IDbExecuteContext> IAsyncDbExecutor<ParameterizedQuery>.ExecuteAsync( ParameterizedQuery query )
    {
      return CreateAsyncExecuteContext( CreateCommand( query ) );
    }


    private SqlCommand CreateCommand( ParameterizedQuery query )
    {
      return query.CreateCommand( new SqlParameterizedQueryParser( this ) );
    }



    IDbExecuteContext IDbExecutor<StoredProcedureQuery>.Execute( StoredProcedureQuery query )
    {
      var command = CreateCommand( query );
      return CreateExecuteContext( command );
    }

    Task<IDbExecuteContext> IAsyncDbExecutor<StoredProcedureQuery>.ExecuteAsync( StoredProcedureQuery query )
    {
      var command = CreateCommand( query );
      return CreateAsyncExecuteContext( command );
    }


    private SqlCommand CreateCommand( StoredProcedureQuery query )
    {
      var command = new SqlCommand( query.Name );
      command.Parameters.AddRange( query.Parameters.Select( pair => CreateParameter( pair.Key, pair.Value ) ).ToArray() );

      return command;
    }


    protected virtual IDbExecuteContext CreateExecuteContext( SqlCommand command )
    {
      var connection = new SqlConnection( ConnectionString );
      connection.Open();
      command.Connection = connection;

      return new SqlDbExecuteContext( connection, command.ExecuteReader() );
    }


    protected virtual async Task<IDbExecuteContext> CreateAsyncExecuteContext( SqlCommand command )
    {
      var connection = new SqlConnection( ConnectionString );
      await connection.OpenAsync();
      command.Connection = connection;

      return new SqlDbExecuteContext( connection, await command.ExecuteReaderAsync() );
    }




    public virtual SqlParameter CreateParameter( string name, object value )
    {
      if ( !name.StartsWith( "@" ) )
        throw new InvalidOperationException( "对于 SQL Server 而言，参数名必须以 '@' 开头" );

      return new SqlParameter( name, value );
    }

  }
}
