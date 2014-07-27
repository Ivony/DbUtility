using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Parser
{


  /// <summary>
  /// 定义 Tokenizer 的抽象基类
  /// </summary>
  public abstract class TokenizerBase<T> where T : ITextToken
  {



    /// <summary>
    /// 在当前位置尝试匹配 Token
    /// </summary>
    /// <param name="scaner">字符扫描器</param>
    /// <param name="token">匹配到的 Token</param>
    /// <returns>是否成功匹配</returns>
    public abstract bool TryMatch( TextScaner scaner, out T token );

    /// <summary>
    /// 在当前位置尝试匹配 Token
    /// </summary>
    /// <param name="scaner">字符扫描器</param>
    /// <returns>匹配到的 Token</returns>
    public virtual T TryMatch( TextScaner scaner )
    {
      T token;
      if ( TryMatch( scaner, out token ) )
        return token;

      else
        return default( T );
    }


    public virtual bool IsMatch( TextScaner scaner )
    {
      T token;
      return TryMatch( scaner, out token );
    }


    public virtual bool TryFindMatch( TextScaner scaner, out T token )
    {
      while ( true )
      {
        if ( TryMatch( scaner, out token ) )
          return true;

        if ( !scaner.MoveNext() )
          return false;
      }
    }


    public virtual T TryFindMatch( TextScaner scaner )
    {
      T token;
      if ( TryFindMatch( scaner, out token ) )
        return token;

      else
        return default( T );
    }
  }
}
