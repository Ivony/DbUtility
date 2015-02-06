using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{

  /// <summary>
  /// 代表一个参数化查询
  /// </summary>
  public class ParameterizedQuery : IDbQuery, IParameterizedQueryPartial
  {

    /// <summary>
    /// 定义匹配参数占位符的正则表达式
    /// </summary>
    public static readonly Regex ParameterPlaceholdRegex = new Regex( @"#(?<index>[0-9]+)#", RegexOptions.Compiled );



    /// <summary>
    /// 获取查询文本模板
    /// </summary>
    public string TextTemplate
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取参数值
    /// </summary>
    public object[] ParameterValues
    {
      get;
      private set;
    }



    /// <summary>
    /// 构建参数化查询对象
    /// </summary>
    /// <param name="template">查询文本模板</param>
    /// <param name="values">参数值</param>
    public ParameterizedQuery( string template, object[] values )
    {

      if ( template == null )
        throw new ArgumentNullException( "template" );

      if ( values == null )
        throw new ArgumentNullException( "values" );

      TextTemplate = template;
      ParameterValues = new object[values.Length];
      values.CopyTo( ParameterValues, 0 );
    }


    /// <summary>
    /// 将参数化查询解析为另一个参数化查询的一部分。
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    public void AppendTo( ParameterizedQueryBuilder builder )
    {

      int index = 0;

      foreach ( Match match in ParameterPlaceholdRegex.Matches( TextTemplate ) )
      {

        var length = match.Index - index;
        if ( length > 0 )
          builder.AppendText( TextTemplate.Substring( index, length ) );


        var parameterIndex = int.Parse( match.Groups["index"].Value );
        builder.AppendParameter( ParameterValues[parameterIndex] );

        index = match.Index + match.Length;
      }

      builder.AppendText( TextTemplate.Substring( index, TextTemplate.Length - index ) );
    }



    /// <summary>
    /// 重写 ToString 方法，输出参数化查询的字符串表达形式
    /// </summary>
    /// <returns>字符串表达形式</returns>
    public override string ToString()
    {
      if ( stringExpression == null )
        stringExpression = GetString();

      return stringExpression;
    }

    private string stringExpression;

    private string GetString()
    {
      var writer = new StringWriter();

      writer.WriteLine( "\"" + TextTemplate.Replace( "\"", "\"\"" ) + "\"" );
      writer.WriteLine();

      for ( int i = 0; i < this.ParameterValues.Length; i++ )
      {
        writer.WriteLine( "#{0}#: {1}", i, ParameterValues[i] );
      }

      return writer.ToString();
    }




    /// <summary>
    /// 提供从字符串到 ParameterizedQuery 的隐式类型转换
    /// </summary>
    /// <param name="text">要转换为参数化查询的查询字符串</param>
    /// <returns>参数化查询对象</returns>
    public static implicit operator ParameterizedQuery( string text )
    {
      if ( text == null )
        return null;
      
      return TemplateParser.ParseTemplate( text, new object[0] );
    }


    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery operator +( ParameterizedQuery query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static DbExecutableQuery<ParameterizedQuery> operator +( DbExecutableQuery<ParameterizedQuery> query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> operator +( AsyncDbExecutableQuery<ParameterizedQuery> query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }


    internal bool IsStartWithWhiteSpace()
    {
      return char.IsWhiteSpace( TextTemplate[0] );
    }
  }
}
