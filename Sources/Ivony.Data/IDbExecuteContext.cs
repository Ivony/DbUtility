using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义数据库执行上下文
  /// </summary>
  public interface IDbExecuteContext
  {

    IDbResult Execute();

  }


  public interface IAsyncDbExecuteContext
  {
    Task<IAsyncDbResult> ExecuteAsync( CancellationToken token = default( CancellationToken ) );
  }
}
