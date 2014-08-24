using Ivony.Data.Common;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Ivony.Data.MySqlClient
{
  /// <summary>
  /// 用于操作 MySQL 的数据库访问工具
  /// </summary>
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

    public IDbResult Execute( ParameterizedQuery query )
    {

      return Execute( CreateCommand( query ), TryCreateTracing( this, query ) );

    }

    protected virtual IDbResult Execute( MySqlCommand command, IDbTracing tracing )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );


        var connection = new MySqlConnection( ConnectionString );
        connection.Open();
        command.Connection = connection;

        if ( Configuration.QueryExecutingTimeout.HasValue )
          command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;

        var context = new MySqlExecuteContext( connection, command.ExecuteReader(), tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
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
