using Ivony.Data.Common;
using Ivony.Data.MySqlClient;
using Ivony.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 提供 MySql 数据库支持
  /// </summary>
  public static class MySql
  {

    /// <summary>
    /// 从配置文件中读取连接字符串并创建 MySql 数据库访问器
    /// </summary>
    /// <param name="name">连接字符串配置名称</param>
    /// <param name="configuration">MySql 配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbExecutor FromConfiguration( string name, MySqlDbConfiguration configuration = null )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return Create( setting.ConnectionString, configuration );
    }


    /// <summary>
    /// 通过指定的连接字符串并创建 MySql 数据库访问器
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">MySql 配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbExecutor Create( string connectionString, MySqlDbConfiguration configuration = null )
    {
      return new MySqlDbExecutor( connectionString, configuration ?? DefaultConfiguration );
    }



    /// <summary>
    /// 通过指定的连接字符串构建器创建 MySql 数据库访问器
    /// </summary>
    /// <param name="builder">连接字符串构建器</param>
    /// <param name="configuration">MySql 配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbExecutor Create( MySqlConnectionStringBuilder builder, MySqlDbConfiguration configuration = null )
    {
      return Create( builder.ConnectionString, configuration );
    }



    /// <summary>
    /// 通过指定的用户名和密码登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="database">数据库名称</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbExecutor Create( string server, string database, string userID, string password, bool pooling = true, MySqlDbConfiguration configuration = null )
    {
      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        IntegratedSecurity = false,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return Create( builder.ConnectionString, configuration );
    }


    /// <summary>
    /// 通过指定的用户名和密码登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="port">数据库服务器端口</param>
    /// <param name="database">数据库名称</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbExecutor Create( string server, uint port, string database, string userID, string password, bool pooling = true, MySqlDbConfiguration configuration = null )
    {
      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        IntegratedSecurity = false,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return Create( builder.ConnectionString, configuration );
    }


    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器实例名称</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbExecutor Create( string server, string database, bool pooling = true, MySqlDbConfiguration configuration = null )
    {

      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        IntegratedSecurity = true,
        Pooling = pooling
      };

      return Create( builder.ConnectionString, configuration );
    }




    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="dataSource">数据库服务器地址</param>
    /// <param name="port">数据库服务器端口</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbExecutor Create( string server, uint port, string database, bool pooling = true, MySqlDbConfiguration configuration = null )
    {

      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Port = port,
        Database = database,
        IntegratedSecurity = true,
        Pooling = pooling
      };

      return Create( builder.ConnectionString, configuration );
    }





    static MySql()
    {
      DefaultConfiguration = new MySqlDbConfiguration();
    }


    /// <summary>
    /// 获取或设置默认配置
    /// </summary>
    public static MySqlDbConfiguration DefaultConfiguration
    {
      get;
      set;
    }


    /// <summary>
    /// 获取或设置 MySql 全局设置
    /// </summary>
    public static MySqlConfiguration MySqlGlobalSettings
    {
      get { return MySqlConfiguration.Settings; }
    }


  }
}
