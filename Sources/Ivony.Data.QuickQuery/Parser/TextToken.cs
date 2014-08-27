using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Parser
{
  /// <summary>
  /// 代表一个文本 Token
  /// </summary>
  public struct TextToken : ITextToken
  {

    /// <summary>
    /// 创建 TextToken 对象
    /// </summary>
    /// <param name="text">分析的原始文本</param>
    /// <param name="startIndex">在文本中的开始位置</param>
    /// <param name="length">TextToken 的长度</param>
    public TextToken( string text, int startIndex, int length, string type = null )
    {
      _text = text;
      _startIndex = startIndex;
      _length = length;
      _type = type;

      _str = null;
    }


    private string _text;
    /// <summary>分析的原始文本</summary>
    public string Text
    {
      get { return _text; }
    }


    private int _startIndex;
    /// <summary>TextToken 在文本中的起始位置</summary>
    public int StartIndex
    {
      get { return _startIndex; }
    }


    private int _length;
    /// <summary>TextToken 的长度</summary>
    public int Length
    {
      get { return _length; }
    }




    private string _type;
    /// <summary>TextToken 的类型</summary>
    public string Type
    {
      get { return _type; }
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
