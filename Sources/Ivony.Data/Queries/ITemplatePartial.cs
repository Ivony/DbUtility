using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 定义可以作为模板的一部分被嵌入模板的表达式
  /// </summary>
  public interface ITemplatePartial
  {
    /// <summary>
    /// 解析模版并提供嵌入的 SQL 表达式
    /// </summary>
    /// <param name="context">模版解析上下文</param>
    /// <returns></returns>
    void Parse( ParameterizedQueryBuilder builder );
  }
}
