using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Ivony.Data.SqlClient
{
  public class SqlAsyncDbResult : AsyncDbResultBase
  {

    internal SqlAsyncDbResult( SqlDataReader reader ) : base( reader ) { }

  }
}
