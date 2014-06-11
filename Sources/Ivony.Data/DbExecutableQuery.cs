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
  /// IDbExecutableQuery 的一个标准实现
  /// </summary>
  /// <typeparam name="T">查询对象的类型</typeparam>
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
      Query = query;
    }

    /// <summary>
    /// 用于同步执行查询的执行器
    /// </summary>
    public IDbExecutor<T> Executor { get; private set; }


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


  /// <summary>
  /// IAsyncDbExecutableQuery 的标准实现
  /// </summary>
  /// <typeparam name="T">查询类型</typeparam>
  public sealed class AsyncDbExecutableQuery<T> : IAsyncDbExecutableQuery, IDbQueryContainer where T : IDbQuery
  {

    /// <summary>
    /// 创建 AsyncDbExecutableQuery 对象
    /// </summary>
    /// <param name="executor">可以执行查询的执行器</param>
    /// <param name="query">要执行的查询</param>
    public AsyncDbExecutableQuery( IAsyncDbExecutor<T> executor, T query )
    {
      Executor = executor;
      Query = query;
    }



    /// <summary>
    /// 用于异步执行查询的查询器
    /// </summary>
    public IAsyncDbExecutor<T> Executor { get; private set; }


    /// <summary>
    /// 要执行的查询对象
    /// </summary>
    public T Query { get; private set; }


    /// <summary>
    /// 定义隐式类型转换，将 DbExecutableQuery 转换为实际的查询对象
    /// </summary>
    /// <param name="executable">可异步执行的查询</param>
    /// <returns></returns>
    public static implicit operator T( AsyncDbExecutableQuery<T> executable )
    {
      return executable.Query;
    }


    IDbQuery IDbQueryContainer.Query
    {
      get { return Query; }
    }



    /// <summary>
    /// 同步执行查询
    /// </summary>
    /// <returns>查询执行上下文</returns>
    [System.ComponentModel.EditorBrowsable( System.ComponentModel.EditorBrowsableState.Advanced )]
    public IDbExecuteContext Execute()
    {
      return Executor.Execute( Query );
    }

    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <returns>异步查询执行上下文</returns>
    [System.ComponentModel.EditorBrowsable( System.ComponentModel.EditorBrowsableState.Advanced )]
    public Task<IAsyncDbExecuteContext> ExecuteAsync( CancellationToken token = default( CancellationToken ) )
    {
      return Executor.ExecuteAsync( Query, token );
    }
  }
}
