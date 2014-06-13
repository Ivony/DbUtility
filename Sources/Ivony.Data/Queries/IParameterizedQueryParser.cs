using System;


namespace Ivony.Data.Queries
{

  /// <summary>
  /// 定义参数化查询解析器
  /// </summary>
  /// <typeparam name="TCommand">解析完成的命令对象的类型</typeparam>
  public interface IParameterizedQueryParser<TCommand>
  {
    /// <summary>
    /// 创建查询命令对象
    /// </summary>
    /// <param name="query">参数化查询</param>
    /// <returns>查询命令对象</returns>
    TCommand Parse( ParameterizedQuery query );
  }
}
