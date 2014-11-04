using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  public sealed class SimpleDataColumnCollection : IReadOnlyCollection<SimpleDataColumn>, IReadOnlyDictionary<string, SimpleDataColumn>
  {


    private Dictionary<string, int> _columnMap;

    private SimpleDataColumn[] _columns;


    public SimpleDataColumnCollection( SimpleDataColumn[] columns )
    {

      HashSet<string> ignoreCaseNames = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
      CaseSensitive = false;

      foreach ( var item in columns )
      {
        if ( ignoreCaseNames.Add( item.Name ) )
        {
          CaseSensitive = true;
          break;
        }
      }


      _columns = columns;
      if ( CaseSensitive )
        _columnMap = new Dictionary<string, int>( StringComparer.Ordinal );

      else
        _columnMap = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );


      for ( int i = 0; i < columns.Length; i++ )
        _columnMap.Add( columns[i].Name, i );
    }


    public bool CaseSensitive { get; private set; }


    public SimpleDataColumn this[int index]
    {
      get { return _columns[index]; }
    }


    public SimpleDataColumn this[string name]
    {
      get { return _columns[GetOrdinal( name )]; }
    }


    public int GetOrdinal( string columnName )
    {
      return _columnMap[columnName];
    }

    int IReadOnlyCollection<SimpleDataColumn>.Count
    {
      get { return _columns.Length; }
    }

    IEnumerator<SimpleDataColumn> IEnumerable<SimpleDataColumn>.GetEnumerator()
    {
      return _columns.AsEnumerable().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _columns.GetEnumerator();
    }

    bool IReadOnlyDictionary<string, SimpleDataColumn>.ContainsKey( string key )
    {
      return _columnMap.ContainsKey( key );
    }

    IEnumerable<string> IReadOnlyDictionary<string, SimpleDataColumn>.Keys
    {
      get { return _columnMap.Keys; }
    }

    bool IReadOnlyDictionary<string, SimpleDataColumn>.TryGetValue( string key, out SimpleDataColumn value )
    {
      int index;
      var result = _columnMap.TryGetValue( key, out index );

      if ( result )
      {
        value = _columns[index];
        return true;
      }

      else
      {
        value = null;
        return false;
      }
    }

    IEnumerable<SimpleDataColumn> IReadOnlyDictionary<string, SimpleDataColumn>.Values
    {
      get { return _columns; }
    }

    SimpleDataColumn IReadOnlyDictionary<string, SimpleDataColumn>.this[string key]
    {
      get { return _columns[GetOrdinal( key )]; }
    }

    int IReadOnlyCollection<KeyValuePair<string, SimpleDataColumn>>.Count
    {
      get { return _columns.Length; }
    }

    IEnumerator<KeyValuePair<string, SimpleDataColumn>> IEnumerable<KeyValuePair<string, SimpleDataColumn>>.GetEnumerator()
    {
      return _columnMap.Select( item => new KeyValuePair<string, SimpleDataColumn>( item.Key, _columns[item.Value] ) ).GetEnumerator();
    }
  }
}
