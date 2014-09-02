using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Parser
{
  public abstract class SyntacticAnalyzer<T>
  {

    public T Analyze( string text )
    {
      var lexicalAnalyzer = GetLexicalAnalyzer();


      var phraseStack = new Stack();

      foreach ( var token in lexicalAnalyzer.Analyze( text ) )
      {
        phraseStack.Push( token );

        var phrase = TryCreatePhrase( phraseStack );

      }

    }



    private MethodMapping[] methods;

    private object TryCreatePhrase( Stack phraseStack )
    {
      var count = phraseStack.Count;






    }


    private class MethodMapping
    {

      private Type[] _argumentTypes;

      public bool Match( Type[] types )
      {

        var offset = types.Length - _argumentTypes.Length;
        if ( offset < 0 )
          return false;

        for ( int i = 0; i < types.Length; i++ )
        {
          if ( _argumentTypes[i].IsAssignableFrom( types[i + offset] ) )
            return false;
        }

        return true;
      }
    }

    protected abstract ILexicalAnalyzer GetLexicalAnalyzer();


    protected Stack PhraseStack
    {
      get;
      private set;
    }

  }
}
