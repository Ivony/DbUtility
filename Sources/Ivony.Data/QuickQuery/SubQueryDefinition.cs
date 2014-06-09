using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{

  /// <summary>
  /// 子查询定义
  /// </summary>
  public class SubQueryDefinition
  {

    public Ivony.Data.Queries.QuickQuery Query { get; private set; }

    public string Alias { get; private set; }



  }
}
