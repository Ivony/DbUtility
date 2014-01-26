using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public interface IDbExecutor<T> where T : IDbQuery
  {

    IDbExecuteContext Execute( T query );

  }


  public interface IAsyncDbExecutor<T> : IDbExecutor<T> where T : IDbQuery
  {

    Task<IDbExecuteContext> ExecuteAsync( T query );


  }
}
