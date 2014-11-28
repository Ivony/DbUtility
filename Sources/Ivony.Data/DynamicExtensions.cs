using Ivony.Data.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 提供关于动态对象的扩展方法
  /// </summary>
  public static class DynamicExtensions
  {

    /// <summary>
    /// 将 DataRow 转换为动态对象
    /// </summary>
    /// <param name="dataItem"></param>
    /// <returns></returns>
    public static dynamic ToDynamic( this DataRow dataItem )
    {
      if ( dataItem == null )
        return null;

      return new DynamicDataRow( dataItem );
    }



    /// <summary>
    /// 将 DataTable 转换为动态对象数组
    /// </summary>
    /// <param name="data">DataTable对象</param>
    /// <returns></returns>
    public static dynamic[] ToDynamics( this DataTable data )
    {
      return data.Rows.Cast<DataRow>().Select( dataItem => dataItem.ToDynamic() ).ToArray();
    }


    /// <summary>
    /// 执行查询并将第一个结果集填充动态对象列表
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>查询结果</returns>
    public static dynamic[] ExecuteDynamics( this IDbExecutableQuery query )
    {
      var data = query.ExecuteDataTable();
      return ToDynamics( data );
    }

#if !NET40
    /// <summary>
    /// 异步执行查询并将第一个结果集填充动态对象列表
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<dynamic[]> ExecuteDynamicsAsync( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      var data = await query.ExecuteDataTableAsync( token );
      return ToDynamics( data );
    }
#endif



    /// <summary>
    /// 执行查询并将第一个结果集的第一条记录填充动态对象
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>查询结果</returns>
    public static dynamic ExecuteDynamicObject( this IDbExecutableQuery query )
    {
      var dataItem = query.ExecuteFirstRow();
      return ToDynamic( dataItem );
    }

#if !NET40
    /// <summary>
    /// 异步执行查询并将第一个结果集的第一条记录填充动态对象
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>查询结果</returns>
    public static async Task<dynamic> ExecuteDynamicObjectAsync( this IAsyncDbExecutableQuery query )
    {
      var dataItem = await query.ExecuteFirstRowAsync();
      return ToDynamic( dataItem );
    }
#endif


  }
}
