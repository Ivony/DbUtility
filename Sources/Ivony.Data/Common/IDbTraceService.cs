using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 定义对数据库查询过程进行追踪记录的服务
  /// </summary>
  public interface IDbTraceService
  {

    /// <summary>
    /// 当开始执行一个查询时，框架调用此方法创建一个追踪器，关于这个查询的所有消息，将记录到这个追踪器
    /// </summary>
    /// <typeparam name="TQuery">正在执行的查询的类型</typeparam>
    /// <param name="executor">正在执行查询的查询执行器</param>
    /// <param name="query">正在执行的查询对象</param>
    /// <returns>实现类应返回一个追踪器，用于记录日志信息。</returns>
    IDbTracing CreateTracing<TQuery>( IDbExecutor<TQuery> executor, TQuery query ) where TQuery : IDbQuery;
  }
}
