using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  
  /// <summary>
  /// 提供 SQL Server Express 支持
  /// </summary>
  public static class SqlServerExpress
  {
    /// <summary>
    /// 通过连接 SQL Server Express 2012 LocalDB，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="instanceName">SQL Server LocalDB 实例名称</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility LocalDB2012( string database, SqlDbConfiguration configuration = null )
    {

      return Create( database, "v11.0", configuration );

    }

    /// <summary>
    /// 通过连接 SQL Server Express 2014 LocalDB，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="instanceName">SQL Server LocalDB 实例名称</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility LocalDB2014( string database, SqlDbConfiguration configuration = null )
    {

      return Create( database, "MSSQLLocalDB", configuration );

    }


    /// <summary>
    /// 通过连接 SQL Server Express 默认实例，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbUtility Create( string database, SqlDbConfiguration configuration = null )
    {
      return Create( database, "SQLExpress", configuration );
    }


    /// <summary>
    /// 通过连接 SQL Server Express 指定实例，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="instanceName">SQL Server 实例名称</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    private static SqlDbUtility Create( string database, string instanceName, SqlDbConfiguration configuration = null )
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


      return SqlServer.Create( builder.ConnectionString, configuration );
    }
  }
}
