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
  public sealed class SimpleDataTable
  {



    private bool _initialized = false;

    private void Initialize( IDataRecord record )
    {

      Columns = new SimpleDataColumn[record.FieldCount];

      for ( int i = 0; i < Columns.Length; i++ )
      {
        var fieldType = record.GetFieldType( i );
        var name = record.GetName( i );
        Columns[i] = SimpleDataColumn.CreateDataColumn( name, fieldType );

      }

      _initialized = true;
    }

    private void Fill( IDataRecord record )
    {
      if ( Columns == null )
        Initialize( record );

    }


    public SimpleDataColumn[] Columns
    {
      get;
      private set;
    }

  }
}
