using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Common;

namespace Ivony.Data
{

  /// <summary>
  /// 提供 SQL Server 数据库访问支持
  /// </summary>
  public static class SqlServer
  {

    public static SqlDbUtility FromConfiguration( string name, IDbTraceService traceService = null )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return Create( setting.ConnectionString, traceService );
    }


    public static SqlDbUtility Create( string connectionString, IDbTraceService traceService = null )
    {
      return new SqlDbUtility( connectionString, traceService ?? DefaultTraceService );
    }



    public static SqlDbUtility Create( SqlConnectionStringBuilder buildler, IDbTraceService traceService = null )
    {
      return Create( buildler.ConnectionString, traceService );
    }



    public static SqlDbUtility Create( string dataSource, string initialCatalog = null, bool? integratedSecurity = null, string userID = null, string password = null, string attachDbFilename = null, bool? pooling = null, int? maxPoolSize = null, int? minPoolSize = null, IDbTraceService traceService = null )
    {
      var builder = new SqlConnectionStringBuilder();

      builder.DataSource = dataSource;
      if ( userID != null )
        builder.UserID = userID;

      if ( password != null )
        builder.Password = password;

      if ( integratedSecurity != null )
        builder.IntegratedSecurity = integratedSecurity.Value;

      if ( initialCatalog != null )
        builder.InitialCatalog = initialCatalog;

      if ( pooling != null )
        builder.Pooling = pooling.Value;

      if ( maxPoolSize != null )
        builder.MaxPoolSize = maxPoolSize.Value;

      if ( minPoolSize != null )
        builder.MaxPoolSize = minPoolSize.Value;

      if ( attachDbFilename != null )
        builder.AttachDBFilename = attachDbFilename;

      return Create( builder.ConnectionString, traceService );



    }


    public static IDbTraceService DefaultTraceService
    {
      get;
      set;
    }

  }
}
