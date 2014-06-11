using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public class MySqlDbUtility : IDbExecutor<ParameterizedQuery>
  {



    public MySqlDbUtility( string connectionString, IDbTraceService traceService = null )
    {

      ConnectionString = connectionString;
      TraceService = traceService ?? BlankTraceService.Instance;

    }


    protected string ConnectionString
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

      var command = CreateCommand( query );
      var connection = CreateConnection();
      command.Connection = connection;
      connection.Open();

      return new MySqlExecuteContext( connection, command.ExecuteReader() );

    }

    protected MySqlConnection CreateConnection()
    {
      return new MySqlConnection( ConnectionString );
    }

    private MySqlCommand CreateCommand( ParameterizedQuery query )
    {
      return new MySqlParameterizedQueryParser().Parse( query );
    }
  }
}
