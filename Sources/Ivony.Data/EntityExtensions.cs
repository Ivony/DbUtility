using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  /// <summary>
  /// 提供面向 Entity 的扩展方法
  /// </summary>
  public static class EntityExtensions
  {



    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutableQuery query )
    {
      var data = query.ExecuteDataTable();
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>() ).ToArray();
    }


#if !NET40
    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      var data = await query.ExecuteDataTableAsync( token );
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>() ).ToArray();
    }
#endif


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutableQuery query, IEntityConverter<T> converter )
    {
      var data = query.ExecuteDataTable();
      return data.GetRows().Select( dataItem => dataItem.ToEntity( converter ) ).ToArray();
    }

#if !NET40

    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, IEntityConverter<T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var data = await query.ExecuteDataTableAsync( token );
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>( converter ) ).ToArray();
    }
#endif


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutableQuery query, Func<DataRow, T> converter )
    {
      var data = query.ExecuteDataTable();
      return data.GetRows().Select( dataItem => converter( dataItem ) ).ToArray();
    }

#if !NET40

    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, Func<DataRow, T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var data = await query.ExecuteDataTableAsync( token );
      return data.GetRows().Select( dataItem => converter( dataItem ) ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, Func<DataRow, CancellationToken, Task<T>> converter, CancellationToken token = default( CancellationToken ) )
    {
      var data = await query.ExecuteDataTableAsync( token );
      List<T> result = new List<T>();

      foreach ( var dataItem in data.GetRows() )
      {
        var entity = await converter( dataItem, token );
        result.Add( entity );
      }

      return result.ToArray();

    }
#endif


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutableQuery query )
    {
      var dataItem = query.ExecuteFirstRow();
      return dataItem.ToEntity<T>();

    }

#if !NET40
    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await query.ExecuteFirstRowAsync( token );
      return dataItem.ToEntity<T>();

    }
#endif


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutableQuery query, IEntityConverter<T> converter )
    {
      var dataItem = query.ExecuteFirstRow();
      return dataItem.ToEntity<T>( converter );

    }

#if !NET40

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, IEntityConverter<T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await query.ExecuteFirstRowAsync( token );
      return dataItem.ToEntity<T>( converter );

    }
#endif

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutableQuery query, Func<DataRow, T> converter )
    {
      var dataItem = query.ExecuteFirstRow();
      return converter( dataItem );
    }

#if !NET40
    /// <summary>
    /// 异步查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, Func<DataRow, T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await query.ExecuteFirstRowAsync( token );
      return converter( dataItem );
    }


    /// <summary>
    /// 异步查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">异步实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, Func<DataRow, CancellationToken, Task<T>> converter, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await query.ExecuteFirstRowAsync( token );
      return await converter( dataItem, token );
    }
#endif




    /// <summary>
    /// 将 DataRow 转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dataItem">包含数据的 DataRow</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this DataRow dataItem )
    {
      return ToEntity<T>( dataItem, null );
    }

    /// <summary>
    /// 将 DataRow 转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dataItem">包含数据的 DataRow</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this DataRow dataItem, IEntityConverter<T> converter )
    {
      if ( dataItem == null )
      {

        if ( typeof( T ).IsValueType )
          throw new ArgumentNullException( "dataItem" );

        else
          return default( T );//等同于return null
      }

      if ( dataItem.Table.Columns.Count == 1 )
      {
        var value = dataItem[0];

        if ( value is T )
          return (T) value;
      }


      var entityConverter = converter ?? EntityConvert<T>.GetConverter();
      return entityConverter.Convert( dataItem );
    }



    private static object sync = new object();
    private static Dictionary<Type, Func<DataRow, object>> entityConverterDictionary = new Dictionary<Type, Func<DataRow, object>>();


    internal static object ToEntity( this DataRow dataItem, Type entityType )
    {
      return GetToEntityMethod( entityType )( dataItem );
    }


    private static Func<DataRow, object> GetToEntityMethod( Type entityType )
    {
      lock ( sync )
      {
        if ( entityConverterDictionary.ContainsKey( entityType ) )
          return entityConverterDictionary[entityType];


        var method = typeof( EntityExtensions )
          .GetMethod( "ToEntity", new[] { typeof( DataRow ) } )
          .MakeGenericMethod( entityType );

        return entityConverterDictionary[entityType] = (Func<DataRow, object>) Delegate.CreateDelegate( typeof( Func<DataRow, object> ), method );
      }
    }






    private static Dictionary<Type, Func<DataRow, DataColumn, object>> dbValueConverterDictionary = new Dictionary<Type, Func<DataRow, DataColumn, object>>();


    internal static object FieldValue( this DataRow dataItem, DataColumn column, Type valueType )
    {
      return GetFieldValueMethod( valueType )( dataItem, column );
    }


    private static Func<DataRow, DataColumn, object> GetFieldValueMethod( Type valueType )
    {
      lock ( sync )
      {
        if ( dbValueConverterDictionary.ContainsKey( valueType ) )
          return dbValueConverterDictionary[valueType];


        var method = typeof( EntityExtensions )
          .GetMethod( "FieldValue", new[] { typeof( DataRow ), typeof( DataColumn ) } )
          .MakeGenericMethod( valueType );

        return dbValueConverterDictionary[valueType] = (Func<DataRow, DataColumn, object>) Delegate.CreateDelegate( typeof( Func<DataRow, DataColumn, object> ), method );
      }
    }




    /// <summary>
    /// 获取指定字段的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="dataRow">数据行</param>
    /// <param name="columnIndex">要返回的列的序号</param>
    /// <returns>强类型的值</returns>
    public static T FieldValue<T>( this DataRow dataRow, int columnIndex )
    {

      return FieldValue<T>( dataRow, dataRow.Table.Columns[columnIndex] );
    }

    /// <summary>
    /// 获取指定字段的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="dataRow">数据行</param>
    /// <param name="columnName">要返回其值的列名称</param>
    /// <returns>强类型的值</returns>
    public static T FieldValue<T>( this DataRow dataRow, string columnName )
    {
      return FieldValue<T>( dataRow, dataRow.Table.Columns[columnName] );
    }



    private static T FieldValue<T>( this DataRow dataRow, DataColumn column )
    {
      if ( dataRow == null )
        throw new ArgumentNullException( "dataRow" );

      if ( column == null )
        throw new ArgumentNullException( "column" );


      try
      {
        return DbValueConverter.ConvertFrom<T>( dataRow[column] );
      }
      catch ( Exception e )
      {
        e.Data.Add( "DataColumnName", column.ColumnName );
        throw;
      }
    }

  }
}
