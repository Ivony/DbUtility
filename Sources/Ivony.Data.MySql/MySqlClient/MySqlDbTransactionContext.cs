using Ivony.Data.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.MySqlClient
{
  public class MySqlDbTransactionContext : DbTransactionContextBase<MySqlDbExecutor, MySqlTransaction>
  {




    /// <summary>
    /// 获取数据库连接
    /// </summary>
    public MySqlConnection Connection
    {
      get;
      private set;
    }



    internal MySqlDbTransactionContext( string connectionString, MySqlDbConfiguration configuration )
    {
      Connection = new MySqlConnection( connectionString );
      _executor = new MySqlDbExecutorWithTransaction( this, configuration );
    }



    /// <summary>
    /// 打开数据库连接并创建数据库事务对象。
    /// </summary>
    /// <returns>SQL Server 数据库事务对象</returns>
    protected override MySqlTransaction CreateTransaction()
    {
      if ( Connection.State == ConnectionState.Closed )
        Connection.Open();

      return Connection.BeginTransaction();
    }



    private MySqlDbExecutor _executor;

    /// <summary>
    /// 获取用于在事务中执行查询的 MySql 查询执行器
    /// </summary>
    public override MySqlDbExecutor DbExecutor
    {
      get { return _executor; }
    }


    private class MySqlDbExecutorWithTransaction : MySqlDbExecutor
    {

      public MySqlDbExecutorWithTransaction( MySqlDbTransactionContext transaction, MySqlDbConfiguration configuration )
        : base( transaction.Connection.ConnectionString, configuration )
      {
        _transaction = transaction;
      }


      private MySqlDbTransactionContext _transaction;


      protected override IDbResult Execute( MySqlCommand command, IDbTracing tracing )
      {

        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

        command.Connection = _transaction.Connection;
        command.Transaction = _transaction.Transaction;

        if ( Configuration.QueryExecutingTimeout.HasValue )
          command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


        var dataReader = command.ExecuteReader();
        var context = new MySqlExecuteContext( _transaction, dataReader, tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }

    }
  }
}
