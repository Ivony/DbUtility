using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{

  /// <summary>
  /// 代表一个查询
  /// </summary>
  public class Query
  {

    public SelectItem[] SelectItems
    {
      get;
      private set;
    }

    public FilterItem Filter
    {
      get;
      private set;
    }

    public SortItem Sort
    {
      get;
      private set;
    }

  }
}
