using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{
  public abstract class BooleanExpression : Expression
  {
    public override Type ValueType
    {
      get { return typeof( bool ); }
    }
  }
}
