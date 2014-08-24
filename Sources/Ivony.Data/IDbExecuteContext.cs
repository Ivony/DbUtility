using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public interface IDbExecuteContext
  {

    IDbResult Execute();

  }


  public interface IAsyncDbExecuteContext
  {
    Task<IAsyncDbResult> ExecuteAsync( CancellationToken token = default( CancellationToken ) );
  }
}
