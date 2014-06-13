using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 定义可以作为参数化查询的片段的对象
  /// </summary>
  public interface IParameterizedQueryPartial
  {
    /// <summary>
    /// 将该片段添加到正在构建的参数化查询的末尾
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    void AppendTo( ParameterizedQueryBuilder builder );
  }
}
