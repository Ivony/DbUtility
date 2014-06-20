using Ivony.Data.Common;
using Ivony.Fluent;
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
  /// 提供基本的查询方法扩展
  /// </summary>
  public static class BasicExecuteExtensions
  {

    /// <summary>
    /// 执行查询并将第一个结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataTable ExecuteDataTable( this IDbExecutableQuery query )
    {
      using ( var context = query.Execute() )
      {
        return context.LoadDataTable( 0, 0 );
      }
    }

    /// <summary>
    /// 异步执行查询并将第一个结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable> ExecuteDataTableAsync( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      using ( var context = await query.ExecuteAsync( token ) )
      {

        return await context.LoadDataTableAsync( 0, 0, token );

      }
    }




    /// <summary>
    /// 执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataTable[] ExecuteAllDataTables( this IDbExecutableQuery query )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var context = query.Execute() )
      {

        do
        {
          dataTables.Add( context.LoadDataTable( 0, 0 ) );
        } while ( context.NextResult() );
      }

      return dataTables.ToArray();
    }

    /// <summary>
    /// 异步执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable[]> ExecuteAllDataTablesAsync( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var context = await query.ExecuteAsync( token ) )
      {

        do
        {
          dataTables.Add( context.LoadDataTable( 0, 0 ) );

        } while ( await context.NextResultAsync() );
      }

      return dataTables.ToArray();

    }




    /// <summary>
    /// 执行查询并返回首行首列
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static object ExecuteScalar( this IDbExecutableQuery query )
    {
      using ( var context = query.Execute() )
      {
        var record = context.ReadRecord();
        if ( record != null && record.FieldCount > 0 )
          return record[0];

        else
          return null;
      }
    }

    /// <summary>
    /// 异步执行查询并返回首行首列
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<object> ExecuteScalarAsync( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      using ( var context = await query.ExecuteAsync( token ) )
      {

        var record = await context.ReadRecordAsync();
        if ( record != null && record.FieldCount > 0 )
          return record[0];

        else
          return null;
      }
    }





    /// <summary>
    /// 执行没有结果的查询
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询所影响的行数</returns>
    public static int ExecuteNonQuery( this IDbExecutableQuery query )
    {
      using ( var context = query.Execute() )
      {
        return context.RecordsAffected;
      }
    }

    /// <summary>
    /// 异步执行没有结果的查询
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询所影响的行数</returns>
    public static async Task<int> ExecuteNonQueryAsync( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      using ( var context = await query.ExecuteAsync( token ) )
      {
        return context.RecordsAffected;
      }
    }




    /// <summary>
    /// 执行查询并返回首行
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataRow ExecuteFirstRow( this IDbExecutableQuery query )
    {
      //UNDONE

      using ( var context = query.Execute() )
      {
        var data = context.LoadDataTable( 0, 1 );
        if ( data.Rows.Count > 0 )
          return data.Rows[0];

        else
          return null;
      }
    }

    /// <summary>
    /// 异步执行查询并返回首行
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataRow> ExecuteFirstRowAsync( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      //UNDONE

      using ( var context = await query.ExecuteAsync( token ) )
      {
        var data = context.LoadDataTable( 0, 1 );
        if ( data.Rows.Count > 0 )
          return data.Rows[0];

        else
          return null;
      }
    }




    /// <summary>
    /// 执行查询并返回首行首列
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static T ExecuteScalar<T>( this IDbExecutableQuery query )
    {
      return DbValueConverters.Convert<T>( ExecuteScalar( query ) );
    }

    /// <summary>
    /// 异步执行查询并返回首行首列
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public async static Task<T> ExecuteScalarAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      return DbValueConverters.Convert<T>( await ExecuteScalarAsync( query, token ) );
    }

  }



}
