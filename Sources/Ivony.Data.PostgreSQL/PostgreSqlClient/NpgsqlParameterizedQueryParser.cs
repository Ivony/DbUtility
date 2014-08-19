using Ivony.Data.Common;
using Npgsql;

namespace Ivony.Data.PostgreSQL.PostgreSqlClient
{
  /// <summary>
  /// 定义 PostgreSQL 参数化查询解析器。
  /// </summary>
  public class NpgsqlParameterizedQueryParser : ParameterizedQueryParser<NpgsqlCommand, NpgsqlParameter>
  {
    /// <summary>
    /// 派生类实现此方法产生一个参数对象，并生成一段占位符字符串。
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="index">参数索引位置</param>
    /// <param name="parameter">参数对象</param>
    /// <returns>参数占位符</returns>
    protected override string GetParameterPlaceholder( object value, int index, out NpgsqlParameter parameter )
    {
      var name = "@Param" + index;
      parameter = new NpgsqlParameter( name, value );

      return name;
    }

    /// <summary>
    /// 创建命令对象
    /// </summary>
    /// <param name="commandText">命令文本</param>
    /// <param name="parameters">命令参数</param>
    /// <returns>命令对象</returns>
    protected override NpgsqlCommand CreateCommand( string commandText, NpgsqlParameter[] parameters )
    {
      var command = new NpgsqlCommand
      {
        CommandText = commandText
      };

      command.Parameters.AddRange( parameters );

      return command;
    }
  }
}