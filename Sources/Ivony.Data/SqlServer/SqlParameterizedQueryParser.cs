using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Ivony.Fluent;

namespace Ivony.Data.SqlServer
{

  /// <summary>
  /// 定义 SQL Server 的参数化查询解析器
  /// </summary>
  public class SqlParameterizedQueryParser : IParameterizedQueryParser<SqlCommand>
  {


    private bool _disposed = false;


    private int index = 0;
    private IDictionary<string, object> parameterList = new Dictionary<string, object>();



    private object _sync = new object();


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get { return _sync; }
    }


    /// <summary>
    /// 创建参数占位符
    /// </summary>
    /// <param name="value">参数值</param>
    /// <returns>参数占位符或参数值表达式</returns>
    public string CreateParameterPlacehold( object value )
    {
      if ( _disposed )
        throw new ObjectDisposedException( "SqlParameterizedQueryParser" );


      var name = "@Param" + index++;
      parameterList.Add( name, value );

      return name;
    }


    /// <summary>
    /// 创建 SqlCommand 对象
    /// </summary>
    /// <param name="commandText">查询文本</param>
    /// <returns>SqlCommand 对象</returns>
    public SqlCommand CreateCommand( string commandText )
    {
      if ( _disposed )
        throw new ObjectDisposedException( "SqlParameterizedQueryParser" );


      var command = new SqlCommand();
      command.CommandText = commandText;
      parameterList.ForAll( pair => command.Parameters.AddWithValue( pair.Key, pair.Value ) );
      _disposed = true;

      return command;
    }


    /// <summary>
    /// 销毁此对象，释放所有托管和非托管资源
    /// </summary>
    public void Dispose()
    {
      _disposed = true;
    }
  }
}
