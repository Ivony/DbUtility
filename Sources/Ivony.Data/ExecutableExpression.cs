using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public class ExecutableExpression
  {

    public IDbExpression Expression
    {
      get;
      private set;
    }

    public DbUtility DbUtility
    {
      get;
      private set;
    }
  }

}
