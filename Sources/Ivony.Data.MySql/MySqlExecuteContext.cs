using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public class MySqlExecuteContext  : AsyncDbExecuteContextBase
  {
    public MySqlExecuteContext( MySqlConnection connection, MySqlDataReader dataReader ) : base( dataReader, connection )
    {

    }
  }
}
