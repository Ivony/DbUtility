using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  
  /// <summary>
  /// 定义参数化查询解析器，解析器可以将参数化查询实例解析为指定命令对象
  /// </summary>
  /// <typeparam name="TCommand">命令对象类型</typeparam>
  public interface IParameterizedQueryParser<TCommand> : IDisposable
  {

    /// <summary>
    /// 获取参数占位符表达式，系统调用此方法将制定参数值转换为参数占位符
    /// </summary>
    /// <param name="parameterValue">参数值</param>
    /// <returns>参数占位符</returns>
    string CreateParameterPlacehold( object parameterValue );


    /// <summary>
    /// 创建命令对象
    /// </summary>
    /// <param name="commandText">查询命令文本</param>
    /// <returns>命令对象</returns>
    TCommand CreateCommand( string commandText );


    /// <summary>
    /// 用于同步的对象
    /// </summary>
    object SyncRoot
    {
      get;
    }

  }
}
