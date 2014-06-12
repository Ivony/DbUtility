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
  public class MySqlDbExecutor : DbExecutorBase, IDbExecutor<ParameterizedQuery>, IDbTransactionProvider<MySqlDbExecutor>
  {



    public MySqlDbExecutor( string connectionString, MySqlDbConfiguration configuration )
      : base( configuration )
    {

      if ( connectionString == null )
        throw new ArgumentNullException( "connectionString" );

      if ( configuration == null )
        throw new ArgumentNullException( "configuration" );

      ConnectionString = connectionString;
      Configuration = configuration;

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

    public IDbExecuteContext Execute( ParameterizedQuery query )
    {

      return Execute( CreateCommand( query ), TryCreateTracing( this, query ) );

    }

    protected virtual IDbExecuteContext Execute( MySqlCommand command, IDbTracing tracing )
    {
      TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

      var connection = new MySqlConnection( ConnectionString );
      connection.Open();
      command.Connection = connection;

      var context = new MySqlExecuteContext( connection, command.ExecuteReader(), tracing );

      TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

      return context;
    }



    private MySqlCommand CreateCommand( ParameterizedQuery query )
    {

      return new MySqlParameterizedQueryParser().Parse( query );
    }


    IDbTransactionContext<MySqlDbExecutor> IDbTransactionProvider<MySqlDbExecutor>.CreateTransaction()
    {
      return new MySqlDbTransactionContext( ConnectionString, Configuration );
    }
  }
}
