using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义对数据库查询进行追踪的记录器
  /// </summary>
  public interface IDbTracing
  {

    /// <summary>
    /// 通知追踪记录器正在执行查询
    /// </summary>
    /// <param name="commandObject">查询命令对象</param>
    void OnExecuting( object commandObject );

    /// <summary>
    /// 通知追踪记录器正在加载数据。
    /// </summary>
    /// <param name="context">查询执行上下文</param>
    void OnLoadingData( IDbResult context );

    /// <summary>
    /// 通知追踪记录器查询已经全部完成。
    /// </summary>
    void OnComplete();

    /// <summary>
    /// 通知追踪记录器查询数据库时出现了异常。
    /// </summary>
    /// <param name="exception">异常信息</param>
    void OnException( Exception exception );

  }
}
