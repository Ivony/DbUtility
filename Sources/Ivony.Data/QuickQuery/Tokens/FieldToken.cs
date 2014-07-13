using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery.Tokens
{
  public struct FieldToken
  {

    public FieldToken( string field ) : this( null, field ) { }

    public FieldToken( string table, string field )
    {
      _table = table;
      _field = field;
    }

    private string _table;
    public string TableAlias { get { return _table; } }

    private string _field;
    public string FieldName { get { return _field; } }

  }
}
