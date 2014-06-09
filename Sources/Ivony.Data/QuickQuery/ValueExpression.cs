using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{
  public abstract class ValueExpression : Expression
  {
    public virtual Type ValueType
    {
      get { return typeof( object ); }
    }
  }
}
