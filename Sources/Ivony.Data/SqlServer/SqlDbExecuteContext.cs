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
  public class SqlDbExecuteContext : IAsyncDbExecuteContext
  {


    /// <summary>
    /// 创建 SqlExecuteContext 对象
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="reader">数据读取器</param>
    internal SqlDbExecuteContext( SqlConnection connection, SqlDataReader reader, IDbTracing tracing )
    {
      Connection = connection;
      DataReader = reader;

      DataTableAdapter = new DataTableAdapter();

      SyncRoot = new object();
    }

    /// <summary>
    /// 创建 SqlExecuteContext 对象
    /// </summary>
    /// <param name="transaction">数据库事务</param>
    /// <param name="reader">数据读取器</param>
    internal SqlDbExecuteContext( SqlDbTransactionContext transaction, SqlDataReader reader, IDbTracing tracing )
    {
      TransactionContext = transaction;
      Connection = transaction.Connection;
      DataReader = reader;

      DataTableAdapter = new DataTableAdapter();

      SyncRoot = transaction.SyncRoot;
    }


    /// <summary>
    /// 数据读取器
    /// </summary>
    public IDataReader DataReader
    {
      get;
      private set;
    }


    /// <summary>
    /// 数据库连接
    /// </summary>
    public SqlConnection Connection
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
    /// 销毁此执行上下文对象
    /// </summary>
    public void Dispose()
    {
      DataReader.Close();

      if ( TransactionContext == null )
        Connection.Close();
    }


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get;
      private set;
    }



    /// <summary>
    /// 加载数据到 DataTable
    /// </summary>
    /// <param name="startRecord">要填充的起始记录位置</param>
    /// <param name="maxRecords">最多填充的记录条数</param>
    /// <returns>填充好的 DataTable</returns>
    public DataTable LoadDataTable( int startRecord, int maxRecords )
    {
      return DataTableAdapter.FillDataTable( DataReader, startRecord, maxRecords );
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
  }
}
