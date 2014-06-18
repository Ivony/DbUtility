using System;
using System.Configuration;
using Ivony.Data.PostgreSQL.PostgreSqlClient;
using Npgsql;

namespace Ivony.Data.PostgreSQL
{
  /// <summary>
  /// 提供 PostgreSQL 数据库访问支持
  /// </summary>
  public static class PostgreSql
  {
    /// <summary>
    /// 从配置文件中读取连接字符串并创建 PostgreSQL 数据库访问器
    /// </summary>
    /// <param name="name">连接字符串配置名称</param>
    /// <param name="configuration">PostgreSQL 配置</param>
    /// <returns>PostgreSQL 数据库访问器</returns>
    public static NpgsqlDbExecutor FromConfiguration( string name, NpgsqlDbConfiguration configuration = null )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return Connect( setting.ConnectionString, configuration );
    }

    /// <summary>
    /// 通过指定的连接字符串并创建 PostgreSQL 数据库访问器
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">PostgreSQL配置</param>
    /// <returns>PostgreSQL 数据库访问器</returns>
    public static NpgsqlDbExecutor Connect( string connectionString, NpgsqlDbConfiguration configuration = null )
    {
      return new NpgsqlDbExecutor( connectionString, configuration ?? DefaultConfiguration );
    }

    /// <summary>
    /// 通过指定的连接字符串构建器创建 PostgreSQL 数据库访问器
    /// </summary>
    /// <param name="builder">连接字符串构建器</param>
    /// <param name="configuration">PostgreSQL配置</param>
    /// <returns>PostgreSQL 数据库访问器</returns>
    public static NpgsqlDbExecutor Connect( NpgsqlConnectionStringBuilder builder, NpgsqlDbConfiguration configuration = null )
    {
      return Connect( builder.ConnectionString, configuration );
    }

    /// <summary>
    /// 通过指定的用户名和密码登陆 PostgreSQL 数据库，以创建 PostgreSQL 数据库访问器
    /// </summary>
    /// <param name="host">数据库服务器实例名称</param>
    /// <param name="database">数据库名称</param>
    /// <param name="port">数据库端口</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">PostgreSQL数据库配置</param>
    /// <returns>PostgreSQL 数据库访问器</returns>
    public static NpgsqlDbExecutor Connect( string host, string database, int port, string userID, string password, bool pooling = true, NpgsqlDbConfiguration configuration = null )
    {
      var builder = new NpgsqlConnectionStringBuilder
      {
        Host = host,
        Database = database,
        Port = port,
        IntegratedSecurity = false,
        UserName = userID,
        Password = password,
        Pooling = pooling
      };

      return Connect( builder.ConnectionString, configuration );
    }

    /// <summary>
    /// 通过集成身份验证登陆 PostgreSQL 数据库，以创建 PostgreSQL 数据库访问器
    /// </summary>
    /// <param name="host">数据库服务器实例名称</param>
    /// <param name="database">数据库名称</param>
    /// <param name="port">数据库端口</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">PostgreSQL数据库配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static NpgsqlDbExecutor Connect( string host, string database, int port = 5432, bool pooling = true, NpgsqlDbConfiguration configuration = null )
    {
      var builder = new NpgsqlConnectionStringBuilder
      {
        Host = host,
        Port = port,
        Database = database,
        IntegratedSecurity = true,
        Pooling = pooling
      };

      return Connect( builder.ConnectionString, configuration );
    }

    public static NpgsqlDbExecutor Connect( string host, string database = null, int port = 5432, bool? integratedSecurity = null, string userID = null, string password = null, string attachDbFilename = null, bool? pooling = null, int? maxPoolSize = null, int? minPoolSize = null, NpgsqlDbConfiguration configuration = null )
    {
      var builder = new NpgsqlConnectionStringBuilder
      {
        Host = host
      };

      if ( userID != null )
        builder.UserName = userID;

      if ( password != null )
        builder.Password = password;

      if ( integratedSecurity != null )
        builder.IntegratedSecurity = integratedSecurity.Value;

      if ( database != null )
        builder.Database = database;

      if ( port > 0 )
        builder.Port = port;

      if ( pooling != null )
        builder.Pooling = pooling.Value;

      if ( maxPoolSize != null )
        builder.MaxPoolSize = maxPoolSize.Value;

      if ( minPoolSize != null )
        builder.MaxPoolSize = minPoolSize.Value;

      return Connect( builder.ConnectionString, configuration );
    }

    private static NpgsqlDbConfiguration _defaultConfiguration = new NpgsqlDbConfiguration();

    /// <summary>
    /// 获取或设置默认配置
    /// </summary>
    public static NpgsqlDbConfiguration DefaultConfiguration
    {
      get { return _defaultConfiguration; }
      set
      {
        _defaultConfiguration = value ?? new NpgsqlDbConfiguration();
      }
    }
  }
}
