using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Parser
{
  public struct RegexTextToken : ITextToken
  {


    public RegexTextToken( string text, Match match, string type = null )
    {
      _match = match;
      _text = text;
      _startIndex = match.Index;
      _length = match.Length;
      _type = type;
      _str = null;
    }


    private string _text;
    public string Text
    {
      get { return _text; }
    }

    private int _startIndex;
    public int StartIndex
    {
      get { return _startIndex; }
    }

    private int _length;
    public int Length
    {
      get { return _length; }
    }

    private string _type;
    public string Type
    {
      get { return _type; }
    }


    private Match _match;
    public Match RegexMatch
    {
      get { return _match; }
    }



    private string _str;
    /// <summary>获取 TextToken 的字符串表达</summary>
    public override string ToString()
    {

      if ( _str == null )
      {
        if ( _text == null )
          return null;

        _str = _text.Substring( _startIndex, _length );
      }

      return _str;
    }

  }
}
