using System;
using System.Collections.Generic;
using System.ComponentModel;
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






    private PropertyDescriptor propertyDescriptor;
    private object sync = new object();



    internal PropertyDescriptor GetPropertyDescriptor()
    {
      lock ( sync )
      {
        if ( propertyDescriptor == null )
          propertyDescriptor = new SimpleDataColumnPropertyDescriptor( this );

        return propertyDescriptor;
      }
    }


    private class SimpleDataColumnPropertyDescriptor : PropertyDescriptor
    {

      public SimpleDataColumnPropertyDescriptor( SimpleDataColumn column )
        : base( column.Name, null )
      {
        Column = column;
      }


      public SimpleDataColumn Column { get; private set; }

      public override bool CanResetValue( object component )
      {
        return false;
      }

      public override Type ComponentType
      {
        get { return typeof( SimpleDataRow ); }
      }

      public override object GetValue( object component )
      {
        var row = (SimpleDataRow) component;
        return row[Column];
      }

      public override bool IsReadOnly
      {
        get { return true; }
      }

      public override Type PropertyType
      {
        get { return Column.ColumnType; }
      }

      public override void ResetValue( object component )
      {
        throw new NotSupportedException();
      }

      public override void SetValue( object component, object value )
      {
        throw new NotSupportedException();
      }

      public override bool ShouldSerializeValue( object component )
      {
        return false;
      }
    }



  }
}
