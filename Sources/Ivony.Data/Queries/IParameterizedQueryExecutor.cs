using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  public interface IParameterizedQueryExecutor<TContext> where TContext : IParameterizedQueryExecuteContext
  {

    TContext Execute( ParameterizedQuery query );

  }

}
