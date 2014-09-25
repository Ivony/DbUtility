using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  public class SimpleDataColumn
  {

    private SimpleDataColumn()
    {

    }




    private class SimpleDataColumn<T> : SimpleDataColumn
    {

      private List<T> dataList = new List<T>();


      public void AddDataItem( T data )
      {
        dataList.Add( data );
      }
    }

    internal static SimpleDataColumn CreateDataColumn( string name, Type fieldType )
    {
      throw new NotImplementedException();
    }
  }
}
