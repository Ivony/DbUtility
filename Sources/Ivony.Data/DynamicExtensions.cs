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





	private class DynamicDataRow : DynamicObject
    {

      private DataRow _dataRow;
      private DataColumnCollection _columns;

      public DynamicDataRow( DataRow dataRow )
      {
        _dataRow = dataRow;
        _columns = dataRow.Table.Columns;
      }

      public override bool TryGetMember( GetMemberBinder binder, out object result )
      {
        var name = binder.Name;
        var column = _columns[name];
        if ( column != null )
        {

          if ( binder.ReturnType.IsAssignableFrom( column.DataType ) )
            result = _dataRow[column];

          else
            result = _dataRow.FieldValue( column, binder.ReturnType );


          return true;
        }

        return base.TryGetMember( binder, out result );
      }


      public override bool TrySetMember( SetMemberBinder binder, object value )
      {
        var name = binder.Name;
        var column = _columns[name];
        if ( column != null && column.DataType.IsAssignableFrom( value.GetType() ) )
        {
          _dataRow[column] = value;
          return true;
        }

        return base.TrySetMember( binder, value );
      }

      public override bool TryGetIndex( GetIndexBinder binder, object[] indexes, out object result )
      {
        if ( indexes.Length != 1 )
          return base.TryGetIndex( binder, indexes, out result );


        var index = indexes[0];
        var name = index as string;
        if ( name != null )
        {
          var column = _columns[name];
          if ( column != null && binder.ReturnType.IsAssignableFrom( column.DataType ) )
          {
            result = _dataRow[column];
            return true;
          }
        }

        return base.TryGetIndex( binder, indexes, out result );
      }


      public override bool TrySetIndex( SetIndexBinder binder, object[] indexes, object value )
      {
        if ( indexes.Length != 1 )
          return base.TrySetIndex( binder, indexes, value );


        var name = indexes[0] as string;
        if ( name != null )
        {
          var column = _columns[name];
          if ( column != null && column.DataType.IsAssignableFrom( value.GetType() ) )
          {
            _dataRow[column] = value;
            return true;
          }
        }

        return base.TrySetIndex( binder, indexes, value );
      }


      public override IEnumerable<string> GetDynamicMemberNames()
      {
        return _columns.Cast<DataColumn>().Select( c => c.ColumnName );
      }

      public override bool TryConvert( ConvertBinder binder, out object result )
      {

        if ( binder.ReturnType == typeof( DataRow ) )
          result = _dataRow;

        else if ( binder.ReturnType == typeof( IDictionary<string, object> ) )
          result = _dataRow.ToDictionary();

        else if ( _columns.Count == 1 && _columns[0].DataType.IsSubclassOf( binder.ReturnType ) )
          result = _dataRow[0];

        else
          result = EntityExtensions.ToEntity( _dataRow, binder.ReturnType );

        return true;
      }

      public override DynamicMetaObject GetMetaObject( System.Linq.Expressions.Expression parameter )
      {
        return base.GetMetaObject( parameter );
      }
    }
  }

}
