using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 查询执行上下文
  /// </summary>
  public interface IDbExecuteContext : IDisposable
  {

    /// <summary>
    /// 用于获取查询结果的 DataReader 对象
    /// </summary>
    IDataReader DataReader { get; }

    /// <summary>
    /// 销毁查询执行上下文
    /// </summary>
    void Dispose();
  }
}
