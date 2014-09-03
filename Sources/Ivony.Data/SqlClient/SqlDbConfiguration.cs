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
  public sealed class SqlDbConfiguration
  {

    /// <summary>
    /// 创建一个默认的 SQL Server 数据库配置
    /// </summary>
    public SqlDbConfiguration() { }

    /// <summary>
    /// 从现有的 SQL Server 数据库配置中创建一个数据库配置
    /// </summary>
    /// <param name="configuration">现有的数据库配置</param>
    public SqlDbConfiguration( SqlDbConfiguration configuration )
    {
      TraceService = configuration.TraceService;
      QueryTimeout = configuration.QueryTimeout;
      ImmediateExecution = configuration.ImmediateExecution;
    }



    /// <summary>
    /// 获取或设置追踪查询过程的追踪服务
    /// </summary>
    public IDbTraceService TraceService { get; set; }


    /// <summary>
    /// 获取或设置执行查询超时时间
    /// </summary>
    public TimeSpan? QueryTimeout { get; set; }


    /// <summary>
    /// 获取或设置是否立即执行查询
    /// </summary>
    public bool ImmediateExecution { get; set; }
  }
}
