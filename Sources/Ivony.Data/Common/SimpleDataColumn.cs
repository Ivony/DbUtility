using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 定义简单数据列，保存最小的数据列的元数据
  /// </summary>
  public sealed class SimpleDataColumn
  {


    internal SimpleDataColumn( SimpleDataTable dataTable, string name, Type type, string dataType )
    {
      DataTable = dataTable;
      Name = name;
      ColumnType = type;
      DataTypeName = dataType;
    }


    /// <summary>
    /// 所属的数据表对象
    /// </summary>
    public SimpleDataTable DataTable { get; private set; }

    /// <summary>
    /// 列名称
    /// </summary>
    public string Name { get; private set; }


    /// <summary>
    /// 数据类型
    /// </summary>
    public Type ColumnType { get; private set; }


    /// <summary>
    /// 供参考的原始数据类型名称
    /// </summary>
    public string DataTypeName { get; private set; }
  }
}
