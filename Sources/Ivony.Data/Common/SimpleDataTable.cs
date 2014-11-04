using System;
using System.Collections.Generic;
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




    internal SimpleDataTable()
    {
      SyncRoot = new object();
    }


    public object SyncRoot { get; private set; }


    private void Initialize( IDataRecord record )
    {

      if ( Columns != null || Rows != null )
        throw new InvalidOperationException();

      Columns = new SimpleDataColumn[record.FieldCount];


      for ( int i = 0; i < Columns.Length; i++ )
      {
        var fieldType = record.GetFieldType( i );
        var name = record.GetName( i );
        Columns[i] = SimpleDataColumn.CreateDataColumn( this, name, fieldType );
      }

      DataItemIndex = 0;
    }

    private void FillData( IDataReader reader )
    {

      lock ( SyncRoot )
      {
        Initialize( reader );


        while ( reader.Read() )
          FillDataItem( reader );
      }

      Rows = Enumerable.Range( 0, DataItemIndex ).Select( i => new SimpleDataRow( this, i ) ).ToArray();
    }

    private void FillDataItem( IDataRecord record )
    {
      for ( var i = 0; i < record.FieldCount; i++ )
        Columns[i].AddDataItem( record.GetValue( i ) );
    }


    internal int DataItemIndex { get; private set; }



    /// <summary>
    /// 获取数据表的所有列描述
    /// </summary>
    public SimpleDataColumn[] Columns
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
