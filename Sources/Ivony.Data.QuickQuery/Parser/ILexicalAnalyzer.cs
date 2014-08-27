using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Parser
{


  /// <summary>
  /// 定义一个词法分析器
  /// </summary>
  public interface ILexicalAnalyzer
  {

    /// <summary>
    /// 对文本进行词法分析
    /// </summary>
    /// <param name="text">要分析的文本</param>
    /// <param name="index">开始分析的位置</param>
    /// <returns>分析结果</returns>
    IEnumerable<ITextToken> Analyze( string text, int index = 0 );


  }
}
