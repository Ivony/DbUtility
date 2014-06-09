using Ivony.Data.DatabaseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{

  /// <summary>
  /// 代表一个字段值
  /// </summary>
  public class FieldExpression : ValueExpression
  {

    public FieldExpression( DataField field )
    {
      Field = field;
    }



    public DataField Field { get; private set; }

    public override Type ValueType { get { return Field.ValueType; } }

  }
}
