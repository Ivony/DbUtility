using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Ivony.Data
{


  public interface IDbExecutableQuery
  {
    IDbExecuteContext Execute();

    Task<IDbExecuteContext> ExecuteAsync();

  }


}
