using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  public static class DbTracingHelper
  {

    /// <summary>
    /// 尝试创建 IDbTracing 对象
    /// </summary>
    /// <typeparam name="TQuery">即将执行的查询的类型</typeparam>
    /// <param name="executor">查询执行器</param>
    /// <param name="query">即将执行的查询对象</param>
    /// <returns>追踪该查询执行过程的 IDbTracing 对象</returns>
    public static IDbTracing TryCreateTracing<TQuery>( this IDbTraceService traceService, object dbHandler, TQuery query )
    {

      if ( traceService == null )
        return null;

      IDbTracing tracing;
      try
      {
        tracing = traceService.CreateTracing( dbHandler, query );
      }
      catch
      {
        return null;
      }

      return tracing;
    }


    /// <summary>
    /// 尝试执行查询追踪器的一个追踪方法，此方法会自动判断追踪器是否存在以及对调用中出现的异常进行异常屏蔽。
    /// </summary>
    /// <param name="tracing">查询追踪器，如果有的话</param>
    /// <param name="action">要执行的追踪操作</param>
    public static void TryExecuteTracing( this IDbTracing tracing, Action<IDbTracing> action )
    {
      if ( tracing == null )
        return;

      try
      {
        action( tracing );
      }
      catch
      {

      }
    }



  }
}
