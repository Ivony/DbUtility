using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Parser
{
  public interface ILexicalAnalyzer
  {

    IEnumerable<ITextToken> Tokenize( string text );


  }
}
