using Ivony.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Logs
{
  public class DbTraceService : IDbTraceService
  {

    public DbTraceService( Logger logger )
    {

      Logger = logger;

    }

    protected Logger Logger
    {
      get;
      private set;
    }





    public IDbTracing CreateTracing<TQuery>( IDbExecutor<TQuery> executor, TQuery query ) where TQuery : IDbQuery
    {
      return new DbTracing( query, LogTracing );
    }

    protected virtual void LogTracing( DbTracing tracing )
    {
      Logger.LogEntry( CreateEntry( tracing ) );
    }

    protected LogEntry CreateEntry( DbTracing tracing )
    {
      if ( tracing.Exception != null )
      {

        using ( var writer = new StringWriter() )
        {

          writer.WriteLine( "执行数据查询时出现异常" );
          writer.WriteLine( "执行的查询：" );
          writer.WriteLine( tracing.QueryObject );
          writer.WriteLine( new string( '-', 30 ) );
          if ( tracing.CommandObject != null )
          {
            writer.WriteLine( "命令对象：" );
            writer.WriteLine( tracing.CommandObject );
            writer.WriteLine( new string( '-', 30 ) );
          }

          writer.WriteLine( "查询花费时间： {0}", tracing.QueryTime );
          writer.WriteLine( "异常详细信息：" );
          writer.WriteLine( tracing.Exception );
          writer.WriteLine( new string( '=', 30 ) );

          return new LogEntry( writer.ToString(), new LogMeta() { Type = LogType.Exception } );
        }
      }
      else
      {
        using ( var writer = new StringWriter() )
        {

          writer.WriteLine( "成功执行查询" );

          writer.WriteLine( "执行的查询：" );
          writer.WriteLine( tracing.QueryObject );
          writer.WriteLine( new string( '-', 30 ) );

          writer.WriteLine( "命令对象：" );
          writer.WriteLine( tracing.CommandObject );
          writer.WriteLine( new string( '-', 30 ) );


          writer.WriteLine( "数据库执行时间： {0}", tracing.ExecutionTime );
          writer.WriteLine( "查询总计花费： {0}", tracing.QueryTime );
          writer.WriteLine( new string( '=', 30 ) );

          return new LogEntry( writer.ToString(), new LogMeta() { Type = LogType.Info } );
        }
      }
    }



  }
}
