using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{

  /// <summary>
  /// 表示一个数据表的引用
  /// </summary>
  public class TableReference
  {

    public string TableAlias { get; set; }

    public string TableDefinition { get; set; }

  }
}
