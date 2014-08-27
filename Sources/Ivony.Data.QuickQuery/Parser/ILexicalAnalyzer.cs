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

    IEnumerable<ITextToken> Analyze( string text, int index = 0 );


  }
}
