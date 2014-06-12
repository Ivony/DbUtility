using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{

  /// <summary>
  /// 定义 SQL Server 版本信息
  /// </summary>
  public sealed class SqlServerVersion
  {

    public bool AllowLocalDB { get; private set; }

    public string LocalDBInstanceName { get; private set; }





    private static SqlServerVersion sql2008 = new SqlServerVersion()
    {
      AllowLocalDB = true,
      LocalDBInstanceName = "11.0"
    };

    public static SqlServerVersion SqlServer2008 { get { return sql2008; } }


  }
}
