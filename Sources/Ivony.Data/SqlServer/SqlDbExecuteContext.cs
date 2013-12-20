using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SqlServer
{
  public class SqlDbExecuteContext : IDbExecuteContext
  {

    public SqlDbExecuteContext( SqlConnection connection, SqlDataReader reader )
    {
      Connection = connection;
      DataReader = reader;
    }


    public IDataReader DataReader
    {
      get;
      private set;
    }


    public SqlConnection Connection
    {
      get;
      private set;
    }

    public void Dispose()
    {
      Connection.Close();
    }
  }
}
