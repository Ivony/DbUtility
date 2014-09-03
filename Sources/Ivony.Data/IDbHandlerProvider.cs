using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义数据库访问工具提供程序
  /// </summary>
  /// <typeparam name="T">数据库访问工具类型</typeparam>
  public interface IDbHandlerProvider<out T>
  {

    /// <summary>
    /// 获取数据库数据库访问工具
    /// </summary>
    T GetDbHandler();
  }
}
