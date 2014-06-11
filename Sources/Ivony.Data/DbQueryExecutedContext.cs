using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义执行查询后的上下文信息
  /// </summary>
  /// <typeparam name="TQuery">查询对象类型</typeparam>
  public class DbQueryExecutedContext<TQuery> where TQuery : IDbQuery
  {

    public DbQueryExecutedContext( IDbExecutor<TQuery> executor, TQuery query, object commandObject, TimeSpan duration, Exception exception )
    {

      if ( executor == null )
        throw new ArgumentNullException( "executor" );

      if ( query == null )
        throw new ArgumentNullException( "query" );

      Executor = executor;
      Query = query;
      CommandObject = commandObject;
      Duration = duration;
      Exception = exception;

    }


    /// <summary>
    /// 负责执行查询的查询执行器
    /// </summary>
    public IDbExecutor<TQuery> Executor { get; private set; }


    /// <summary>
    /// 将要执行的查询对象
    /// </summary>
    public TQuery Query { get; private set; }


    /// <summary>
    /// 查询所生成的命令对象，如果有的话。
    /// </summary>
    public object CommandObject { get; private set; }


    /// <summary>
    /// 执行查询所耗费的时间
    /// </summary>
    public TimeSpan Duration { get; private set; }


    /// <summary>
    /// 若执行查询过程中出现了异常，则获取这个异常
    /// </summary>
    public Exception Exception { get; private set; }

  }
}
