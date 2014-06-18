using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{


  /// <summary>
  /// 辅助实现 IDbExecuteContext 接口的基类
  /// </summary>
  public abstract class DbExecuteContextBase : IDbExecuteContext
  {



    /// <summary>
    /// 创建数据库查询执行上下文
    /// </summary>
    /// <param name="dataReader">用于读取数据的 IDataReader 对象</param>
    /// <param name="tracing">用于追踪此次查询过程的追踪器</param>
    /// <param name="connectionResource">销毁该上下文时，需要同时销毁的连接资源</param>
    protected DbExecuteContextBase( IDataReader dataReader, IDbTracing tracing = null, IDisposable connectionResource = null, object sync = null )
    {

      if ( dataReader == null )
        throw new ArgumentNullException( "dataReader" );


      SyncRoot = sync;

      DataReader = dataReader;
      ConnectionResource = connectionResource;
      Tracing = tracing;

      DataTableAdapter = new DataTableAdapter();


      
      if ( SyncRoot != null )
        Monitor.Enter( SyncRoot );
    }


    /// <summary>
    /// 获取当前用于读取数据的 IDataReader 对象
    /// </summary>
    protected IDataReader DataReader
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取销毁该上下文时，需要同时销毁的连接资源
    /// </summary>
    protected IDisposable ConnectionResource
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取用于追踪数据查询过程的追踪器
    /// </summary>
    protected IDbTracing Tracing
    {
      get;
      private set;
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
    /// 用于填充数据到 DataTable 的 DataTableAdapter 对象
    /// </summary>
    protected DataTableAdapter DataTableAdapter
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
    public virtual DataTable LoadDataTable( int startRecord, int maxRecords )
    {
      return DataTableAdapter.FillDataTable( DataReader, startRecord, maxRecords );
    }


    /// <summary>
    /// 尝试读取下一个结果集
    /// </summary>
    /// <returns>若存在下一个结果集，则返回 true ，否则返回 false</returns>
    public virtual bool NextResult()
    {
      return DataReader.NextResult();
    }



    /// <summary>
    /// 获取更改、插入或删除的行数，如果没有任何行受到影响或语句失败，则为 0。-1 表示是 SELECT 语句。
    /// </summary>
    public int RecordsAffected
    {
      get { return DataReader.RecordsAffected; }
    }


    /// <summary>
    /// 读取一条记录，并将读取指针推移到下一个位置。
    /// </summary>
    /// <returns>若当前位置存在记录，则返回该记录，否则返回 null</returns>
    public IDataRecord ReadRecord()
    {
      if ( DataReader.Read() )
        return DataReader;

      else
        return null;
    }



    /// <summary>
    /// 销毁执行上下文所有相关的资源，并通知追踪器查询已经完成
    /// </summary>
    public virtual void Dispose()
    {
      if ( ConnectionResource != null )
        ConnectionResource.Dispose();

      if ( SyncRoot != null )
        Monitor.Exit( SyncRoot );

      DataReader.Dispose();

      try
      {
        if ( Tracing != null )
          Tracing.OnComplete();
      }
      catch { }
    }
  }



  /// <summary>
  /// 辅助实现 IAsyncDbExecuteContext 接口的基类
  /// </summary>
  public abstract class AsyncDbExecuteContextBase : DbExecuteContextBase, IAsyncDbExecuteContext
  {


    /// <summary>
    /// 创建数据库异步查询执行上下文
    /// </summary>
    /// <param name="dataReader">用于读取数据的 IDataReader 对象</param>
    /// <param name="tracing">用于追踪此次查询过程的追踪器</param>
    /// <param name="connectionResource">销毁该上下文时，需要同时销毁的连接资源</param>
    protected AsyncDbExecuteContextBase( DbDataReader dataReader, IDbTracing tracing = null, IDisposable connectionResource = null, object sync = null )
      : base( dataReader, tracing, connectionResource, sync )
    {

      DataReader = dataReader;

    }



    /// <summary>
    /// 获取当前用于读取数据的 IDataReader 对象
    /// </summary>
    protected new DbDataReader DataReader
    {
      get;
      private set;
    }


    
    /// <summary>
    /// 异步加载数据到 DataTable
    /// </summary>
    /// <param name="startRecord">要填充的起始记录位置</param>
    /// <param name="maxRecords">最多填充的记录条数</param>
    /// <param name="token">取消指示</param>
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
      return DataReader.NextResultAsync();
    }


    /// <summary>
    /// 异步读取一条记录，并将读取指针推移到下一个位置。
    /// </summary>
    /// <returns>若当前位置存在记录，则返回该记录，否则返回 null</returns>
    public Task<IDataRecord> ReadRecordAsync()
    {

      return DataReader.ReadAsync().ContinueWith( task =>
        {

          if ( task.Exception != null )
            throw task.Exception;

          if ( task.Result )
            return (IDataRecord) DataReader;

          else
            return null;

        } );

    }
  }


}
