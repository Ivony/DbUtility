using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  public static class DbTracingHelper
  {




   /// <summary>
    /// 尝试执行查询，并将执行过程使用指定的追踪器记录
    /// </summary>
    /// <typeparam name="TResult">数据库查询结果类型</typeparam>
    /// <typeparam name="TCommand">查询命令对象类型</typeparam>
    /// <param name="tracing">追踪此次查询的追踪器</param>
    /// <param name="commandObject">数据库查询命令对象</param>
    /// <param name="executeMethod">执行数据库查询的方法</param>
    /// <returns>数据库查询执行结果</returns>
    public static TResult TryExecuteWithTracing<TResult, TCommand>( this IDbTracing tracing, TCommand commandObject, Func<TCommand, IDbTracing, TResult> executeMethod ) where TResult : IDbResult
    {
      if ( tracing == null )
        return executeMethod( commandObject, null );

      try
      {
        tracing.OnExecuting( commandObject );
      }
      catch { }



      TResult result;

      try
      {
        result = executeMethod( commandObject, tracing );
      }
      catch ( Exception exception )
      {
        try
        {
          tracing.OnException( exception );
        }
        catch { }


        throw;
      }



      try
      {
        tracing.OnLoadingData( result );
      }
      catch { }


      return result;
    }



    /// <summary>
    /// 尝试异步执行查询，并将执行过程使用指定的追踪器记录
    /// </summary>
    /// <typeparam name="TResult">数据库查询结果类型</typeparam>
    /// <typeparam name="TCommand">查询命令对象类型</typeparam>
    /// <param name="tracing">追踪此次查询的追踪器</param>
    /// <param name="commandObject">数据库查询命令对象</param>
    /// <param name="executeMethod">异步执行数据库查询的方法</param>
    /// <param name="token">取消标识</param>
    /// <returns></returns>
    public async static Task<TResult> TryExecuteAsyncWithTracing<TResult, TCommand>( this IDbTracing tracing, TCommand commandObject, Func<TCommand, IDbTracing, CancellationToken, Task<TResult>> executeMethod, CancellationToken token = default( CancellationToken ) ) where TResult : IDbResult
    {
      if ( tracing == null )
        return await executeMethod( commandObject, null, token );

      try
      {
        tracing.OnExecuting( commandObject );
      }
      catch { }



      TResult result;

      try
      {
        result = await executeMethod( commandObject, tracing, token );
      }
      catch ( Exception exception )
      {
        try
        {
          tracing.OnException( exception );
        }
        catch { }


        throw;
      }



      try
      {
        tracing.OnLoadingData( result );
      }
      catch { }


      return result;
    }




  }
}
