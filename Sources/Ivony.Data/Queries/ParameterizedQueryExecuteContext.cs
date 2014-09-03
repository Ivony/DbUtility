using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  public interface IParameterizedQueryExecuteContext : IDbExecuteContext
  {

    ParameterizedQuery Query { get; }

  }
}
