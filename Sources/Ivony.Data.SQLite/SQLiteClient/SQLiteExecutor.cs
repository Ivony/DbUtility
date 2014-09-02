using Ivony.Data.Common;
using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SQLiteClient
{


  /// <summary>
  /// 用于操作 SQLite 的数据库访问工具
  /// </summary>
  public class SQLiteExecutor : DbHandlerBase, IDbExecutor<ParameterizedQuery>
  {

    public SQLiteExecutor( string connectionString, SQLiteConfiguration configuration )
      : base( configuration )
    {
      Configuration = configuration;

      Connection = new SQLiteConnection( connectionString );
      SyncRoot = new object();
    }

    protected SQLiteConnection Connection
    {
      get;
      private set;
    }


    protected SQLiteConfiguration Configuration
    {
      get;
      private set;
    }



    public object SyncRoot
    {
      get;
      private set;
    }



    public IDbResult Execute( ParameterizedQuery query )
    {
      var tracing = TryCreateTracing( this, query );
      var command = new SQLiteParameterizedQueryParser().Parse( query );

      return Execute( command, tracing );
    }

    private IDbResult Execute( SQLiteCommand command, IDbTracing tracing )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );


        if ( Connection.State == ConnectionState.Closed )
          Connection.Open();
        command.Connection = Connection;

        if ( Configuration.QueryExecutingTimeout.HasValue )
          command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


        var context = new SQLiteExecuteContext( command.ExecuteReader(), tracing, SyncRoot );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }

    }
  }
}
