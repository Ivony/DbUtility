using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Fluent;

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
    public static DataTable ExecuteDataTable( this DbQuery query )
    {
      using ( var context = query.Execute() )
      {

        var data = new DataTable();
        data.Load( context.DataReader );

        return data;

      }
    }

    /// <summary>
    /// 异步执行查询并将第一个结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable> ExecuteDataTableAsync( this DbQuery query )
    {
      using ( var context = await query.ExecuteAsync() )
      {

        var data = new DataTable();
        data.Load( context.DataReader );

        return data;

      }
    }




    /// <summary>
    /// 执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataTable[] ExecuteAllDataTables( this DbQuery query )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var context = query.Execute() )
      {

        do
        {
          var data = new DataTable();
          data.Load( context.DataReader );

          dataTables.Add( data );

        } while ( context.DataReader.NextResult() );
      }

      return dataTables.ToArray();
    }

    /// <summary>
    /// 异步执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable[]> ExecuteAllDataTablesAsync( this DbQuery query )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var context = await query.ExecuteAsync() )
      {

        do
        {
          var data = new DataTable();
          data.Load( context.DataReader );

          dataTables.Add( data );

        } while ( context.DataReader.NextResult() );
      }

      return dataTables.ToArray();

    }




    /// <summary>
    /// 执行查询并返回首行首列
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static object ExecuteScalar( this DbQuery query )
    {
      using ( var context = query.Execute() )
      {
        if ( context.DataReader.Read() )
          return context.DataReader[0];

        else
          return null;
      }
    }

    /// <summary>
    /// 异步执行查询并返回首行首列
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static async Task<object> ExecuteScalarAsync( this DbQuery query )
    {
      using ( var context = await query.ExecuteAsync() )
      {
        if ( context.DataReader.Read() )
          return context.DataReader[0];

        else
          return null;
      }
    }





    /// <summary>
    /// 执行没有结果的查询
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询所影响的行数</returns>
    public static int ExecuteNonQuery( this DbQuery query )
    {
      using ( var context = query.Execute() )
      {
        return context.DataReader.RecordsAffected;
      }
    }

    /// <summary>
    /// 异步执行没有结果的查询
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询所影响的行数</returns>
    public static async Task<int> ExecuteNonQueryAsync( this DbQuery query )
    {
      using ( var context = await query.ExecuteAsync() )
      {
        return context.DataReader.RecordsAffected;
      }
    }




    /// <summary>
    /// 执行查询并返回首行
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataRow ExecuteFirstRow( this DbQuery query )
    {
      //UNDONE

      var data = query.ExecuteDataTable();
      if ( data.Rows.Count > 0 )
        return data.Rows[0];

      else
        return null;
    }

    /// <summary>
    /// 异步执行查询并返回首行
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static async Task<DataRow> ExecuteFirstRowAsync( this DbQuery query )
    {
      //UNDONE

      var data = await query.ExecuteDataTableAsync();
      if ( data.Rows.Count > 0 )
        return data.Rows[0];

      else
        return null;
    }




    /// <summary>
    /// 执行查询并返回首行首列
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static T ExecuteScalar<T>( this DbQuery query )
    {
      return ExecuteScalar( query ).ConvertTo<T>();
    }

    /// <summary>
    /// 异步执行查询并返回首行首列
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public async static Task<T> ExecuteScalarAsync<T>( this DbQuery query )
    {
      var scalar = await ExecuteScalarAsync( query );
      return scalar.ConvertTo<T>();
    }

  }



}
