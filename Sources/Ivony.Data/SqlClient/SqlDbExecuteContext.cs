using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{


  /// <summary>
  /// 实现 SQL Server 执行上下文
  /// </summary>
  public class SqlDbExecuteContext : AsyncDbExecuteContextBase
  {


    /// <summary>
    /// 创建 SqlExecuteContext 对象
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="reader">数据读取器</param>
    /// <param name="tracing">用于记录此次查询过程的的查询追踪器</param>
    internal SqlDbExecuteContext( SqlConnection connection, SqlDataReader reader, IDbTracing tracing )
      : base( reader, tracing, connection )
    {
      SqlDataReader = reader;
    }

    /// <summary>
    /// 创建 SqlExecuteContext 对象
    /// </summary>
    /// <param name="transaction">数据库事务</param>
    /// <param name="reader">数据读取器</param>
    /// <param name="tracing">用于记录此次查询过程的的查询追踪器</param>
    internal SqlDbExecuteContext( SqlDbTransactionContext transaction, SqlDataReader reader, IDbTracing tracing )
      : base( reader, tracing, null, transaction.SyncRoot )
    {
      SqlDataReader = reader;
    }


    /// <summary>
    /// 数据读取器
    /// </summary>
    public SqlDataReader SqlDataReader
    {
      get;
      private set;
    }


    /// <summary>
    /// 数据库事务上下文，如果有的话
    /// </summary>
    public SqlDbTransactionContext TransactionContext
    {
      get;
      private set;
    }
  }
}
