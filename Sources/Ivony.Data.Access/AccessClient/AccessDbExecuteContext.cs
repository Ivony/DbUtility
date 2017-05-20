using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Common;
using System.Data.OleDb;

namespace Ivony.Data.Access.AccessClient
{
    public class AccessDbExecuteContext:DbExecuteContextBase
    {
      
        public AccessDbExecuteContext(OleDbDataReader dataReader, IDbTracing tracing, object sync)
            : base(dataReader, tracing, sync: sync)
        {
            AcccessDataReader = dataReader;
        }


        public OleDbDataReader AcccessDataReader
        {
            get;
            private set;
        }

    }
}
