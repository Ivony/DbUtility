using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{


  /// <summary>
  /// 模板表达式
  /// </summary>
  public class TemplateQuery : DbQuery
  {


    internal static readonly Regex FormatRegexNum = new Regex( @"\{(?<index>[0-9]+)\}", RegexOptions.Compiled );


    protected IDbExecutor<TemplateQuery> DbExecutor
    {
      get;
      private set;
    }



    public override IDbExecuteContext Execute()
    {
      return DbExecutor.Execute( this );
    }

    public override Task<IDbExecuteContext> ExecuteAsync()
    {
      var asyncExecutor = DbExecutor as IAsyncDbExecutor<TemplateQuery>;
      if ( asyncExecutor != null )
        return asyncExecutor.ExecuteAsync( this );

      return new Task<IDbExecuteContext>( Execute );
    }


    /// <summary>
    /// 创建 TemplateExpression 对象
    /// </summary>
    /// <param name="template">模版</param>
    /// <param name="parameters">参数列表</param>
    public TemplateQuery( IDbExecutor<TemplateQuery> executor, string template, params object[] parameters )
    {
      DbExecutor = executor;


      Template = template.Replace( "{...}", ParseParameterListSymbol( parameters.Length ) );

      Parameters = parameters.Select( item =>
      {
        var partial = item as ITemplatePartial;
        if ( partial == null )
          partial = new TemplateParameter( item );
        
        return partial;
      } ).ToArray();

      var indexes = FormatRegexNum.Matches( Template ).Cast<Match>().Select( m => int.Parse( m.Groups["index"].Value ) );
      if ( indexes.Any() && indexes.Max() >= Parameters.Length )
        throw new IndexOutOfRangeException();
    }

    /// <summary>
    /// 模版
    /// </summary>
    protected string Template
    {
      get;
      private set;
    }

    /// <summary>
    /// 参数列表
    /// </summary>
    protected ITemplatePartial[] Parameters
    {
      get;
      private set;
    }




    /// <summary>
    /// 解析参数列表表达式“{...}”
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



    public ParameterizedQuery CreateQuery()
    {
      throw new NotImplementedException();
    }






    /// <summary>
    /// 将多个 TemplateExpression 拼接在一起
    /// </summary>
    /// <param name="expressions">要拼接的 Expression</param>
    /// <returns>拼接后的 Expression</returns>
    public static TemplateQuery Concat( params TemplateQuery[] templates )
    {

      var parameters = new ArrayList();
      var template = new StringBuilder();
      var offset = 0;

      IDbExecutor<TemplateQuery> dbUtility = null;

      foreach ( var e in templates )
      {
        if ( dbUtility == null )
          dbUtility = e.DbExecutor;

        else if ( !dbUtility.Equals( e.DbExecutor ) )
          throw new InvalidOperationException();

        parameters.AddRange( e.Parameters );
        template.Append( OffsetParameterIndex( e.Template, offset ) );

        offset += e.Parameters.Length;
      }

      return new TemplateQuery( dbUtility, template.ToString(), parameters.ToArray() );
    }


    /// <summary>
    /// 将所有参数序号推移指定的偏移量
    /// </summary>
    /// <param name="template">要处理的字符串模板</param>
    /// <param name="offset">要推移的偏移量</param>
    /// <returns>处理后的字符串模板</returns>
    private static string OffsetParameterIndex( string template, int offset )
    {

      if ( offset == 0 )
        return template;

      else if ( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset" );


      return FormatRegexNum.Replace( template, delegate( Match match )
      {
        var index = int.Parse( match.Groups["index"].Value );
        index += offset;
        return "{" + index + "}";
      } );

    }

    /// <summary>
    /// 将两个 TemplateExpression 拼接在一起
    /// </summary>
    /// <param name="template1">第一个要拼接的 TemplateExpression</param>
    /// <param name="template2">第二个要拼接的 TemplateExpression</param>
    /// <returns>拼接好的 TemplateExpression</returns>
    public static TemplateQuery operator +( TemplateQuery template1, TemplateQuery template2 )
    {
      return Concat( template1, template2 );
    }


    /// <summary>
    /// 将多个 TemplateExpression 拼接在一起，并使用指定的字符串分隔
    /// </summary>
    /// <param name="separator">用于分隔各个 TemplateExpression 的字符串</param>
    /// <param name="templates">要拼合的模板表达式</param>
    /// <returns></returns>
    public static TemplateQuery Join( string separator, params TemplateQuery[] templates )
    {
      if ( templates.Length == 0 )
        return null;

      return Join( new TemplateQuery( templates[0].DbExecutor, separator ), templates );
    }

    /// <summary>
    /// 将多个 TemplateExpression 拼接在一起，并使用指定的 TemplateExpression 分隔
    /// </summary>
    /// <param name="separator">用于分隔各个 TemplateExpression 的字符串</param>
    /// <param name="expressions">要拼合的模板表达式</param>
    /// <returns></returns>
    public static TemplateQuery Join( TemplateQuery separator, params TemplateQuery[] expressions )
    {

      if ( expressions.Length == 0 )
        return null;

      if ( expressions.Length == 1 )
        return expressions[0];

      var set = new TemplateQuery[expressions.Length + expressions.Length - 1];
      for ( int i = 0; i < expressions.Length; i++ )
        set[i * 2] = expressions[i];

      for ( int i = 1; i < set.Length; i += 2 )
        set[i] = separator;

      return Concat( set );


    }


  }
}
