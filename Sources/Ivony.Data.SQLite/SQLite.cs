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


    /// <summary>
    /// 根据连接字符串连接到指定的 SQLite 服务器
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">SQLite 数据库配置</param>
    /// <returns>SQLite 数据库访问器</returns>
    public static SQLiteExecutor Connect( string connectionString, SQLiteConfiguration configuration = null )
    {
      return new SQLiteExecutor( connectionString, configuration );
    }


    /// <summary>
    /// 连接到指定的数据库文件
    /// </summary>
    /// <param name="filepath">数据库文件路径</param>
    /// <param name="create">如果数据库文件不存在，是否自动创建（默认为true）</param>
    /// <param name="configuration">SQLite 数据库配置</param>
    /// <returns>SQLite 数据库访问器</returns>
    public static SQLiteExecutor ConnectFile( string filepath, bool create = true, SQLiteConfiguration configuration = null )
    {

      if ( !File.Exists( filepath ) )
      {
        if ( create )
        {
          Directory.CreateDirectory( Path.GetDirectoryName( filepath ) );
          SQLiteConnection.CreateFile( filepath );
        }

        else
          throw new FileNotFoundException( "要连接的数据库文件不存在" );
      }

      var builder = new SQLiteConnectionStringBuilder();
      builder.DataSource = filepath;

      return Connect( builder.ConnectionString, configuration ?? DefaultConfiguration );
    }



    /// <summary>
    /// 创建一个空白的数据库文件并连接
    /// </summary>
    /// <param name="filepath">文件路径</param>
    /// <param name="overwrite">若文件已经存在是否覆盖（默认为false）</param>
    /// <param name="configuration">SQLite 数据库配置</param>
    /// <returns>SQLite 数据库访问器</returns>
    public static SQLiteExecutor ConnectNewFile( string filepath, bool overwrite = false, SQLiteConfiguration configuration = null )
    {

      if ( File.Exists( filepath ) )
      {
        if ( overwrite )
          File.Delete( filepath );

        else
          throw new InvalidOperationException( "文件已存在" );
      }

      SQLiteConnection.CreateFile( filepath );

      return ConnectFile( filepath, false, configuration );
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
