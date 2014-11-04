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
  /// DataTable 的功能缩减版本
  /// </summary>
  public sealed class SimpleDataTable : IEnumerable<SimpleDataRow>
  {




    public static SimpleDataTable Fill( IDataReader reader )
    {
      var table = new SimpleDataTable();
      table.FillData( reader );

      return table;
    }

    public static SimpleDataTable Fill( IEnumerable<IDataRecord> records )
    {
      var table = new SimpleDataTable();
      var initialized = false;
      foreach ( var item in records )
      {
        if ( !initialized )
        {
          table.Initialize( item );
          initialized = true;
        }

        table.FillDataItem( item );
      }
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


      while ( reader.Read() )
        FillDataItem( reader );
    }


    private SimpleDataRow FillDataItem( IDataRecord record )
    {
      var values = new object[record.FieldCount];
      record.GetValues( values );

      return new SimpleDataRow( this, values );

    }


    internal int DataItemIndex { get; private set; }



    /// <summary>
    /// 获取数据表的所有列描述
    /// </summary>
    public SimpleDataColumnCollection Columns
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取数据表的所有数据行
    /// </summary>
    public SimpleDataRow[] Rows
    {
      get;
      private set;
    }




    IEnumerator<SimpleDataRow> IEnumerable<SimpleDataRow>.GetEnumerator()
    {
      return Rows.AsEnumerable().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return Rows.GetEnumerator();
    }
  }
}
