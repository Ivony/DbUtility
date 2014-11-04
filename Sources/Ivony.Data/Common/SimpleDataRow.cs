using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 简单数据行，一个只读的数据行实现
  /// </summary>
  public sealed class SimpleDataRow
  {

    internal SimpleDataRow( SimpleDataTable dataTable, object[] values )
    {

      DataTable = dataTable;
      _values = new ReadOnlyCollection<object>( values );
    }



    /// <summary>
    /// 获取所属的简单数据表
    /// </summary>
    public SimpleDataTable DataTable
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取数据行的索引位置
    /// </summary>
    public int DataItemIndex
    {
      get;
      private set;
    }


    private ReadOnlyCollection<object> _values;


    /// <summary>
    /// 以 object 数组的形式返回所有值
    /// </summary>
    /// <returns></returns>
    public object[] GetValues()
    {
      return _values.ToArray();
    }

    /// <summary>
    /// 获取指定列的值
    /// </summary>
    /// <param name="columnName">列名称</param>
    /// <returns></returns>
    public object this[string columnName]
    {
      get
      {
        return _values[DataTable.Columns.GetOrdinal( columnName )];
      }
    }


    /// <summary>
    /// 获取指定列的值
    /// </summary>
    /// <param name="columnIndex">列序号</param>
    /// <returns></returns>
    public object this[int columnIndex]
    {
      get
      {
        return _values[columnIndex];
      }
    }

  }
}
