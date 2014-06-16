using Ivony.Data.SQLiteClient;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public static class SQLite
  {


    public static SQLiteExecutor CreateDatabase( string filepath, bool overwrite = false, SQLiteConfiguration configuration = null )
    {

      if ( File.Exists( filepath ) )
      {
        if ( overwrite )
          File.Delete( filepath );

        else
          throw new InvalidOperationException( "文件已存在" );
      }

      SQLiteConnection.CreateFile( filepath );

      return ConnectFile( filepath,, false, configuration );
    }

    public static SQLiteExecutor Connect( string connectionString, SQLiteConfiguration configuration = null )
    {
      return new SQLiteExecutor( connectionString, configuration );
    }


    public static SQLiteExecutor ConnectFile( string filepath, bool create = true, SQLiteConfiguration configuration = null )
    {

      if ( !File.Exists( filepath ) )
      {
        if ( create )
          SQLiteConnection.CreateFile( filepath );

        else
          throw new InvalidOperationException( "要连接的数据库文件不存在" );
      }

      var builder = new SQLiteConnectionStringBuilder();
      builder.DataSource = filepath;

      return Connect( builder.ConnectionString, configuration ?? DefaultConfiguration );
    }



    static SQLite()
    {
      DefaultConfiguration = new SQLiteConfiguration();
    }

    public static SQLiteConfiguration DefaultConfiguration
    {
      get;
      private set;


    }

  }
}
