using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Ivony.Data.SQLiteClient
{
  class SQLiteExecuteContext : AsyncDbExecuteContextBase
  {
    public SQLiteExecuteContext( SQLiteConnection connection, SQLiteDataReader dataReader, IDbTracing tracing )
      : base( dataReader, connection, tracing )
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
