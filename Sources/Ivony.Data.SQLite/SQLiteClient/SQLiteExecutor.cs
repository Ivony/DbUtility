using Ivony.Data.Common;
using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SQLiteClient
{
  public class SQLiteExecutor : DbExecutorBase, IDbExecutor<ParameterizedQuery>
  {

    public SQLiteExecutor( string connectionString, SQLiteConfiguration configuration )
      : base( configuration )
    {
      ConnectionString = connectionString;
    }

    protected string ConnectionString
    {
      get;
      private set;
    }



    public IDbExecuteContext Execute( ParameterizedQuery query )
    {
      var command = new SQLiteParameterizedQueryParser().Parse( query );

      return Execute( command, TryCreateTracing( this, query ) );
    }

    private IDbExecuteContext Execute( SQLiteCommand command, IDbTracing tracing )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

        var connection = new SQLiteConnection( ConnectionString );
        connection.Open();
        command.Connection = connection;

        var context = new SQLiteExecuteContext( connection, command.ExecuteReader(), tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }

    }
  }
}
