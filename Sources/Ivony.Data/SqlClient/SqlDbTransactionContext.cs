using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{


  /// <summary>
  /// SQL Server 数据库事务上下文对象
  /// </summary>
  public class SqlDbTransactionContext : DbTransactionContextBase<SqlDbExecutor, SqlTransaction>
  {





    private IsolationLevel _isolationLevel;

    internal SqlDbTransactionContext( string connectionString, SqlDbConfiguration configuration, IsolationLevel isolationLevel )
    {
      Connection = new SqlConnection( connectionString );
      _executor = new SqlDbExecutorWithTransaction( this, configuration );
      _isolationLevel = isolationLevel;
    }


    /// <summary>
    /// 获取数据库连接
    /// </summary>
    public SqlConnection Connection
    {
      get;
      private set;
    }



    /// <summary>
    /// 打开数据库连接并创建数据库事务对象。
    /// </summary>
    /// <returns>SQL Server 数据库事务对象</returns>
    protected override SqlTransaction CreateTransaction()
    {
      if ( Connection.State == ConnectionState.Closed )
        Connection.Open();

      return Connection.BeginTransaction( _isolationLevel );
    }


    private SqlDbExecutorWithTransaction _executor;

    /// <summary>
    /// 获取用于在事务中执行查询的 SQL Server 查询执行器
    /// </summary>
    public override SqlDbExecutor DbExecutor
    {
      get { return _executor; }
    }



    private class SqlDbExecutorWithTransaction : SqlDbExecutor
    {
      public SqlDbExecutorWithTransaction( SqlDbTransactionContext transaction, SqlDbConfiguration configuration )
        : base( transaction.Connection.ConnectionString, configuration )
      {
        TransactionContext = transaction;
      }


      /// <summary>
      /// 当前所处的事务
      /// </summary>
      protected SqlDbTransactionContext TransactionContext
      {
        get;
        private set;
      }

#if !NET40
      /// <summary>
      /// 重写 ExecuteAsync 方法，在事务中异步执行查询
      /// </summary>
      /// <param name="command">要执行的查询命令</param>
      /// <param name="token">取消指示</param>
      /// <param name="tracing">用于追踪的追踪器</param>
      /// <returns>查询执行上下文</returns>
      protected sealed override async Task<IAsyncDbExecuteContext> ExecuteAsync( SqlCommand command, CancellationToken token, IDbTracing tracing = null )
      {
        try
        {
          TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

          command.Connection = TransactionContext.Connection;
          command.Transaction = TransactionContext.Transaction;

          if ( Configuration.QueryExecutingTimeout.HasValue )
            command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


          var reader = await command.ExecuteReaderAsync( token );
          var context = new SqlDbExecuteContext( TransactionContext, reader, tracing );

          TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

          return context;
        }
        catch ( DbException exception )
        {
          TryExecuteTracing( tracing, t => t.OnException( exception ) );
          throw;
        }

      }
#endif

      /// <summary>
      /// 执行查询命令并返回执行上下文
      /// </summary>
      /// <param name="command">查询命令</param>
      /// <param name="tracing">用于追踪查询过程的追踪器</param>
      /// <returns>查询执行上下文</returns>
      protected sealed override IDbExecuteContext Execute( SqlCommand command, IDbTracing tracing = null )
      {
        try
        {
          TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

          command.Connection = TransactionContext.Connection;
          command.Transaction = TransactionContext.Transaction;

          if ( Configuration.QueryExecutingTimeout.HasValue )
            command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


          var reader = command.ExecuteReader();
          var context = new SqlDbExecuteContext( TransactionContext, reader, tracing );

          TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

          return context;
        }
        catch ( DbException exception )
        {
          TryExecuteTracing( tracing, t => t.OnException( exception ) );
          throw;
        }
      }


    }

  }
}
