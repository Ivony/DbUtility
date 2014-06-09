using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{
  public struct SortClause
  {

    public ValueExpression SortField { get; private set; }

  }


  public enum SortDirection
  {
    /// <summary>正序</summary>
    Ascending,
    /// <summary>倒序</summary>
    Descending
  }
}
