using Ivony.Data.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data.MySqlClient
{

  /// <summary>
  /// MySql 数据执行上下文
  /// </summary>
  public class MySqlExecuteContext : AsyncDbResultBase
  {

    /// <summary>
    /// 创建 MySqlExecuteContext 对象
    /// </summary>
    /// <param name="connection">MySql 数据库连接</param>
    /// <param name="dataReader">MySql 数据读取器</param>
    /// <param name="tracing">用于当前查询的追踪器</param>
    public MySqlExecuteContext( MySqlConnection connection, MySqlDataReader dataReader, IDbTracing tracing )
      : base( dataReader, tracing, connection )
    {
      MySqlDataReader = dataReader;
    }

    /// <summary>
    /// 创建 MySqlExecuteContext 对象
    /// </summary>
    /// <param name="transaction">MySql 数据库事务上下文</param>
    /// <param name="dataReader">MySql 数据读取器</param>
    /// <param name="tracing">用于当前查询的追踪器</param>
    public MySqlExecuteContext( MySqlDbTransactionContext transaction, MySqlDataReader dataReader, IDbTracing tracing )
      : base( dataReader, tracing, null )
    {
      TransactionContext = transaction;
      MySqlDataReader = dataReader;
    }


    /// <summary>
    /// 数据读取器
    /// </summary>
    public MySqlDataReader MySqlDataReader
    {
      get;
      private set;
    }


    /// <summary>
    /// 数据库事务上下文，如果有的话
    /// </summary>
    public MySqlDbTransactionContext TransactionContext
    {
      get;
      private set;
    }

  }
}
