using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义通用的数据库配置项
  /// </summary>
  public class DbConfiguration
  {


    /// <summary>
    /// 创建默认的通用数据库配置
    /// </summary>
    public DbConfiguration() { }


    /// <summary>
    /// 从现有配置中创建通用数据库配置
    /// </summary>
    public DbConfiguration( DbConfiguration configuration )
    {

      TraceService = configuration.TraceService;
    
    }


    /// <summary>
    /// 获取或设置在执行数据库查询过程中提供追踪服务的对象
    /// </summary>
    public IDbTraceService TraceService
    {
      get;
      set;
    }

  }
}
