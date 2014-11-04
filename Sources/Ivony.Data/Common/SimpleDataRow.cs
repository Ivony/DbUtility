using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  public sealed class SimpleDataRow
  {
    internal SimpleDataRow( SimpleDataTable dataTable, int index )
    {

      DataTable = dataTable;
      DataItemIndex = index;

    }


    public SimpleDataTable DataTable
    {
      get;
      private set;
    }


    public int DataItemIndex
    {
      get;
      private set;
    }


  }
}
