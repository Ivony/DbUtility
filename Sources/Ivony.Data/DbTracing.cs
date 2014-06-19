using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public class DbTracing : IDbTracing
  {


    public DbTracing( IDbQuery query ) : this( query, null ) { }


    public DbTracing( IDbQuery query, Action<DbTracing> complateCallback )
    {

      QueryObject = query;

      ExecutionTime = TimeSpan.Zero;
      QueryTime = TimeSpan.Zero;


      callback = complateCallback;
    }




    private Stopwatch executionStopwatch = new Stopwatch();
    private Stopwatch queryStopwatch = new Stopwatch();



    public IDbQuery QueryObject
    {
      get;
      private set;
    }

    public object CommandObject
    {
      get;
      private set;
    }



    public TimeSpan ExecutionTime
    {
      get;
      private set;
    }

    public TimeSpan QueryTime
    {
      get;
      private set;
    }

    public Exception Exception
    {
      get;
      private set;
    }


    private Action<DbTracing> callback;


    void IDbTracing.OnExecuting( object commandObject )
    {
      executionStopwatch.Restart();
      queryStopwatch.Restart();
    }

    void IDbTracing.OnLoadingData( IDbExecuteContext context )
    {
      executionStopwatch.Stop();
      ExecutionTime = executionStopwatch.Elapsed;
    }

    void IDbTracing.OnComplete()
    {
      queryStopwatch.Stop();
      QueryTime = queryStopwatch.Elapsed;

      if ( callback != null )
        callback( this );
    }

    void IDbTracing.OnException( Exception exception )
    {
      executionStopwatch.Stop();
      queryStopwatch.Stop();

      QueryTime = queryStopwatch.Elapsed;

      Exception = exception;

      if ( callback != null )
        callback( this );
    }
  }
}
