using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public interface IDbProvider
  {

    IDbExecutor<T> GetDbExecutor<T>( string connectionString ) where T : IDbQuery;

  }
}
