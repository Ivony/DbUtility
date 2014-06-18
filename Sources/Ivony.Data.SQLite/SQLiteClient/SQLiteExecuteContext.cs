using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Ivony.Data.SQLiteClient
{
  class SQLiteExecuteContext : DbExecuteContextBase
  {
    public SQLiteExecuteContext( SQLiteDataReader dataReader, IDbTracing tracing, object sync )
      : base( dataReader, tracing, sync: sync )
    {
      DataReader = dataReader;
    }


    public new SQLiteDataReader DataReader
    {
      get;
      private set;
    }
  }
}
