using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public class MySqlExecuteContext  : IDbExecuteContext
  {
    private   MySql.Data.MySqlClient.MySqlConnection connection;
    private   MySql.Data.MySqlClient.MySqlDataReader mySqlDataReader;

    public MySqlExecuteContext( MySql.Data.MySqlClient.MySqlConnection connection, MySql.Data.MySqlClient.MySqlDataReader mySqlDataReader )
    {
      // TODO: Complete member initialization
      this.connection = connection;
      this.mySqlDataReader = mySqlDataReader;
    }
    public DataTable LoadDataTable( int startRecord, int maxRecords )
    {
      throw new NotImplementedException();
    }

    public bool NextResult()
    {
      throw new NotImplementedException();
    }

    public int RecordsAffected
    {
      get { throw new NotImplementedException(); }
    }

    public IDataRecord ReadRecord()
    {
      throw new NotImplementedException();
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}
