using Ivony.Data.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data.MySqlClient
{
  public class MySqlExecuteContext : AsyncDbExecuteContextBase
  {

    public MySqlExecuteContext( MySqlConnection connection, MySqlDataReader dataReader )
      : base( dataReader, connection )
    {

    }

    public MySqlExecuteContext( MySqlTransactionContext transaction, MySqlDataReader dataReader )
      : base( dataReader, null )
    {

    }
  }
}
