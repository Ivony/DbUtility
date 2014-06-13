using Ivony.Data.Common;
using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SQLiteClient
{
  public class SQLiteExecutor : DbExecutorBase, IDbExecutor<ParameterizedQuery>
  {

    public SQLiteExecutor( string connectionString, SQLiteConfiguration configuration )
      : base( configuration )
    {
      ConnectionString = connectionString;
    }

    protected string ConnectionString
    {
      get;
      private set;
    }


  }
}
