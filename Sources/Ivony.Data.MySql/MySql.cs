using Ivony.Data.Common;
using Ivony.Data.MySqlClient;
using Ivony.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public static class MySql
  {

    public static MySqlDbUtility FromConfiguration( string name, MySqlDbConfiguration configuration = null )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return Create( setting.ConnectionString, configuration );
    }


    public static MySqlDbUtility Create( string connectionString, MySqlDbConfiguration configuration = null )
    {
      return new MySqlDbUtility( connectionString, configuration ?? DefaultConfiguration );
    }



    public static MySqlDbUtility Create( SqlConnectionStringBuilder buildler, MySqlDbConfiguration configuration = null )
    {
      return Create( buildler.ConnectionString, configuration );
    }



    public static MySqlDbUtility Create( string server, string database = null, bool? integratedSecurity = null, string userID = null, string password = null, string attachDbFilename = null, bool? pooling = null, uint? maximunPoolSize = null, uint? minimunPoolSize = null, MySqlDbConfiguration configuration = null )
    {
      var builder = new MySqlConnectionStringBuilder();

      builder.Server = server;
      if ( userID != null )
        builder.UserID = userID;

      if ( password != null )
        builder.Password = password;

      if ( integratedSecurity != null )
        builder.IntegratedSecurity = integratedSecurity.Value;

      if ( database != null )
        builder.Database = database;

      if ( pooling != null )
        builder.Pooling = pooling.Value;

      if ( maximunPoolSize != null )
        builder.MaximumPoolSize = maximunPoolSize.Value;

      if ( minimunPoolSize != null )
        builder.MinimumPoolSize = minimunPoolSize.Value;

      return Create( builder.ConnectionString, configuration );



    }


    public static MySqlDbConfiguration DefaultConfiguration
    {
      get;
      set;
    }


  }
}
