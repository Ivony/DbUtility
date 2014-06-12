using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Common;
using System.IO;

namespace Ivony.Data
{

  /// <summary>
  /// 提供 SQL Server 数据库访问支持
  /// </summary>
  public static class SqlServer
  {


    /// <summary>
    /// 从配置文件中读取连接字符串并创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="name">连接字符串配置名称</param>
    /// <param name="configuration">SQL Server配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility FromConfiguration( string name, SqlDbConfiguration configuration = null )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return Create( setting.ConnectionString, configuration );
    }


    /// <summary>
    /// 通过指定的连接字符串并创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">SQL Server配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility Create( string connectionString, SqlDbConfiguration configuration = null )
    {
      return new SqlDbUtility( connectionString, configuration ?? DefaultConfiguration );
    }



    /// <summary>
    /// 通过指定的连接字符串构建器创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="buildler">连接字符串构建器</param>
    /// <param name="configuration">SQL Server配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility Create( SqlConnectionStringBuilder buildler, SqlDbConfiguration configuration = null )
    {
      return Create( buildler.ConnectionString, configuration );
    }



    /// <summary>
    /// 通过指定的用户名和密码登陆SQL Server数据库，以创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="dataSource">数据库服务器实例名称</param>
    /// <param name="initialCatalog">数据库名称</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">SQL Server数据库配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility Create( string dataSource, string initialCatalog, string userID, string password, bool pooling = true, SqlDbConfiguration configuration = null )
    {
      var builder = new SqlConnectionStringBuilder()
      {
        DataSource = dataSource,
        InitialCatalog = initialCatalog,
        IntegratedSecurity = false,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return Create( builder.ConnectionString, configuration );
    }


    /// <summary>
    /// 通过集成身份验证登陆SQL Server数据库，以创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="dataSource">数据库服务器实例名称</param>
    /// <param name="initialCatalog">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">SQL Server数据库配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility Create( string dataSource, string initialCatalog, bool pooling = true, SqlDbConfiguration configuration = null )
    {
      var builder = new SqlConnectionStringBuilder()
      {
        DataSource = dataSource,
        InitialCatalog = initialCatalog,
        IntegratedSecurity = true,
        Pooling = pooling
      };

      return Create( builder.ConnectionString, configuration );
    }





    /// <summary>
    /// 通过连接 SQL Server Express 2008 LocalDB，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="instanceName">SQL Server LocalDB 实例名称</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility FromLocalDB2008( string database, SqlDbConfiguration configuration = null )
    {

      return FromSqlExpress( database, "v11.0", configuration );

    }

    /// <summary>
    /// 通过连接 SQL Server Express 2012 LocalDB，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="instanceName">SQL Server LocalDB 实例名称</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility FromLocalDB2012( string database, SqlDbConfiguration configuration = null )
    {

      return FromSqlExpress( database, "MSSQLLocalDB", configuration );

    }


    /// <summary>
    /// 通过连接 SQL Server Express，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility FromSqlExpress( string database, SqlDbConfiguration configuration = null )
    {
      return FromSqlExpress( database, "SQLExpress", configuration );
    }

    /// <summary>
    /// 通过连接 SQL Server Express 指定实例，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="instanceName">SQL Server 实例名称</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility FromSqlExpress( string database, string instanceName, SqlDbConfiguration configuration = null )
    {
      var builder = new SqlConnectionStringBuilder()
      {
        DataSource = @"(local)\" + instanceName,
        IntegratedSecurity = true,
      };


      if ( database.IndexOfAny( Path.GetInvalidPathChars() ) == -1 && Path.IsPathRooted( database ) )
        builder.AttachDBFilename = database;

      else
        builder.InitialCatalog = database;


      return Create( builder.ConnectionString, configuration );
    }



    public static SqlDbUtility Create( string dataSource, string initialCatalog = null, bool? integratedSecurity = null, string userID = null, string password = null, string attachDbFilename = null, bool? pooling = null, int? maxPoolSize = null, int? minPoolSize = null, SqlDbConfiguration configuration = null )
    {
      var builder = new SqlConnectionStringBuilder();

      builder.DataSource = dataSource;
      if ( userID != null )
        builder.UserID = userID;

      if ( password != null )
        builder.Password = password;

      if ( integratedSecurity != null )
        builder.IntegratedSecurity = integratedSecurity.Value;

      if ( initialCatalog != null )
        builder.InitialCatalog = initialCatalog;

      if ( pooling != null )
        builder.Pooling = pooling.Value;

      if ( maxPoolSize != null )
        builder.MaxPoolSize = maxPoolSize.Value;

      if ( minPoolSize != null )
        builder.MaxPoolSize = minPoolSize.Value;

      if ( attachDbFilename != null )
        builder.AttachDBFilename = attachDbFilename;

      return Create( builder.ConnectionString, configuration );



    }



    static SqlServer()
    {
      DefaultConfiguration = new SqlDbConfiguration();
    }


    public static SqlDbConfiguration DefaultConfiguration
    {
      get;
      set;
    }

  }
}
