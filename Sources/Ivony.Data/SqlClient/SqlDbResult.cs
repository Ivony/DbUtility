using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{
  public sealed class SqlDbResult : DbResultBase
  {

    internal SqlDbResult( SqlDataReader reader ) : base( reader ) { }

  }
}
