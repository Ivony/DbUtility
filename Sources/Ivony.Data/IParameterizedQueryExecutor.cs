using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public interface IParameterizedQueryExecutor<TContext> where TContext : ParameterizedQueryExecuteContext
  {

    TContext Execute( ParameterizedQuery query );

  }

}
