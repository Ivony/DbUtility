using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Ivony.Data.Common;


namespace Ivony.Data
{
  /// <summary>
  /// 为系统的 DataSet 和 DataTable 对象提供扩展方法
  /// </summary>
  public static class DataSetExtensions
  {

    /// <summary>
    /// 将 DataTable 转换为易于数据绑定的 DataRowView 对象列表
    /// </summary>
    /// <param name="table">要转换的 DataTable</param>
    /// <returns>易于数据绑定的形式</returns>
    public static IEnumerable<DataRowView> GetRowViews( this DataTable table )
    {
      return table.DefaultView.Cast<DataRowView>();
    }


    /// <summary>
    /// 获取 DataRow 列表
    /// </summary>
    /// <param name="table">要转换的 DataTable</param>
    /// <returns>DataRow 列表</returns>
    public static IEnumerable<DataRow> GetRows( this DataTable table )
    {
      return table.Rows.Cast<DataRow>();
    }


    /// <summary>
    /// 将 DataTable 转换为易于数据绑定的 IDictionary&lt;string, object&gt; 对象列表
    /// </summary>
    /// <param name="table">要转换的 DataTable</param>
    /// <returns>易于数据绑定的形式</returns>
    public static IEnumerable<IDictionary<string, object>> AsDictionaries( this DataTable table )
    {
      return GetRowViews( table ).Select( item => CreateDictionary( item, table ) );
    }

    private static IDictionary<string, object> CreateDictionary( DataRowView item, DataTable table )
    {
      var result = new Dictionary<string, object>( table.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase );
      foreach ( DataColumn column in table.Columns )
        result.Add( column.ColumnName, item[column.ColumnName] );

      return result;
    }




    /// <summary>
    /// 获取第一列数据
    /// </summary>
    /// <typeparam name="T">列数据类型</typeparam>
    /// <param name="table">数据对象</param>
    /// <returns></returns>
    public static T[] Column<T>( this DataTable table )
    {
      return table.Rows.Cast<DataRow>().Select( item => item.FieldValue<T>( 0 ) ).ToArray();
    }

    /// <summary>
    /// 获取指定列数据
    /// </summary>
    /// <typeparam name="T">列数据类型</typeparam>
    /// <param name="table">数据对象</param>
    /// <param name="columnName">列名</param>
    /// <returns></returns>
    public static T[] Column<T>( this DataTable table, string columnName )
    {
      return table.Rows.Cast<DataRow>().Select( item => item.FieldValue<T>( columnName ) ).ToArray();
    }


    /// <summary>
    /// 将 DataRow 转换为等效的 Dictionary
    /// </summary>
    /// <param name="dataItem">要转换的 DataRow</param>
    /// <returns>等效的 Dictionary</returns>
    public static IDictionary<string, object> ToDictionary( this DataRow dataItem )
    {

      IDictionary<string, object> result;

      var table = dataItem.Table;
      if ( table.CaseSensitive )
        result = new Dictionary<string, object>( StringComparer.Ordinal );
      else
        result = new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );

      foreach ( DataColumn column in table.Columns )
        result.Add( column.ColumnName, dataItem[column] );

      return result;
    }


    /// <summary>
    /// 将 DataTable 转换为等效的 Dictionary 数组
    /// </summary>
    /// <param name="data">要转换的 DataTable</param>
    /// <returns>等效的 Dictionary</returns>
    public static IDictionary<string, object>[] ToDictionaries( this DataTable data )
    {
      return data.Rows.Cast<DataRow>().Where( r => r.RowState == DataRowState.Unchanged ).Select( r => ToDictionary( r ) ).ToArray();
    }



    /// <summary>
    /// 执行查询并返回第一列数据
    /// </summary>
    /// <typeparam name="T">列类型</typeparam>
    /// <param name="context">要执行的查询</param>
    /// <returns>第一列的数据</returns>
    public static T[] ExecuteFirstColumn<T>( this IDbExecuteContext context )
    {
      return context.ExecuteDataTable().Column<T>();
    }

    /// <summary>
    /// 异步执行查询并返回第一列数据
    /// </summary>
    /// <typeparam name="T">列类型</typeparam>
    /// <param name="context">要执行的查询</param>
    /// <returns>第一列的数据</returns>
    public async static Task<T[]> ExecuteFirstColumnAsync<T>( this IAsyncDbExecuteContext context )
    {
      return (await context.ExecuteDataTableAsync()).Column<T>();
    }


    /// <summary>
    /// 执行查询并将数据转换为 DataRowView 集合返回
    /// </summary>
    /// <param name="context">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public static IEnumerable<DataRowView> ExecuteDataRowViews( this IDbExecuteContext context )
    {
      return context.ExecuteDataTable().GetRowViews();
    }


    /// <summary>
    /// 异步执行查询并将数据转换为 DataRowView 集合返回
    /// </summary>
    /// <param name="context">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public async static Task<IEnumerable<DataRowView>> ExecuteDataRowViewsAsync( this IAsyncDbExecuteContext context )
    {
      return (await context.ExecuteDataTableAsync()).GetRowViews();
    }



    /// <summary>
    /// 执行查询并将第一行数据数据转换为 DataRowView 返回
    /// </summary>
    /// <param name="context">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public static DataRowView ExecuteFirstDataRowView( this IDbExecuteContext context )
    {
      return context.ExecuteDataTable().GetRowViews().FirstOrDefault();
    }

    /// <summary>
    /// 异步执行查询并将第一行数据数据转换为 DataRowView 返回
    /// </summary>
    /// <param name="context">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public async static Task<DataRowView> ExecuteFirstDataRowViewAsync( this IAsyncDbExecuteContext context )
    {
      return (await context.ExecuteDataTableAsync()).GetRowViews().FirstOrDefault();
    }


  }
}
