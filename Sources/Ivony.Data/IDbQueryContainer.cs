using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  internal interface IDbQueryContainer
  {

    IDbQuery Query { get; }

  }
}
