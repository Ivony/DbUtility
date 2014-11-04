using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 定义数据列的抽象
  /// </summary>
  public abstract class SimpleDataColumn
  {

    /// <summary>
    /// 私有化构造函数，避免外部实例化
    /// </summary>
    private SimpleDataColumn( SimpleDataTable dataTable )
    {

      DataTable = dataTable;

    }


    public SimpleDataTable DataTable { get; private set; }


    /// <summary>
    /// 数据类型
    /// </summary>
    public abstract Type ColumnType { get; }

    /// <summary>
    /// 添加一个数据项
    /// </summary>
    /// <param name="data"></param>
    internal abstract void AddDataItem( object data );



    private class DataColumn<T> : SimpleDataColumn
    {


      public DataColumn( SimpleDataTable dataTable ) : base( dataTable ) { }


      private List<T> dataList = new List<T>();


      public override Type ColumnType
      {
        get { return typeof( T ); }
      }

      internal override void AddDataItem( object data )
      {
        dataList.Add( (T) data );
      }
    }



    private static object _sync = new object();
    private static Dictionary<Type, Func<SimpleDataTable,SimpleDataColumn>> columnCreators = new Dictionary<Type, Func<SimpleDataTable, SimpleDataColumn>>();

    internal static SimpleDataColumn CreateDataColumn( SimpleDataTable dataTable, string name, Type fieldType )
    {

      Func<SimpleDataTable,SimpleDataColumn> func;

      lock ( _sync )
      {
        if ( !columnCreators.TryGetValue( fieldType, out func ) )
        {
          var newExpression = Expression.New( typeof( DataColumn<> ).MakeGenericType( fieldType ).GetConstructor( new[] { typeof( SimpleDataTable ) } ), Expression.Variable( typeof( SimpleDataTable ), "dataTable" ) );
          func = Expression.Lambda<Func<SimpleDataTable, SimpleDataColumn>>( newExpression, Expression.Parameter( typeof( SimpleDataTable ), "dataTable" ) ).Compile();
          columnCreators[fieldType] = func;
        }
      }


      return func( dataTable );
    }
  }
}
