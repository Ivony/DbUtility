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
    /// 获取或设置执行查询超时时间
    /// </summary>
    public TimeSpan? CommandTimeout { get; set; }


    /// <summary>
    /// 获取或设置是否立即执行查询
    /// </summary>
    public bool ImmediateExecution { get; set; }
  }
}
