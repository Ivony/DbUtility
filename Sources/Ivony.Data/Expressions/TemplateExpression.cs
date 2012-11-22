using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Data
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

  /// <summary>
  /// 定义可以作为模板的一部分被嵌入模板的表达式
  /// </summary>
  public interface ITemplatePartialExpression : IDbExpression
  {
    /// <summary>
    /// 解析模版并提供嵌入的 SQL 表达式
    /// </summary>
    /// <param name="context">模版解析上下文</param>
    /// <returns></returns>
    string Parse( TemplateParseContext context );

  }

  /// <summary>
  /// 模板表达式
  /// </summary>
  public class TemplateExpression : ITemplatePartialExpression
  {
    /// <summary>
    /// 创建 TemplateExpression 对象
    /// </summary>
    /// <param name="template">模版</param>
    /// <param name="parameters">参数列表</param>
    public TemplateExpression( string template, params object[] parameters )
    {

      Template = template.Replace( "{...}", ParseParameterListSymbol( parameters.Length ) );

      Parameters = parameters.Select( item => 
      {
        var partial = item as ITemplatePartialExpression;
        if ( partial == null )
          partial = new ParameterExpression( item );
        return partial;
      }).ToArray();
    }

    /// <summary>
    /// 模版
    /// </summary>
    public string Template
    {
      get;
      private set;
    }

    /// <summary>
    /// 参数列表
    /// </summary>
    public ITemplatePartialExpression[] Parameters
    {
      get;
      private set;
    }

    /// <summary>
    /// 解析模版表达式
    /// </summary>
    /// <param name="context">模版解析上下文</param>
    /// <returns>需要嵌入在模版中的形式</returns>
    public string Parse( TemplateParseContext context )
    {
      return FormatRegexNum.Replace( Template, delegate( Match match )
      {
        string format = match.Groups["format"].ToString();
        int index = int.Parse( match.Groups["index"].ToString() );

        if ( index >= Parameters.Length )
          throw new IndexOutOfRangeException();

        ITemplatePartialExpression partial = Parameters[index];

        return partial.Parse( context );
      }
      );
    }


    /// <summary>
    /// 处理参数列表表达式“{...}”
    /// </summary>
    /// <param name="amount">参数个数</param>
    /// <returns></returns>
    private static string ParseParameterListSymbol( int amount )
    {
      StringBuilder builder = new StringBuilder();

      bool begin = true;
      for ( int i = 0; i < amount; i++ )
      {
        if ( !begin )
          builder.Append( " , " );
        builder.Append( "{" + i + "}" );
        begin = false;
      }

      return builder.ToString();
    }


    internal static readonly Regex FormatRegexNum = new Regex( @"\{(?<index>[0-9]*)(,(?<alignment>[0-9]+[a-zA-Z]*))?(\:(?<format>[^{}]*))?\}", RegexOptions.Compiled );
  }

  /// <summary>
  /// 参数表达式
  /// </summary>
  public class ParameterExpression : ITemplatePartialExpression
  {
    /// <summary>
    /// 创建 ParameterExpression 实例
    /// </summary>
    /// <param name="value">参数值</param>
    public ParameterExpression( object value )
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
