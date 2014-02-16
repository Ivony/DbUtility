using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public interface IDbTransactionContext<out TDbExecutor> : IDisposable
  {

    void Commit();

    void Rollback();

    TDbExecutor DbExecutor { get; }

    object SyncRoot { get; }
  }
}
