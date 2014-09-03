using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义数据库查询执行上下文
  /// </summary>
  public interface IDbExecuteContext
  {

    /// <summary>
    /// 执行查询并获得查询结果
    /// </summary>
    /// <returns>数据库查询结果</returns>
    IDbResult Execute();

  }



  /// <summary>
  /// 定义数据库异步查询执行上下文
  /// </summary>
  public interface IAsyncDbExecuteContext
  {
    /// <summary>
    /// 异步执行查询，并获得查询结果
    /// </summary>
    Task<IAsyncDbResult> ExecuteAsync( CancellationToken token = default( CancellationToken ) );
  }
}
