using Ivony.Data.Common;
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
    public static DataTable ExecuteDataTable( this IDbExecuteContext context )
    {
      using ( var result = context.Execute() )
      {
        return result.LoadDataTable( 0, 0 );
      }
    }

    /// <summary>
    /// 异步执行查询并将第一个结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable> ExecuteDataTableAsync( this IAsyncDbExecuteContext context, CancellationToken token = default( CancellationToken ) )
    {
      using ( var result = await context.ExecuteAsync( token ) )
      {

        return await result.LoadDataTableAsync( 0, 0, token );

      }
    }




    /// <summary>
    /// 执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataTable[] ExecuteAllDataTables( this IDbExecuteContext context )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var result = context.Execute() )
      {

        do
        {
          dataTables.Add( result.LoadDataTable( 0, 0 ) );
        } while ( result.NextResult() );
      }

      return dataTables.ToArray();
    }

    /// <summary>
    /// 异步执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable[]> ExecuteAllDataTablesAsync( this IAsyncDbExecuteContext query, CancellationToken token = default( CancellationToken ) )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var result  = await query.ExecuteAsync( token ) )
      {

        do
        {
          dataTables.Add( result.LoadDataTable( 0, 0 ) );

        } while ( await result.NextResultAsync() );
      }

      return dataTables.ToArray();

    }




    /// <summary>
    /// 执行查询并返回首行首列
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static object ExecuteScalar( this IDbExecuteContext context )
    {
      using ( var result = context.Execute() )
      {
        var record = result.ReadRecord();
        if ( record != null && record.FieldCount > 0 )
          return record[0];

        else
          return null;
      }
    }

    /// <summary>
    /// 异步执行查询并返回首行首列
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<object> ExecuteScalarAsync( this IAsyncDbExecuteContext context, CancellationToken token = default( CancellationToken ) )
    {
      using ( var result = await context.ExecuteAsync( token ) )
      {

        var record = await result.ReadRecordAsync();
        if ( record != null && record.FieldCount > 0 )
          return record[0];

        else
          return null;
      }
    }





    /// <summary>
    /// 执行没有结果的查询
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <returns>查询所影响的行数</returns>
    public static int ExecuteNonQuery( this IDbExecuteContext context )
    {
      using ( var result = context.Execute() )
      {
        return result.RecordsAffected;
      }
    }

    /// <summary>
    /// 异步执行没有结果的查询
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询所影响的行数</returns>
    public static async Task<int> ExecuteNonQueryAsync( this IAsyncDbExecuteContext context, CancellationToken token = default( CancellationToken ) )
    {
      using ( var result = await context.ExecuteAsync( token ) )
      {
        return result.RecordsAffected;
      }
    }




    /// <summary>
    /// 执行查询并返回首行
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataRow ExecuteFirstRow( this IDbExecuteContext context )
    {
      //UNDONE

      using ( var result = context.Execute() )
      {
        var data = result.LoadDataTable( 0, 1 );
        if ( data.Rows.Count > 0 )
          return data.Rows[0];

        else
          return null;
      }
    }

    /// <summary>
    /// 异步执行查询并返回首行
    /// </summary>
    /// <param name="context">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataRow> ExecuteFirstRowAsync( this IAsyncDbExecuteContext context, CancellationToken token = default( CancellationToken ) )
    {
      //UNDONE

      using ( var result = await context.ExecuteAsync( token ) )
      {
        var data = result.LoadDataTable( 0, 1 );
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
    /// <param name="context">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static T ExecuteScalar<T>( this IDbExecuteContext context )
    {
      return DbValueConverter.ConvertFrom<T>( ExecuteScalar( context ) );
    }

    /// <summary>
    /// 异步执行查询并返回首行首列
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="context">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public async static Task<T> ExecuteScalarAsync<T>( this IAsyncDbExecuteContext context, CancellationToken token = default( CancellationToken ) )
    {
      return DbValueConverter.ConvertFrom<T>( await ExecuteScalarAsync( context, token ) );
    }

  }



}
