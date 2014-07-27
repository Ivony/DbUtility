using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Parser
{
  public class LiteralTokenizer : TokenizerBase<TextToken>
  {


    private LiteralTokenizer( string literal, bool ignoreCase = false )
    {
      LiteralText = literal;
      IgnoreCase = ignoreCase;

    }

    public string LiteralText
    {
      get;
      private set;
    }


    public bool IgnoreCase
    {
      get;
      private set;
    }



    public override bool TryMatch( TextScaner scaner, out TextToken token )
    {
      var text = scaner.SubString( scaner.Offset, LiteralText.Length );
      if ( IgnoreCase )
      {
        if ( string.Equals( text, LiteralText, StringComparison.OrdinalIgnoreCase ) )
        {
          token = new TextToken( scaner.Text, scaner.Offset, text.Length );
          return true;
        }
      }
      else
      {
        if ( string.Equals( text, LiteralText, StringComparison.Ordinal ) )
        {
          token = new TextToken( scaner.Text, scaner.Offset, text.Length );
          return true;
        }
      }

      token = default( TextToken );
      return false;
    }
  }
}
