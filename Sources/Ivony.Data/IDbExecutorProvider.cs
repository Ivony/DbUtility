using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义数据库执行器提供程序
  /// </summary>
  /// <typeparam name="TExecutor"></typeparam>
  public interface IDbExecutorProvider<TExecutor>
  {

    /// <summary>
    /// 获取数据库查询执行器
    /// </summary>
    TExecutor GetDbExecutor();
  }
}
