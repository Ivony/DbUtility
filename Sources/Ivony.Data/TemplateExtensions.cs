using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using Ivony.Data.Queries;

namespace Ivony.Data
{

  /// <summary>
  /// 有关模板的扩展方法
  /// </summary>
  public static class TemplateExtensions
  {

    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static TemplateQuery Template( this IDbExecutor<TemplateQuery> executor, string template, params object[] parameters )
    {
      return new TemplateQuery( executor, template, parameters );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static TemplateQuery T( this IDbExecutor<TemplateQuery> executor, string template, params object[] parameters )
    {
      return Template( executor, template, parameters );
    }



  }
}
