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
    /// 将当前结果集数据填充 DataTable 并返回
    /// </summary>
    /// <param name="startRecord">填充的起始记录位置</param>
    /// <param name="maxRecords">最大要填充的记录条数</param>
    /// <returns>填充了数据的 DataTable</returns>
    DataTable LoadDataTable( int startRecord, int maxRecords );


    /// <summary>
    /// 尝试读取下一个结果集
    /// </summary>
    /// <returns>若存在下一个结果集，则返回 true ，否则返回 false</returns>
    bool NextResult();


    /// <summary>
    /// 获取更改、插入或删除的行数，如果没有任何行受到影响或语句失败，则为 0。-1 表示是 SELECT 语句。
    /// </summary>
    int RecordsAffected { get; }


    /// <summary>
    /// 读取一条记录，并将读取指针推移到下一个位置。
    /// </summary>
    /// <returns>若当前位置存在记录，则返回该记录，否则返回 null</returns>
    IDataRecord ReadRecord();



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


    /// <summary>
    /// 尝试异步读取下一个结果集
    /// </summary>
    /// <returns>若存在下一个结果集，则返回 true ，否则返回 false</returns>
    Task<bool> NextResultAsync();


    /// <summary>
    /// 异步读取一条记录，并将读取指针推移到下一个位置。
    /// </summary>
    /// <returns>若当前位置存在记录，则返回该记录，否则返回 null</returns>
    Task<IDataRecord> ReadRecordAsync();

  }
}
