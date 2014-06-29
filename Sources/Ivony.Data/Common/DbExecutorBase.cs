using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 辅助实现数据库查询器的基类
  /// </summary>
  public abstract class DbExecutorBase
  {


    /// <summary>
    /// 初始化 DbExecuterBase 类型
    /// </summary>
    protected DbExecutorBase()
    {
    }


    /// <summary>
    /// 获取追踪数据库查询过程的追踪服务
    /// </summary>
    protected virtual IDbTraceService TraceService
    {
      get { return null; }
    }



    /// <summary>
    /// 尝试创建 IDbTracing 对象
    /// </summary>
    /// <typeparam name="TQuery">即将执行的查询的类型</typeparam>
    /// <param name="executor">查询执行器</param>
    /// <param name="query">即将执行的查询对象</param>
    /// <returns>追踪该查询执行过程的 IDbTracing 对象</returns>
    protected IDbTracing TryCreateTracing<TQuery>( IDbExecutor<TQuery> executor, TQuery query ) where TQuery : IDbQuery
    {

      if ( TraceService == null )
        return null;

      IDbTracing tracing;
      try
      {
        tracing = TraceService.CreateTracing( executor, query );
      }
      catch
      {
        return null;
      }

      return tracing;
    }




    /// <summary>
    /// 尝试创建 IDbTracing 对象
    /// </summary>
    /// <typeparam name="TQuery">即将执行的查询的类型</typeparam>
    /// <param name="query">即将执行的查询对象</param>
    /// <returns>追踪该查询执行过程的 IDbTracing 对象</returns>
    protected IDbTracing TryCreateTracing<TQuery>( TQuery query ) where TQuery : IDbQuery
    {

      return TryCreateTracing( (IDbExecutor<TQuery>) this, query );

    }



    /// <summary>
    /// 尝试执行查询追踪器的一个追踪方法，此方法会自动判断追踪器是否存在以及对调用中出现的异常进行异常屏蔽。
    /// </summary>
    /// <param name="tracing">查询追踪器，如果有的话</param>
    /// <param name="action">要执行的追踪操作</param>
    protected void TryExecuteTracing( IDbTracing tracing, Action<IDbTracing> action )
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




    /// <summary>
    /// 辅助实现执行查询
    /// </summary>
    /// <typeparam name="TQuery">查询类型</typeparam>
    /// <typeparam name="TCommand">命令对象类型</typeparam>
    /// <typeparam name="TContext">查询执行上下文</typeparam>
    /// <param name="query">查询对象</param>
    /// <param name="commandCreator">创建命令对象的方法</param>
    /// <param name="executor">执行命令的方法</param>
    /// <returns>查询执行上下文</returns>
    protected TContext ExecuteQuery<TQuery, TCommand, TContext>( TQuery query, Func<TQuery, TCommand> commandCreator, Func<TQuery, TCommand, IDbTracing, TContext> executor )
      where TQuery : IDbQuery
      where TContext : IDbExecuteContext
    {
      var tracing = TryCreateTracing<TQuery>( query );
      var command = commandCreator( query );


      TContext context;
      TryTraceOnExecuting( tracing, command );
      try
      {
        context = executor( query, command, tracing );
      }
      catch ( DbException exception )
      {
        TryTraceException( tracing, exception );
        throw;
      }
      TryTraceLoadingData( tracing, context );

      return context;
    }



    /// <summary>
    /// 辅助实现执行查询
    /// </summary>
    /// <typeparam name="TQuery">查询类型</typeparam>
    /// <typeparam name="TCommand">命令对象类型</typeparam>
    /// <typeparam name="TContext">查询执行上下文</typeparam>
    /// <param name="query">查询对象</param>
    /// <param name="asyncCommandCreator">异步创建命令对象的方法</param>
    /// <param name="asyncExecutor">异步执行命令的方法</param>
    /// <param name="token">取消标识</param>
    /// <returns>查询执行上下文</returns>
    protected async Task<TContext> ExecuteQuery<TQuery, TCommand, TContext>( TQuery query, Func<TQuery, CancellationToken, Task<TCommand>> asyncCommandCreator, Func<TQuery, TCommand, IDbTracing, CancellationToken, Task<TContext>> asyncExecutor, CancellationToken token = default(CancellationToken) )
      where TQuery : IDbQuery
      where TContext : IAsyncDbExecuteContext
    {
      var tracing = TryCreateTracing<TQuery>( query );
      var command = await asyncCommandCreator( query, token );


      TContext context;
      TryTraceOnExecuting( tracing, command );
      try
      {
        context = await asyncExecutor( query, command, tracing, token );
      }
      catch ( DbException exception )
      {
        TryTraceException( tracing, exception );
        throw;
      }
      TryTraceLoadingData( tracing, context );

      return context;
    }


    /// <summary>
    /// 辅助实现执行查询
    /// </summary>
    /// <typeparam name="TQuery">查询类型</typeparam>
    /// <typeparam name="TCommand">命令对象类型</typeparam>
    /// <typeparam name="TContext">查询执行上下文</typeparam>
    /// <param name="query">查询对象</param>
    /// <param name="commandCreator">创建命令对象的方法</param>
    /// <param name="asyncExecutor">异步执行命令的方法</param>
    /// <param name="token">取消标识</param>
    /// <returns>查询执行上下文</returns>
    protected async Task<TContext> ExecuteQuery<TQuery, TCommand, TContext>( TQuery query, Func<TQuery, TCommand> commandCreator, Func<TQuery, TCommand, IDbTracing, CancellationToken, Task<TContext>> asyncExecutor, CancellationToken token = default(CancellationToken) )
      where TQuery : IDbQuery
      where TContext : IAsyncDbExecuteContext
    {
      var tracing = TryCreateTracing<TQuery>( query );
      var command = commandCreator( query );


      TContext context;
      TryTraceOnExecuting( tracing, command );
      try
      {
        context = await asyncExecutor( query, command, tracing, token );
      }
      catch ( DbException exception )
      {
        TryTraceException( tracing, exception );
        throw;
      }
      TryTraceLoadingData( tracing, context );

      return context;
    }


    /// <summary>
    /// 尝试追踪执行查询事件
    /// </summary>
    /// <param name="tracing">查询追踪器</param>
    /// <param name="commandObject">查询命令对象</param>
    protected void TryTraceOnExecuting( IDbTracing tracing, object commandObject )
    {
      if ( tracing == null )
        return;

      try { tracing.OnExecuting( commandObject ); }
      catch { }
    }

    /// <summary>
    /// 尝试追踪加载数据事件
    /// </summary>
    /// <param name="tracing">查询追踪器</param>
    /// <param name="context">查询执行上下文</param>
    protected void TryTraceLoadingData( IDbTracing tracing, IDbExecuteContext context )
    {
      if ( tracing == null )
        return;

      try { tracing.OnLoadingData( context ); }
      catch { }
    }

    /// <summary>
    /// 尝试追踪一个查询过程中发生的异常
    /// </summary>
    /// <param name="tracing">查询追踪器</param>
    /// <param name="exception">要追踪的异常</param>
    protected void TryTraceException( IDbTracing tracing, Exception exception )
    {
      if ( tracing == null )
        return;

      try { tracing.OnException( exception ); }
      catch { }
    }


  }
}
