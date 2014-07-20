using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Parser
{
  public abstract class ParserBase
  {

    /// <summary>
    /// 用于匹配 CName 的正则表达式的词法分析器
    /// </summary>
    protected static readonly RegexTokenizer CName = new RegexTokenizer( @"[a-zA-z_][a-zA-Z_0-9]*" );


    /// <summary>
    /// 用于匹配空白字符的正则表达式的词法分析器
    /// </summary>
    protected static readonly RegexTokenizer WhiteSpace = new RegexTokenizer( @"\s+" );



    /// <summary>
    /// 字符串扫描器
    /// </summary>
    protected TextScaner Scaner
    {
      get;
      private set;
    }


    private object _sync = new object();


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public virtual object SyncRoot
    {
      get { return _sync; }
    }


    /// <summary>
    /// 初始化 Tokenizer
    /// </summary>
    /// <param name="text">要分析的字符串</param>
    /// <param name="index">要开始分析的位置</param>
    protected void Initialize( string text, int index = 0 )
    {
      Scaner = new TextScaner( SyncRoot, text, index );
    }



    /// <summary>
    /// 在扫描器当前位置是否为指定的字符，若匹配到指定字符，则推移扫描器指针
    /// </summary>
    /// <param name="ch">指定的字符</param>
    /// <returns>当前位置是否为指定的字符</returns>
    public char? Match( char ch )
    {
      if ( Scaner.IsEnd )
        return null;

      if ( Scaner.Current != ch )
        return null;

      Scaner.MoveNext();
      return ch;
    }



    /// <summary>
    /// 判断扫描器当前位置是否为指定的字符
    /// </summary>
    /// <param name="ch">指定的字符</param>
    /// <returns>当前位置是否为指定的字符</returns>
    public bool IsMatch( char ch )
    {

      if ( Scaner.IsEnd )
        return false;

      if ( Scaner.Current != ch )
        return false;


      return true;
    }


    /// <summary>
    /// 确定扫描器当前位置必须为指定的字符
    /// </summary>
    /// <param name="ch">指定的字符</param>
    public void EnsureMatch( char ch )
    {

      if ( !IsMatch( ch ) )
        throw FormatError( ch );
    }


    /// <summary>
    /// 判断扫描器当前位置是否为指定的字符
    /// </summary>
    /// <param name="chars">指定的字符</param>
    /// <returns>当前位置是否为指定的字符</returns>
    public bool IsMatchAny( params char[] chars )
    {
      if ( Scaner.IsEnd )
        return false;

      var current = Scaner.Current;
      return chars.Any( ch => ch == current );
    }


    /// <summary>
    /// 确定扫描器当前位置必须为指定的字符
    /// </summary>
    /// <param name="chars">指定的字符</param>
    public void EnsureMatchAny( params char[] chars )
    {

      if ( !IsMatchAny( chars ) )
        throw FormatError();
    }


    /// <summary>
    /// 在扫描器当前位置尝试匹配。
    /// </summary>
    /// <param name="tokenizer">词法分析器</param>
    /// <returns>若匹配成功，返回 Token 对象，否则返回 Token 类型的默认值。</returns>
    protected T Match<T>( TokenizerBase<T> tokenizer ) where T : ITextToken
    {
      T token;
      if ( TryMatch( tokenizer, out token ) )
        return token;

      else                               
        return default( T );
    }


    /// <summary>
    /// 在扫描器当前位置尝试匹配。
    /// </summary>
    /// <param name="tokenizer">词法分析器</param>
    /// <param name="token">匹配到的 Token</param>
    /// <returns>是否匹配成功。</returns>
    protected bool TryMatch<T>( TokenizerBase<T> tokenizer, out T token ) where T : ITextToken
    {
      return tokenizer.TryMatch( Scaner, out token );
    }


    /// <summary>
    /// 确认在扫描器当前位置是否满足正则匹配
    /// </summary>
    /// <param name="tokenizer">词法分析器</param>
    /// <returns>当前位置是否满足该词法分析</returns>
    protected bool IsMatch<T>( TokenizerBase<T> tokenizer ) where T : ITextToken
    {
      return tokenizer.IsMatch( Scaner );
    }


    /// <summary>
    /// 确保在扫描器当前位置必须满足正则匹配
    /// </summary>
    /// <param name="tokenizer">词法分析器</param>
    /// <param name="description">关于该匹配的描述</param>
    /// <returns>当前位置是否满足该词法分析</returns>
    public T EnsureMatch<T>( TokenizerBase<T> tokenizer, string description = null ) where T : ITextToken
    {
      T token;
      if ( TryMatch( tokenizer, out token ) )
        throw FormatError( description );

      return token;
    }



    /// <summary>
    /// 构建一个字符串格式错误的异常，报告解析器在当前位置遇到格式错误
    /// </summary>
    /// <returns>异常信息</returns>
    protected FormatException FormatError()
    {

      string message;
      if ( Scaner.IsEnd )
        message = string.Format( CultureInfo.InvariantCulture, "意外遇到字符串结束，在分析字符串 \"{0}\" 时。", Scaner.ToString() );

      else
        message = string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析字符串 \"{1}\" 第 {2} 字符处。", Scaner.Current, Scaner.ToString(), Scaner.Offset );

      return new FormatException( message );
    }



    /// <summary>
    /// 构建一个字符串格式错误的异常，报告解析器在当前位置遇到格式错误
    /// </summary>
    /// <param name="desired">在当前位置期望的字符</param>
    /// <returns>异常信息</returns>
    protected FormatException FormatError( char desired )
    {
      string message;
      if ( Scaner.IsEnd )
        message = string.Format( CultureInfo.InvariantCulture, "意外遇到字符串结束，在分析字符串 \"{0}\" 时，期望的字符为 '{1}'。", Scaner.ToString(), desired );

      else
        message = string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析字符串 \"{1}\" 第 {2} 字符处，期望的字符为 '{3}' 。", Scaner.Current, Scaner.ToString(), Scaner.Offset, desired );


      return new FormatException( message );
    }

    /// <summary>
    /// 构建一个字符串格式错误的异常，报告解析器在当前位置遇到格式错误
    /// </summary>
    /// <param name="description">在当前位置期望遇到的匹配的描述</param>
    /// <returns>异常信息</returns>
    protected FormatException FormatError( string description )
    {
      string message;
      if ( Scaner.IsEnd )
        message = string.Format( CultureInfo.InvariantCulture, "意外遇到字符串结束，在分析字符串 \"{0}\" 时，期望的表达式为{1}。", Scaner.ToString(), description );

      else
        message = string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析字符串 \"{1}\" 第 {2} 字符处，期望的表达式为 {3} 。", Scaner.Current, Scaner.ToString(), Scaner.Offset, description );

      return new FormatException( message );
    }


  }
}
