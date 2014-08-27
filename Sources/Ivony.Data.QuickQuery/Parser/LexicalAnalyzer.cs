using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Parser
{


  /// <summary>
  /// 定义词法分析器的抽象基类
  /// </summary>
  public abstract class LexicalAnalyzer : ILexicalAnalyzer
  {


    /// <summary>
    /// 对指定文本字符串进行词法分析
    /// </summary>
    /// <param name="text">要分析的文本字符串</param>
    /// <returns>分析结果</returns>
    public IEnumerable<ITextToken> Analyze( string text, int index = 0 )
    {
      var tokenizer = GetTokenizer( GetType(), new TextScaner( new object(), text, index ) );

      while ( true )
      {
        var token = tokenizer.NextToken();
        if ( token == null )
          yield break;

        else
          yield return token;
      }
    }

    private Tokenizer GetTokenizer( Type type, TextScaner scaner )
    {

      return new Tokenizer( scaner, lexicalsCache.GetOrAdd( type, GetLexicals ) );

    }

    private static Func<TextScaner, ITextToken>[] GetLexicals( Type type )
    {
      var methods = from method in type.GetMethods( BindingFlags.Static | BindingFlags.NonPublic )
                    where typeof( ITextToken ).IsAssignableFrom( method.ReturnType )
                    where method.IsFamily
                    let parameters = method.GetParameters()
                    where parameters.Length == 1 && parameters[0].ParameterType == typeof( TextScaner )
                    select (Func<TextScaner, ITextToken>) Delegate.CreateDelegate( typeof( Func<TextScaner, ITextToken> ), method );


      return methods.ToArray();
    }



    private static ConcurrentDictionary<Type, Func<TextScaner, ITextToken>[]> lexicalsCache = new ConcurrentDictionary<Type, Func<TextScaner, ITextToken>[]>();



    private class Tokenizer
    {


      public Tokenizer( TextScaner scaner, Func<TextScaner, ITextToken>[] lexicals )
      {
        if ( scaner == null )
          throw new ArgumentNullException( "scaner" );

        Scaner = scaner;
        Lexicals = lexicals;
      }


      /// <summary>
      /// 获取当前使用的字符扫描器
      /// </summary>
      public TextScaner Scaner { get; private set; }



      /// <summary>
      /// 获取要使用的词法分析器列表
      /// </summary>
      public Func<TextScaner, ITextToken>[] Lexicals
      {
        get;
        private set;
      }


      /// <summary>
      /// 获取下一个词素
      /// </summary>
      /// <returns>词素对象</returns>
      public ITextToken NextToken()
      {


        foreach ( var lexical in Lexicals )
        {
          var token = lexical( Scaner );

          if ( token != null )
            return token;
        }

        return null;
      }
    }





    protected static TextToken CreateToken( TextScaner scaner, int length, string type = null )
    {

      var offset = scaner.Offset;
      scaner.Skip( length );

      return new TextToken( scaner.Text, offset, length, type );
    }
  }
}

