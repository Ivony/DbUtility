using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{


  /// <summary>
  /// DataTable 的功能缩减版本
  /// </summary>
  public sealed class SimpleDataTable : IEnumerable<SimpleDataRow>
  {




    /// <summary>
    /// 用 DataReader 填充一个 SimpleDataTable 对象
    /// </summary>
    /// <param name="reader">用于读取数据的 DataReader</param>
    /// <returns></returns>
    public static SimpleDataTable Fill( IDataReader reader )
    {
      var table = new SimpleDataTable();
      table.FillData( reader );

      return table;
    }



    private SimpleDataTable()
    {
    }


    private void Initialize( IDataRecord record )
    {

      if ( Columns != null || Rows != null )
        throw new InvalidOperationException();

      var columns = new SimpleDataColumn[record.FieldCount];


      for ( int i = 0; i < columns.Length; i++ )
      {
        var dataType = record.GetDataTypeName( i );
        var fieldType = record.GetFieldType( i );
        var name = record.GetName( i );

        columns[i] = new SimpleDataColumn( this, name, fieldType, dataType );
      }

      Columns = new SimpleDataColumnCollection( columns );


      DataItemIndex = 0;
    }


    /// <summary>
    /// 指示列名称是否大小写敏感的
    /// </summary>
    public bool CaseSensitive
    {
      get { return Columns.CaseSensitive; }
    }


    private void FillData( IDataReader reader )
    {
      Initialize( reader );


      var rows = new List<SimpleDataRow>();

      while ( reader.Read() )
        rows.Add( FillDataItem( reader ) );

      Rows = new ReadOnlyCollection<SimpleDataRow>( rows );
    }


    private SimpleDataRow FillDataItem( IDataRecord record )
    {
      var values = new object[record.FieldCount];
      record.GetValues( values );

      return new SimpleDataRow( this, values );

    }


    internal int DataItemIndex { get; private set; }



    /// <summary>
    /// 获取数据表的所有数据列
    /// </summary>
    public SimpleDataColumnCollection Columns
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取数据表的所有数据行
    /// </summary>
    public IReadOnlyCollection<SimpleDataRow> Rows
    {
      get;
      private set;
    }




    IEnumerator<SimpleDataRow> IEnumerable<SimpleDataRow>.GetEnumerator()
    {
      return Rows.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return Rows.GetEnumerator();
    }





    internal PropertyDescriptorCollection GetProperties()
    {
      return new PropertyDescriptorCollection( Columns.Select( item => item.Value.GetPropertyDescriptor() ).ToArray(), true );
    }
  }
}
