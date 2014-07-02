using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{
  /// <summary>
  /// 定义用于 SQL Server 数据库的配置项
  /// </summary>
  public class SqlDbConfiguration : DbConfiguration
  {

    /// <summary>
    /// 创建一个默认的 SQL Server 数据库配置
    /// </summary>
    public SqlDbConfiguration() { }

    /// <summary>
    /// 从现有的 SQL Server 数据库配置中创建一个数据库配置
    /// </summary>
    /// <param name="configuration">现有的数据库配置</param>
    public SqlDbConfiguration( SqlDbConfiguration configuration ) : base( configuration ) { }

    /// <summary>
    /// 从现有的通用数据库配置文件中创建一个 SQL Server 数据库配置
    /// </summary>
    /// <param name="configuration">现有的数据库配置</param>
    public SqlDbConfiguration( DbConfiguration configuration ) : base( configuration ) { }


    /// <summary>
    /// 获取或设置SQL Server Express LocalDB 默认实例名（此配置为全局配置）
    /// </summary>
    /// <remarks>
    /// 默认值为 v11.0，即 SQL Server Express LocalDB 2012，若需要连接 SQL Server Express LocalDB 2014 ，请使用 MSSQLLocalDB
    /// </remarks>
    public string LocalDBInstanceName
    {
      get { return SqlServer.LocalDBInstanceName; }
      set { SqlServer.LocalDBInstanceName = value; }
    }

    /// <summary>
    /// 获取或设置SQL Server Express 默认实例名（此配置为全局配置）
    /// </summary>
    public string ExpressInstanceName
    {
      get { return SqlServer.ExpressInstanceName; }
      set { SqlServer.ExpressInstanceName = value; }
    }



  }
}
