using Ivony.Data;
using Ivony.Data.Common;
using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ivony.Data.SqlClient
{

  /// <summary>
  /// 定义 SQL Server 的参数化查询解析器
  /// </summary>
  public class SqlParameterizedQueryParser : ParameterizedQueryParser<SqlCommand, SqlParameter>
  {



    private class ParameterSpecification
    {
      public SqlDbType Type { get; set; }

      public int Size { get; set; }

    }


    private static Dictionary<Type, ParameterSpecification> _parameterRules = new Dictionary<Type, ParameterSpecification>();

    private static object _sync = new object();

    /// <summary>
    /// 注册一个参数规范
    /// </summary>
    /// <param name="valueType">所适用的值类型</param>
    /// <param name="type">数据库参数类型</param>
    /// <param name="size">参数精度</param>
    public static void RegisterParameterSpecification( Type valueType, SqlDbType type, int size = 0 )
    {
      lock ( _sync )
      {
        _parameterRules[valueType] = new ParameterSpecification { Type = type, Size = size };
      }

    }


    /// <summary>
    /// 解除注册一个参数规范
    /// </summary>
    /// <param name="valueType">所适用的值类型</param>
    public static void UnregisterParameterSpecification( Type valueType )
    {
      lock ( _sync )
      {
        _parameterRules.Remove( valueType );
      }
    }



    /// <summary>
    /// 创建参数并获取参数占位符
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="index">在模板中参数的位置顺序</param>
    /// <param name="parameter">创建的参数对象</param>
    /// <returns>参数占位符</returns>
    protected override string GetParameterPlaceholder( object value, int index, out SqlParameter parameter )
    {
      var name = "@Param" + index;
      parameter = CreateParameter( name, value );

      return name;
    }


    /// <summary>
    /// 创建参数对象
    /// </summary>
    /// <param name="name">参数名</param>
    /// <param name="value">参数值</param>
    /// <returns>参数对象</returns>
    protected SqlParameter CreateParameter( string name, object value )
    {
      if ( value != null )
      {
        var valueType = value.GetType();

        lock ( _sync )
        {
          foreach ( var type in _parameterRules.Keys )
          {
            if ( type.IsAssignableFrom( valueType ) )
            {
              var rule = _parameterRules[type];

              return new SqlParameter( name, rule.Type, rule.Size ) { Value = value };
            }
          }
        }
      }

      return new SqlParameter( name, value );
    }


    /// <summary>
    /// 创建 SqlCommand 对象
    /// </summary>
    /// <param name="commandText">经过分析后的 SQL 查询文本</param>
    /// <param name="parameters">参数值列表</param>
    /// <returns>用于执行查询的 SqlCommand 对象</returns>
    protected override SqlCommand CreateCommand( string commandText, SqlParameter[] parameters )
    {
      var command = new SqlCommand();

      command.CommandText = commandText;
      command.Parameters.AddRange( parameters );

      return command;
    }
  }
}
