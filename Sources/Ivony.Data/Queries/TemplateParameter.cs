using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 参数表达式
  /// </summary>
  public class TemplateParameter : ITemplatePartial
  {
    /// <summary>
    /// 创建 ParameterExpression 实例
    /// </summary>
    /// <param name="value">参数值</param>
    public TemplateParameter( object value )
    {
      Value = value;
    }

    /// <summary>
    /// 参数值
    /// </summary>
    public object Value
    {
      get;
      private set;
    }

    /// <summary>
    /// 数据类型，可选
    /// </summary>
    public DbType? DbType
    {
      get;
      private set;
    }

    /// <summary>
    /// 解析成参数表达式
    /// </summary>
    /// <param name="context">模版解析上下文</param>
    /// <returns>需要嵌入在模版中的形式</returns>
    public string Parse( TemplateParseContext context )
    {
      return context.CreateParameterExpression( this );
    }
  }

}
