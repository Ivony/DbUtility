using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.SqlServer
{


  /// <summary>
  /// 实现 SQL Server 执行上下文
  /// </summary>
  public class SqlDbExecuteContext : DbExecuteContextBase, IAsyncDbExecuteContext
  {


    /// <summary>
    /// 创建 SqlExecuteContext 对象
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="reader">数据读取器</param>
    internal SqlDbExecuteContext( SqlConnection connection, SqlDataReader reader, IDbTracing tracing )
      : base( reader, connection, tracing )
    {
      SqlDataReader = reader;
    }

    /// <summary>
    /// 创建 SqlExecuteContext 对象
    /// </summary>
    /// <param name="transaction">数据库事务</param>
    /// <param name="reader">数据读取器</param>
    internal SqlDbExecuteContext( SqlDbTransactionContext transaction, SqlDataReader reader, IDbTracing tracing )
      : base( reader, null, tracing )
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



    /// <summary>
    /// 获取 DataTableAdapter 对象
    /// </summary>
    protected DataTableAdapter DataTableAdapter
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取数据库查询追踪器
    /// </summary>
    protected IDbTracing Tracing
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public override object SyncRoot
    {
      get
      {
        if ( TransactionContext != null )
          return TransactionContext.SyncRoot;

        else
          return base.SyncRoot;
      }
    }





    /// <summary>
    /// 异步加载数据到 DataTable
    /// </summary>
    /// <param name="startRecord">要填充的起始记录位置</param>
    /// <param name="maxRecords">最多填充的记录条数</param>
    /// <returns>填充好的 DataTable</returns>
    public Task<DataTable> LoadDataTableAsync( int startRecord, int maxRecords, CancellationToken token = default( CancellationToken ) )
    {

      var builder = new TaskCompletionSource<DataTable>();

      if ( token.IsCancellationRequested )
      {
        builder.SetCanceled();
        return builder.Task;
      }


      try
      {

        var result = LoadDataTable( startRecord, maxRecords );
        builder.SetResult( result );
        return builder.Task;

      }
      catch ( Exception exception )
      {

        builder.SetException( exception );
        return builder.Task;
      }
    }




    /// <summary>
    /// 尝试异步读取下一个结果集
    /// </summary>
    /// <returns>若存在下一个结果集，则返回 true ，否则返回 false</returns>
    public Task<bool> NextResultAsync()
    {
      return SqlDataReader.NextResultAsync();
    }



    async Task<IDataRecord> IAsyncDbExecuteContext.ReadRecordAsync()
    {
      if ( await SqlDataReader.ReadAsync() )
        return SqlDataReader;

      else
        return null;
    }
  }
}
