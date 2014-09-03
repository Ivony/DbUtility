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
  public class SqlDbHandler : IParameterizedQueryExecutor<SqlDbParameterizedQueryExecuteContext>, IDbTransactionProvider<SqlDbHandler>
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
    /// <param name="traceService">要使用的查询追踪服务</param>
    /// <param name="commandTimeout">查询超时时间</param>
    /// <param name="immediateExecution">是否立即执行查询</param>
    public SqlDbHandler( string connectionString, IDbTraceService traceService = null, TimeSpan? commandTimeout = null, bool immediateExecution = false )
    {
      if ( connectionString == null )
        throw new ArgumentNullException( "connectionString" );

      ConnectionString = connectionString;

      Configuration = new SqlDbConfiguration()
      {
        TraceService = traceService,
        CommandTimeout = commandTimeout,
        ImmediateExecution = immediateExecution,
      };

    }



    /// <summary>
    /// 创建 SqlServer 数据库查询执行程序
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">当前要使用的数据库配置信息</param>
    public SqlDbHandler( string connectionString, SqlDbConfiguration configuration )
    {

      if ( connectionString == null )
        throw new ArgumentNullException( "connectionString" );


      ConnectionString = connectionString;
      Configuration = new SqlDbConfiguration( configuration );
    }



    /// <summary>
    /// 创建一个新的 SqlServerHandler 对象，使用指定的追踪服务
    /// </summary>
    /// <param name="traceService">用于查询追踪的服务对象</param>
    /// <returns>使用指定追踪服务的 SqlServerHandler 对象</returns>
    public SqlDbHandler WithTraceService( IDbTraceService traceService )
    {
      return WithConfiguration( configuration => configuration.TraceService = traceService );
    }



    /// <summary>
    /// 创建一个新的 SqlServerHandler 对象，使用指定的查询超时时间
    /// </summary>
    /// <param name="timeout">指定的查询超时时间</param>
    /// <returns>使用指定查询超时时间的 SqlServerHandler 对象</returns>
    public SqlDbHandler WithCommandTimeout( TimeSpan timeout )
    {
      return WithConfiguration( configuration => configuration.CommandTimeout = timeout );
    }



    /// <summary>
    /// 创建一个新的 SqlServerHandler 对象，指定是否应当立即执行查询
    /// </summary>
    /// <param name="immediateExecution">是否应当立即执行查询，默认值是 true</param>
    /// <returns>使用指定 ImmediateExecution 设置的 SqlServerHandler 对象</returns>
    public SqlDbHandler WithImmediateExecution( bool immediateExecution = true )
    {
      return WithConfiguration( configuration => configuration.ImmediateExecution = immediateExecution );
    }


    /// <summary>
    /// 创建一个新的 SqlServerHandler 对象，对设置进行指定的修改
    /// </summary>
    /// <param name="configurationSetter">修改设置的方法</param>
    /// <returns>使用新的设置的 SqlServerHandler 对象</returns>
    protected virtual SqlDbHandler WithConfiguration( Action<SqlDbConfiguration> configurationSetter )
    {
      var newConfiguration = new SqlDbConfiguration( Configuration );
      configurationSetter( newConfiguration );

      return new SqlDbHandler( ConnectionString, newConfiguration );
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


    IDbTransactionContext<SqlDbHandler> IDbTransactionProvider<SqlDbHandler>.CreateTransaction()
    {
      return CreateTransaction();
    }





    SqlDbParameterizedQueryExecuteContext IParameterizedQueryExecutor<SqlDbParameterizedQueryExecuteContext>.Execute( ParameterizedQuery query )
    {
      return new SqlDbParameterizedQueryExecuteContext( this, query, TryCreateTracing( query ) );
    }

    private IDbTracing TryCreateTracing<TQuery>( TQuery query )
    {
      if ( TraceService == null )
        return null;

      else
        return TraceService.CreateTracing( this, query );
    }



    /// <summary>
    /// 对指定的 SQL 命令对象应用数据库连接，此时连接可能处于未打开状态。
    /// </summary>
    /// <param name="command">要应用数据库连接的 SQL 命令对象</param>
    internal virtual SqlCommand ApplyConnection( SqlCommand command )
    {
      command.Connection = new SqlConnection( ConnectionString );
      return command;
    }


    /// <summary>
    /// 对指定的 SQL 命令对象应用数据库连接和设置，此时连接可能处于未打开状态。
    /// </summary>
    /// <param name="command">要应用数据库连接的 SQL 命令对象</param>
    internal SqlCommand ApplyConnectionAndSettings( SqlCommand command )
    {
      command = ApplyConnection( command );
      if ( Configuration.CommandTimeout.HasValue )
        command.CommandTimeout = (int) Math.Ceiling( Configuration.CommandTimeout.Value.TotalSeconds );

      return command;
    }


    /// <summary>
    /// 获取查询执行需要使用的追踪服务
    /// </summary>
    protected virtual IDbTraceService TraceService
    {
      get { return Configuration.TraceService; }
    }
  }

}
