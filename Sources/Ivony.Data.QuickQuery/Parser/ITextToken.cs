using System;
namespace Ivony.Parser
{

  /// <summary>
  /// 定义一个文本 Token
  /// </summary>
  public interface ITextToken
  {
    /// <summary>分析的原始文本</summary>
    string Text { get; }


    /// <summary>TextToken 在文本中的起始位置</summary>
    int StartIndex { get; }


    /// <summary>TextToken 的长度</summary>
    int Length { get; }

    /// <summary>Token 类型，供 Parser 参考</summary>
    string Type { get; }

  }
}
