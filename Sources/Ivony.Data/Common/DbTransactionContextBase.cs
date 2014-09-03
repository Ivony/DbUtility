using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{


  /// <summary>
  /// 辅助实现 IDbTransactionContext 的抽象基类
  /// </summary>
  public abstract class DbTransactionContextBase<TDbHandler, TDbTransaction> : IDbTransactionContext<TDbHandler> where TDbTransaction : IDbTransaction
  {


    private bool _completed = false;
    private bool _disposed = false;

    private object _sync = new object();




    /// <summary>
    /// 获取数据库事务对象
    /// </summary>
    public TDbTransaction Transaction
    {
      get;
      private set;
    }



    private IDbConnection _connection;

    /// <summary>
    /// 开启数据库事务
    /// </summary>
    public virtual void BeginTransaction()
    {

      lock ( SyncRoot )
      {
        Transaction = CreateTransaction();
        _connection = Transaction.Connection;
      }
    }


    /// <summary>
    /// 派生类实现此方法以创建数据库事务对象
    /// </summary>
    /// <returns>数据库事务对象</returns>
    protected abstract TDbTransaction CreateTransaction();



    /// <summary>
    /// 提交事务
    /// </summary>
    public virtual void Commit()
    {
      lock ( SyncRoot )
      {

        if ( Transaction == null )
          throw new InvalidOperationException();

        try
        {
          Transaction.Commit();
          _completed = true;
        }
        finally
        {
          if ( _connection.State != ConnectionState.Closed )
            _connection.Close();
        }
      }
    }


    /// <summary>
    /// 回滚事务
    /// </summary>
    public virtual void Rollback()
    {
      lock ( SyncRoot )
      {
        try
        {
          Transaction.Rollback();
          _completed = true;
        }
        finally
        {
          if ( _connection.State != ConnectionState.Closed )
            _connection.Close();
        }
      }
    }



    /// <summary>
    /// 获取在事务中执行查询的执行器
    /// </summary>
    public abstract TDbHandler GetDbHandler();



    /// <summary>
    /// 销毁事务对象，若事务尚未提交，则会自动回滚
    /// </summary>
    public virtual void Dispose()
    {

      lock ( SyncRoot )
      {
        if ( _disposed )
          throw new ObjectDisposedException( GetType().Name );

        if ( _completed )
        {
          _disposed = true;
          return;
        }

        Rollback();
      }
    }


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public virtual object SyncRoot
    {
      get { return _sync; }
    }
  }
}
