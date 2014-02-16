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
    /// 获取查询结果的 DataReader 对象
    /// </summary>
    IDataReader DataReader { get; }


    /// <summary>
    /// 获取用于确保同步查询过程的同步对象
    /// </summary>
    object SyncRoot { get; }

  }
}
