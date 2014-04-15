using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义执行查询时的上下文信息
  /// </summary>
  public class DbQueryExecutingContext<TQuery> where TQuery : IDbQuery
  {

    public DbQueryExecutingContext( IDbExecutor<TQuery> executor, TQuery query, object commandObject )
    {

      if ( executor == null )
        throw new ArgumentNullException( "executor" );

      if ( query == null )
        throw new ArgumentNullException( "query" );

      Executor = executor;
      Query = query;
      CommandObject = commandObject;

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
    /// 查询所使用的命令对象，如果有的话。
    /// </summary>
    public object CommandObject { get; private set; }


  }
}
