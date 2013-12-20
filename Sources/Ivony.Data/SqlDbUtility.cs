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
  [Serializable]
  public class SqlDbUtility : IAsyncDbExecutor<TemplateQuery>, IAsyncDbExecutor<StoredProcedureQuery>
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





    IDbExecuteContext IDbExecutor<TemplateQuery>.Execute( TemplateQuery query )
    {
      var command = CreateCommand( query );

      var connection = new SqlConnection( ConnectionString );
      connection.Open();
      command.Connection = connection;


      return new SqlDbExecuteContext( command.Connection, command.ExecuteReader() );
    }

    async Task<IDbExecuteContext> IAsyncDbExecutor<TemplateQuery>.ExecuteAsync( TemplateQuery query )
    {
      var command = CreateCommand( query );

      var connection = new SqlConnection( ConnectionString );
      await connection.OpenAsync();
      command.Connection = connection;

      return new SqlDbExecuteContext( connection, await command.ExecuteReaderAsync() );
    }


    private SqlCommand CreateCommand( TemplateQuery query )
    {
      return CreateCommand( query.CreateQuery() );
    }

    private SqlCommand CreateCommand( ParameterizedQuery query )
    {
      var command = query.CreateCommand( new SqlParameterizedQueryParser() ) as SqlCommand;
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
