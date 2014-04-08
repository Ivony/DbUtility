using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 查询执行上下文
  /// </summary>
  public interface IDbExecuteContext : IDisposable
  {

    /// <summary>
    /// 获取查询结果的 DataReader 对象
    /// </summary>
    IDataReader DataReader { get; }

    /// <summary>
    /// 将当前结果集数据填充 DataTable 并返回
    /// </summary>
    /// <param name="startRecord">填充的起始记录位置</param>
    /// <param name="maxRecord">最大要填充的记录条数</param>
    /// <returns>填充了数据的 DataTable</returns>
    DataTable LoadDataTable( int startRecord, int maxRecords );

    /// <summary>
    /// 获取用于确保同步查询过程的同步对象
    /// </summary>
    object SyncRoot { get; }

  }



  /// <summary>
  /// 异步查询执行上下文
  /// </summary>
  public interface IAsyncDbExecuteContext : IDbExecuteContext
  {

    /// <summary>
    /// 异步将当前结果集数据填充 DataTable 并返回
    /// </summary>
    /// <param name="startRecord">填充的起始记录位置</param>
    /// <param name="maxRecord">最大要填充的记录条数</param>
    /// <returns>填充了数据的 DataTable</returns>
    Task<DataTable> LoadDataTableAsync( int startRecord, int maxRecords, CancellationToken token = default( CancellationToken ) );

  }

}
