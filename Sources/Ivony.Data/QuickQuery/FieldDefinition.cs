using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{

  /// <summary>
  /// 字段别名定义
  /// </summary>
  public class FieldDefinition
  {
    public FieldExpression Field { get; private set; }

    public string Alias { get; private set; }
  }
}
