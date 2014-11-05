using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{



  /// <summary>
  /// SimpleDataColumn 容器的实现，仅供 SimpleDataTable 使用
  /// </summary>
  public sealed class SimpleDataColumnCollection : IReadOnlyDictionary<string, SimpleDataColumn>
  {

    private Dictionary<string, int> _columnMap;

    private SimpleDataColumn[] _columns;


    internal SimpleDataColumnCollection( SimpleDataColumn[] columns )
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



    /// <summary>
    /// 获取列名是否为大小写敏感的
    /// </summary>
    public bool CaseSensitive { get; private set; }


    /// <summary>
    /// 通过列序号来获取列
    /// </summary>
    /// <param name="index">列序号</param>
    /// <returns></returns>
    public SimpleDataColumn this[int index]
    {
      get { return _columns[index]; }
    }


    /// <summary>
    /// 通过列名称来获取列
    /// </summary>
    /// <param name="name">数据列名称</param>
    /// <returns></returns>
    public SimpleDataColumn this[string name]
    {
      get { return _columns[GetOrdinal( name )]; }
    }


    /// <summary>
    /// 通过列名称获取列的顺序
    /// </summary>
    /// <param name="columnName">数据列名称</param>
    /// <returns></returns>
    public int GetOrdinal( string columnName )
    {
      return _columnMap[columnName];
    }


    /// <summary>
    /// 获取指定列的序号
    /// </summary>
    /// <param name="column">数据列</param>
    /// <returns></returns>
    public int GetOrdinal( SimpleDataColumn column )
    {
      return _columnMap[column.Name];
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




    private IEnumerable<KeyValuePair<string, SimpleDataColumn>> GetEnumerable()
    {
      return _columnMap.Select( item => new KeyValuePair<string, SimpleDataColumn>( item.Key, _columns[item.Value] ) );
    }


    IEnumerator<KeyValuePair<string, SimpleDataColumn>> IEnumerable<KeyValuePair<string, SimpleDataColumn>>.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }

  }
}
