using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 定义可以同步执行某类型查询的数据库查询执行程序所需要实现的接口
  /// </summary>
  /// <typeparam name="T">查询类型</typeparam>
  public interface IDbExecutor<T> where T : IDbQuery
  {

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="query">查询对象</param>
    /// <returns>查询执行上下文</returns>
    IDbExecuteContext Execute( T query );

  }


  /// <summary>
  /// 定义可以异步执行某类型查询的数据库查询执行程序所需要实现的接口
  /// </summary>
  /// <typeparam name="T">查询类型</typeparam>
  public interface IAsyncDbExecutor<T> : IDbExecutor<T> where T : IDbQuery
  {

    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询执行上下文</returns>
    Task<IDbExecuteContext> ExecuteAsync( T query, CancellationToken token );


  }
}
