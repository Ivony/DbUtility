using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// SimpleDataTable 的容器，可以保存一次查询的所有结果集
  /// </summary>
  public sealed class SimpleDataSet : IEnumerable<SimpleDataTable>
  {


    private SimpleDataSet( IReadOnlyCollection<SimpleDataTable> tables ) { Tables = tables; }



    /// <summary>
    /// 通过 IDataReader 填充 SimpleDataSet
    /// </summary>
    /// <param name="reader">用于读取数据的 DataReader</param>
    /// <returns></returns>
    public static SimpleDataSet Fill( IDataReader reader )
    {

      var tables = new List<SimpleDataTable>();

      do
      {
        tables.Add( SimpleDataTable.Fill( reader ) );

      } while ( reader.NextResult() );



      return new SimpleDataSet( new ReadOnlyCollection<SimpleDataTable>( tables ) );
    }


    /// <summary>
    /// 获取所有结果集，以 SimpleDataTable 实例形式返回
    /// </summary>
    public IReadOnlyCollection<SimpleDataTable> Tables { get; private set; }

    public IEnumerator<SimpleDataTable> GetEnumerator()
    {
      return Tables.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return Tables.GetEnumerator();
    }
  }
}
