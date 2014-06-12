using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 空白的追踪服务，对于任何查询，均不进行追踪
  /// </summary>
  public class BlankTraceService : IDbTraceService
  {
    public IDbTracing CreateTracing<TQuery>( IDbExecutor<TQuery> executor, TQuery query ) where TQuery : IDbQuery
    {
      return null;
    }

    private static BlankTraceService _instance = new BlankTraceService();

    public static BlankTraceService Instance
    {
      get { return _instance; }
    }

  }
}
