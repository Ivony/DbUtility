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
  public sealed class SqlDbTransactionContext : DbTransactionContextBase<SqlDbHandler, SqlTransaction>
  {



    internal SqlDbTransactionContext( string connectionString, SqlDbConfiguration configuration )
    {
      Connection = new SqlConnection( connectionString );
      _executor = new SqlServerHandlerWithTransaction( this, configuration );
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

      return Connection.BeginTransaction();
    }


    private SqlServerHandlerWithTransaction _executor;

    /// <summary>
    /// 获取用于在事务中执行查询的 SQL Server 查询执行器
    /// </summary>
    public override SqlDbHandler GetDbHandler()
    {
      return _executor;
    }



    private class SqlServerHandlerWithTransaction : SqlDbHandler
    {
      public SqlServerHandlerWithTransaction( SqlDbTransactionContext transaction, SqlDbConfiguration configuration )
        : base( transaction.Connection.ConnectionString )
      {
        TransactionContext = transaction;
      }



      protected override SqlDbHandler WithConfiguration( Action<SqlDbConfiguration> configurationSetter )
      {
        var newConfiguration = new SqlDbConfiguration( Configuration );
        configurationSetter( newConfiguration );
        return new SqlServerHandlerWithTransaction( TransactionContext, newConfiguration );
      }


      /// <summary>
      /// 当前所处的事务
      /// </summary>
      protected SqlDbTransactionContext TransactionContext
      {
        get;
        private set;
      }


      internal override SqlCommand ApplyConnection( SqlCommand command )
      {
        command.Connection = TransactionContext.Connection;
        command.Transaction = TransactionContext.Transaction;

        return command;
      }
    }
  }
}
