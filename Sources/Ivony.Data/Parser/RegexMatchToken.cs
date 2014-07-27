using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Parser
{
  public struct RegexMatchToken : ITextToken
  {

    private string _text;
    private Match _match;


    public RegexMatchToken( string text, Match match )
    {
      _text = text;
      _match = match;
    }

    public string Text
    {
      get { return _text; }
    }

    public int StartIndex
    {
      get { return _match.Index; }
    }

    public int Length
    {
      get { return _match.Length; }
    }
  }
}
