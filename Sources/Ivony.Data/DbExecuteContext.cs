using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 查询执行上下文
  /// </summary>
  public abstract class DbExecuteContext : IDisposable
  {

    public abstract IDataReader DataReader { get; }

    public abstract void Dispose();


  }
}
