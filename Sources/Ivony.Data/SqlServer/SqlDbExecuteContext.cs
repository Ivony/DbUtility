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
      Tracing = tracing;

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
      Tracing = tracing;

      DataTableAdapter = new DataTableAdapter();

      SyncRoot = transaction.SyncRoot;
    }


    /// <summary>
    /// 数据读取器
    /// </summary>
    public SqlDataReader DataReader
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
    /// 获取数据库查询追踪器
    /// </summary>
    protected IDbTracing Tracing
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

      if ( Tracing != null )
        Tracing.OnComplete();
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




    /// <summary>
    /// 尝试读取下一个结果集
    /// </summary>
    /// <returns>若存在下一个结果集，则返回 true ，否则返回 false</returns>
    public bool NextResult()
    {
      return DataReader.NextResult();
    }


    /// <summary>
    /// 尝试异步读取下一个结果集
    /// </summary>
    /// <returns>若存在下一个结果集，则返回 true ，否则返回 false</returns>
    public Task<bool> NextResultAsync()
    {
      return DataReader.NextResultAsync();
    }



    /// <summary>
    /// 获取更改、插入或删除的行数，如果没有任何行受到影响或语句失败，则为 0。-1 表示是 SELECT 语句。
    /// </summary>
    public int RecordsAffected
    {
      get { return DataReader.RecordsAffected; }
    }


    IDataRecord IDbExecuteContext.ReadRecord()
    {
      if ( DataReader.Read() )
        return DataReader;

      else
        return null;
    }


    async Task<IDataRecord> IAsyncDbExecuteContext.ReadRecordAsync()
    {
      if ( await DataReader.ReadAsync() )
        return DataReader;

      else
        return null;
    }
  }
}
