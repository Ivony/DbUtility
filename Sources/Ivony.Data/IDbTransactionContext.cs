using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  public interface IDbTransactionContext : IDisposable
  {
    /// <summary>
    /// 提交事务
    /// </summary>
    void Commit();

    /// <summary>
    /// 回滚事务
    /// </summary>
    void Rollback();


    /// <summary>
    /// 开启事务，若事务创建时已经开启，则调用该方法没有副作用
    /// </summary>
    void BeginTransaction();
  }

  /// <summary>
  /// 定义数据库事务上下文
  /// </summary>
  /// <typeparam name="TDbExecutor">数据库查询执行程序类型</typeparam>
  public interface IDbTransactionContext<out TDbExecutor> : IDbTransactionContext
  {

    /// <summary>
    /// 数据库查询执行程序
    /// </summary>
    TDbExecutor DbExecutor { get; }

    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    object SyncRoot { get; }
  }



  /// <summary>
  /// 定义异步数据库事务上下文
  /// </summary>
  /// <typeparam name="TDbExecutor">数据查询执行器类型</typeparam>
  public interface IAsyncDbTransactionContext<out TDbExecutor> : IDbTransactionContext<TDbExecutor>
  {


    /// <summary>
    /// 异步提交事务
    /// </summary>
    void CommitAsync();

    /// <summary>
    /// 异步回滚事务
    /// </summary>
    void RollbackAsync();

    /// <summary>
    /// 异步开启事务，若事务创建时已经开启，则调用该方法没有副作用
    /// </summary>
    void BeginTransactionAsync();
  }

}

