using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 模板解析上下文
  /// </summary>
  public abstract class TemplateParseContext
  {

    /// <summary>
    /// 创建参数 SQL 表达式
    /// </summary>
    /// <param name="parameter">参数表达式</param>
    /// <returns>该参数在 SQL 语句中引用的形式</returns>
    public abstract string CreateParameterExpression( ParameterExpression parameter );

  }
}
