using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public sealed class DbExecutableQuery<T> : IDbExecutableQuery, IDbQueryContainer where T : IDbQuery
  {


    /// <summary>
    /// 创建 DbExecutableQuery 对象
    /// </summary>
    /// <param name="executor">可以执行查询的执行器</param>
    /// <param name="query">要执行的查询</param>
    public DbExecutableQuery( IDbExecutor<T> executor, T query )
    {
      Executor = executor;
      AsyncExecutor = executor as IAsyncDbExecutor<T>;
      Query = query;
    }

    /// <summary>
    /// 创建 DbExecutableQuery 对象
    /// </summary>
    /// <param name="executor">可以执行查询的执行器</param>
    /// <param name="query">要执行的查询</param>
    public DbExecutableQuery( IAsyncDbExecutor<T> executor, T query )
    {
      Executor = AsyncExecutor = executor;
      Query = query;
    }



    /// <summary>
    /// 用于同步执行查询的执行器
    /// </summary>
    public IDbExecutor<T> Executor { get; private set; }


    /// <summary>
    /// 用于异步执行查询的查询器
    /// </summary>
    public IAsyncDbExecutor<T> AsyncExecutor { get; private set; }


    /// <summary>
    /// 要执行的查询对象
    /// </summary>
    public T Query { get; private set; }




    /// <summary>
    /// 同步执行查询
    /// </summary>
    /// <returns>查询执行上下文</returns>
    public IDbExecuteContext Execute()
    {
      return Executor.Execute( Query );
    }



    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <returns>查询执行上下文</returns>
    public Task<IDbExecuteContext> ExecuteAsync()
    {

      if ( AsyncExecutor == null )
        return new Task<IDbExecuteContext>( Execute );

      return AsyncExecutor.ExecuteAsync( Query );
    }


    /// <summary>
    /// 定义隐式类型转换，将 DbExecutableQuery 转换为实际的查询对象
    /// </summary>
    /// <param name="executable"></param>
    /// <returns></returns>
    public static implicit operator T( DbExecutableQuery<T> executable )
    {
      return executable.Query;
    }


    IDbQuery IDbQueryContainer.Query
    {
      get { return Query; }
    }
  }
}
