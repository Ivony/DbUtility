using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
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
    /// <param name="connectionResource">销毁该上下文时，需要同时销毁的连接资源</param>
    /// <param name="SyncRoot">用于同步的对象</param>
    protected DbExecuteContextBase( IDataReader dataReader, IDisposable connectionResource = null, IDbTracing tracing = null )
    {

      if ( dataReader == null )
        throw new ArgumentNullException( "dataReader" );

      DataReader = dataReader;
      ConnectionResource = connectionResource;
      Tracing = tracing;
      SyncRoot = new object();

      DataTableAdapter = new DataTableAdapter();

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
    public virtual object SyncRoot
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

      DataReader.Dispose();

      try
      {
        if ( Tracing != null )
          Tracing.OnComplete();
      }
      catch { }
    }
  }
}
