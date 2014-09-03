using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 辅助实现数据库查询器的基类
  /// </summary>
  public abstract class DbHandlerBase
  {



    /// <summary>
    /// 初始化 DbExecuterBase 类型
    /// </summary>
    /// <param name="configuration">当前要使用的数据库配置</param>
    protected DbHandlerBase()
    {
    }


    /// <summary>
    /// 获取在追踪数据库查询过程的追踪服务
    /// </summary>
    protected virtual IDbTraceService TraceService { get { return null; } }






  }
}
