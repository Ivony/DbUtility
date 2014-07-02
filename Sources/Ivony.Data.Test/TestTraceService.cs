using Ivony.Data.Common;
using Ivony.Logs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Test
{
  public class TestTraceService : IDbTraceService, IEnumerable<DbTracing>
  {


    IList<DbTracing> _list = new List<DbTracing>();


    public IDbTracing CreateTracing<TQuery>( IDbExecutor<TQuery> executor, TQuery query ) where TQuery : IDbQuery
    {
      var tracing = new DbTracing( query );
      _list.Add( tracing );
      return tracing;

    }




    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    IEnumerator<DbTracing> IEnumerable<DbTracing>.GetEnumerator()
    {
      return _list.GetEnumerator();
    }
  }
}
