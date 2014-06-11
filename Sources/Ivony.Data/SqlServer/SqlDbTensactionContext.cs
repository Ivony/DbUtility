using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SqlServer
{


  /// <summary>
  /// SQL Server 数据库事务上下文对象
  /// </summary>
  public class SqlDbTransactionContext : IDbTransactionContext<SqlDbUtility>
  {


    private bool _completed = false;
    private bool _disposed = false;

    private object _sync = new object();

    internal SqlDbTransactionContext( string connectionString, IDbTraceService traceService )
    {
      Connection = new SqlConnection( connectionString );
      DbExecutor = new SqlDbUtilityWithTransaction( this, traceService );
      _sync = new object();
    }


    /// <summary>
    /// 获取数据库连接（如果开启了数据库事务）
    /// </summary>
    public SqlConnection Connection
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取 SQL Server 数据库事务对象
    /// </summary>
    public SqlTransaction Transaction
    {
      get;
      private set;
    }


    /// <summary>
    /// 开启数据库事务
    /// </summary>
    public void BeginTransaction()
    {

      lock ( SyncRoot )
      {
        if ( Connection.State == ConnectionState.Closed )
        {
          Connection.Open();
          Transaction = Connection.BeginTransaction();
        }
      }
    }



    /// <summary>
    /// 提交事务
    /// </summary>
    public void Commit()
    {
      lock ( SyncRoot )
      {
        try
        {
          Transaction.Commit();
          _completed = true;
        }
        finally
        {
          Connection.Close();
        }
      }
    }


    /// <summary>
    /// 回滚事务
    /// </summary>
    public void Rollback()
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
          Connection.Close();
        }
      }
    }



    /// <summary>
    /// 获取查询执行器
    /// </summary>
    public SqlDbUtility DbExecutor
    {
      get;
      private set;
    }



    /// <summary>
    /// 销毁事务对象，若事务尚未提交，则会自动回滚
    /// </summary>
    public void Dispose()
    {

      lock ( SyncRoot )
      {
        if ( _disposed )
          throw new ObjectDisposedException( "SqlDbTransactionContext" );

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
    public object SyncRoot
    {
      get { return _sync; }
    }
  }
}
