using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data.Common
{
  internal interface IDbQueryContainer
  {

    IDbQuery Query { get; }

  }
}
