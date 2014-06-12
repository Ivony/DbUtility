using Ivony.Data.Common;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.MySqlClient
{
  public class MySqlDbUtility : IDbExecutor<ParameterizedQuery>
  {



    public MySqlDbUtility( string connectionString, MySqlDbConfiguration configuration )
    {

      if ( connectionString == null )
        throw new ArgumentNullException( "connectionString" );

      if ( configuration == null )
        throw new ArgumentNullException( "configuration" );

      ConnectionString = connectionString;
      Configuration = configuration;
      TraceService = configuration.TraceService ?? BlankTraceService.Instance;

    }


    protected string ConnectionString
    {
      get;
      private set;
    }



    protected MySqlDbConfiguration Configuration
    {
      get;
      private set;
    }

    protected IDbTraceService TraceService
    {
      get;
      private set;
    }

    public IDbExecuteContext Execute( ParameterizedQuery query )
    {

      return Execute( CreateCommand( query ) );

    }

    protected virtual IDbExecuteContext Execute( MySqlCommand command )
    {
      var connection = new MySqlConnection( ConnectionString );
      connection.Open();
      command.Connection = connection;

      return new MySqlExecuteContext( connection, command.ExecuteReader() );
    }


    private MySqlCommand CreateCommand( ParameterizedQuery query )
    {
      return new MySqlParameterizedQueryParser().Parse( query );
    }
  }
}
