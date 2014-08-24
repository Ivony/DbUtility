using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public interface IDbExecutorProvider<TExecutor>
  {
    TExecutor DbExecutor { get; }
  }
}
