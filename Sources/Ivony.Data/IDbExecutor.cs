using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 定义可以同步执行某类型查询的数据库查询执行程序所需要实现的接口
  /// </summary>
  /// <typeparam name="T">查询类型</typeparam>
  public interface IDbExecutor<T> where T : IDbQuery
  {

    IDbExecuteContext Execute( T query );

  }


  /// <summary>
  /// 定义可以异步执行某类型查询的数据库查询执行程序所需要实现的接口
  /// </summary>
  /// <typeparam name="T">查询类型</typeparam>
  public interface IAsyncDbExecutor<T> : IDbExecutor<T> where T : IDbQuery
  {

    Task<IDbExecuteContext> ExecuteAsync( T query );


  }
}
