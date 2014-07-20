using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Parser
{


  /// <summary>
  /// 定义一个正则表达式词法分析器
  /// </summary>
  public class RegexTokenizer : TokenizerBase<RegexMatchToken>
  {


    private Regex regexInstance;


    public RegexTokenizer( string regularRxpression )
    {

      if ( !regularRxpression.StartsWith( @"\G" ) )
        regularRxpression = @"\G" + regularRxpression;

      regexInstance = new Regex( regularRxpression );
    }




    public override bool TryMatch( TextScaner scaner, out RegexMatchToken token )
    {
      var match = regexInstance.Match( scaner.Text, scaner.Offset );
      if ( match.Success )
      {
        token = CreateToken( scaner.Text, match );
        return true;
      }
      else
      {
        token = default( RegexMatchToken );
        return false;
      }
    }

    protected virtual RegexMatchToken CreateToken( string text, Match match )
    {
      return new RegexMatchToken( text, match );
    }
  }
}
