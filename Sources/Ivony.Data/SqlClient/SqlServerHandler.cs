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

using System.Linq;
using System.Threading;
using System.Data.Common;
using Ivony.Data.Common;

namespace Ivony.Data.SqlClient
{
  /// <summary>
  /// 用于操作 SQL Server 的数据库访问工具
  /// </summary>
  public class SqlServerHandler : DbHandlerBase, IParameterizedQueryExecutor<SqlParameterizedQueryExecuteContext>, IDbTransactionProvider<SqlServerHandler>
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
    /// 创建 SqlServer 数据库查询执行程序
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">当前要使用的数据库配置信息</param>
    public SqlServerHandler( string connectionString, IDbTraceService traceService = null, TimeSpan? commandTimeout = null, TimeSpan? connectionTimeout = null, bool immediateExecution = false )
    {
      if ( connectionString == null )
        throw new ArgumentNullException( "connectionString" );


      ConnectionString = connectionString;
    }


    /// <summary>
    /// 当前要使用的数据库配置信息
    /// </summary>
    protected SqlDbConfiguration Configuration
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建数据库事务上下文
    /// </summary>
    /// <returns>数据库事务上下文</returns>
    public SqlDbTransactionContext CreateTransaction()
    {
      return new SqlDbTransactionContext( ConnectionString, Configuration );
    }


    IDbTransactionContext<SqlServerHandler> IDbTransactionProvider<SqlServerHandler>.CreateTransaction()
    {
      return CreateTransaction();
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


    SqlParameterizedQueryExecuteContext IParameterizedQueryExecutor<SqlParameterizedQueryExecuteContext>.Execute( ParameterizedQuery query )
    {
      var command  = CreateCommand( query );
      ApplyConnection( command );

      return new SqlParameterizedQueryExecuteContext( this, command );
    }

    /// <summary>
    /// 对指定的 SQL 命令对象应用数据库连接，此时连接可能处于未打开状态。
    /// </summary>
    /// <param name="command">要应用数据库连接的 SQL 命令对象</param>
    protected virtual void ApplyConnection( SqlCommand command )
    {
      command.Connection = new SqlConnection( ConnectionString );
    }

    protected override IDbTraceService TraceService
    {
      get { return null; }
    }
  }

}
