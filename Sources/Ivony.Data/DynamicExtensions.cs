using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;

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
      return new DynamicDataRow( dataItem );
    }

    /// <summary>
    /// 查询数据库并将最后一个结果集填充动态对象列表
    /// </summary>
    /// <param name="dbUtility">DbUtility 实例</param>
    /// <param name="expression">查询表达式</param>
    /// <returns>实体集</returns>
    public static dynamic[] Dynamics( this DbUtility dbUtility, IDbExpression expression )
    {
      var data = dbUtility.ExecuteData( expression );
      return data.Rows.Cast<DataRow>().Select( dataItem => dataItem.ToDynamic() ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将最后一个结果集填充动态对象列表
    /// </summary>
    /// <param name="dbUtility">DbUtility 实例</param>
    /// <param name="template">查询字符串模版</param>
    /// <param name="parameters">模版参数</param>
    /// <returns>实体集</returns>
    public static dynamic[] Dynamics( this DbUtility dbUtility, string template, params object[] parameters )
    {
      return dbUtility.Dynamics( TemplateExtensions.Template( template, parameters ) );
    }





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
        if ( column != null && binder.ReturnType.IsAssignableFrom( column.DataType ) )
        {
          result = _dataRow[column];
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
    }
  }

}
