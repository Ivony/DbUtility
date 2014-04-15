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
    /// 当执行某个查询之前将调用此方法通知追踪记录器
    /// </summary>
    /// <typeparam name="TQuery">即将要执行的查询类型</typeparam>
    /// <param name="context">执行查询前的数据上下文</param>
    /// <param name="moment">引发事件的时刻</param>
    void OnQueryExecuting<TQuery>( DbQueryExecutingContext<TQuery> context, DateTime moment ) where TQuery : IDbQuery;


    /// <summary>
    /// 当执行某个查询之前将调用此方法通知追踪记录器
    /// </summary>
    /// <typeparam name="TQuery">即将要执行的查询类型</typeparam>
    /// <param name="context">执行查询后的数据上下文</param>
    /// <param name="moment">引发事件的时刻</param>
    void OnQueryExecuted<TQuery>( DbQueryExecutedContext<TQuery> context, DateTime moment ) where TQuery : IDbQuery;



    void OnResultConstructing( IDbQuery query, DateTime moment );

    void OnResultConstructed( IDbQuery query, DateTime moment );


  }

}
