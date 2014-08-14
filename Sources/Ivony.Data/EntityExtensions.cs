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
    public static T[] ExecuteEntities<T>( this IDbExecutableQuery query ) where T : new()
    {
      var data = query.ExecuteDataTable();
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>() ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) ) where T : new()
    {
      var data = await query.ExecuteDataTableAsync( token );
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>() ).ToArray();
    }



    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutableQuery query, IEntityConverter<T> converter ) where T : new()
    {
      var data = query.ExecuteDataTable();
      return data.GetRows().Select( dataItem => dataItem.ToEntity( converter ) ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, IEntityConverter<T> converter ) where T : new()
    {
      return ExecuteEntitiesAsync( query, CancellationToken.None, converter );
    }

    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token, IEntityConverter<T> converter ) where T : new()
    {
      var data = await query.ExecuteDataTableAsync( token );
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>( converter ) ).ToArray();
    }



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


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, Func<DataRow, T> converter )
    {
      return ExecuteEntitiesAsync( query, CancellationToken.None, converter );
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token, Func<DataRow, T> converter )
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
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token, Func<DataRow, CancellationToken, Task<T>> converter )
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



    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutableQuery query ) where T : new()
    {
      var dataItem = query.ExecuteFirstRow();
      return dataItem.ToEntity<T>();

    }

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token = default( CancellationToken ) ) where T : new()
    {
      var dataItem = await query.ExecuteFirstRowAsync( token );
      return dataItem.ToEntity<T>();

    }



    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutableQuery query, IEntityConverter<T> converter ) where T : new()
    {
      var dataItem = query.ExecuteFirstRow();
      return dataItem.ToEntity<T>( converter );

    }

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, IEntityConverter<T> converter ) where T : new()
    {
      return ExecuteEntityAsync( query, CancellationToken.None, converter );
    }


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token, IEntityConverter<T> converter ) where T : new()
    {
      var dataItem = await query.ExecuteFirstRowAsync( token );
      return dataItem.ToEntity<T>( converter );

    }


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


    /// <summary>
    /// 异步查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, Func<DataRow, T> converter )
    {
      return ExecuteEntityAsync( query, CancellationToken.None, converter );
    }


    /// <summary>
    /// 异步查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token, Func<DataRow, T> converter )
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
    public async static Task<T> ExecuteEntityAsync<T>( this IAsyncDbExecutableQuery query, CancellationToken token, Func<DataRow, CancellationToken, Task<T>> converter )
    {
      var dataItem = await query.ExecuteFirstRowAsync( token );
      return await converter( dataItem, token );
    }






    public static T ToEntity<T>( this DataRow dataItem ) where T : new()
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
    public static T ToEntity<T>( this DataRow dataItem, IEntityConverter<T> converter ) where T : new()
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


      var entityConverter = converter ?? EntityConverterCache<T>.GetConverter();


      var entity = new T();

      if ( entityConverter.NeedPreconversion )
      {
        var method = CreateEntityConvertMethod<T>();
        method( dataItem, entity );
      }

      entityConverter.Convert( dataItem, entity );
      return entity;
    }



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



    /// <summary>
    /// 提供默认的 EntityConverter 对象，这个对象什么都不做，并且被设置为可重用和需要预转换。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class DefaultEntityConverter<T> : IEntityConverter<T>
    {
      public void Convert( DataRow dataItem, T entity ) { return; }

      public bool IsReusable { get { return true; } }

      public bool NeedPreconversion { get { return true; } }
    }


    /// <summary>
    /// 创建转换方法
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>针对指定实体的转换方法</returns>
    private static Action<DataRow, T> CreateEntityConvertMethod<T>()
    {
      var method = ConverterCache<T>.Converter;
      if ( method != null )
        return method;

      var type = typeof( T );

      var properties = type.GetProperties()
        .Where( p => !GetAttributes( p ).OfType<NonFieldAttribute>().Any() );

      var methodName = type.GUID.ToString( "N" ) + "_DataConverter";
      var dynamicMethod = new DynamicMethod( methodName, null, new[] { typeof( DataRow ), type }, typeof( EntityExtensions ) );
      var il = dynamicMethod.GetILGenerator();

      /*
      .maxstack  3
      .locals init ([0] class [System.Data]System.Data.DataColumnCollection columns)
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance class [System.Data]System.Data.DataTable [System.Data]System.Data.DataRow::get_Table()
      IL_0006:  callvirt   instance class [System.Data]System.Data.DataColumnCollection [System.Data]System.Data.DataTable::get_Columns()
      IL_000b:  stloc.0
      */

      il.DeclareLocal( typeof( DataColumnCollection ) );
      il.Emit( OpCodes.Ldarg_0 );
      il.Emit( OpCodes.Callvirt, typeof( DataRow ).GetProperty( "Table" ).GetGetMethod() );
      il.Emit( OpCodes.Callvirt, typeof( DataTable ).GetProperty( "Columns" ).GetGetMethod() );
      il.Emit( OpCodes.Stloc_0 );


      foreach ( var p in properties )
      {
        var name = GetFieldname( p );

        /*
        IL_000c:  ldloc.0
        IL_000d:  ldstr      "Name"
        IL_0012:  callvirt   instance bool [System.Data]System.Data.DataColumnCollection::Contains(string)
        IL_0017:  brfalse.s  IL_002a
        IL_0019:  ldarg.1
        IL_001a:  ldarg.0
        IL_001b:  ldstr      "Name"
        IL_0020:  call       !!0 [System.Data.DataSetExtensions]System.Data.DataRowExtensions::Field<string>(class [System.Data]System.Data.DataRow, string)
        IL_0025:  callvirt   instance void Ivony.Data.EntityExtensions/TestEntity::set_Name(string)
        IL_002a:  ret
        */

        var label = il.DefineLabel();

        il.Emit( OpCodes.Ldloc_0 );
        il.Emit( OpCodes.Ldstr, name );
        il.Emit( OpCodes.Callvirt, typeof( DataColumnCollection ).GetMethod( "Contains" ) );
        il.Emit( OpCodes.Brfalse_S, label );
        il.Emit( OpCodes.Ldarg_1 );
        il.Emit( OpCodes.Ldarg_0 );
        il.Emit( OpCodes.Ldstr, name );
        il.Emit( OpCodes.Call, typeof( EntityExtensions ).GetMethod( "FieldValue", new[] { typeof( DataRow ), typeof( string ) } ).MakeGenericMethod( p.PropertyType ) );
        il.Emit( OpCodes.Callvirt, p.GetSetMethod() );
        il.MarkLabel( label );
      }

      il.Emit( OpCodes.Ret );

      method = (Action<DataRow, T>) dynamicMethod.CreateDelegate( typeof( Action<DataRow, T> ) );



      ConverterCache<T>.Converter = method;
      return method;
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


    /// <summary>
    /// 获取属性所对应的字段名
    /// </summary>
    /// <param name="property">要获取字段名的属性</param>
    /// <returns></returns>
    private static string GetFieldname( PropertyInfo property )
    {
      var attribute = GetAttributes( property ).OfType<FieldNameAttribute>().FirstOrDefault();

      if ( attribute != null )
        return attribute.FieldName;

      return property.Name;
    }


    private static object sync = new object();
    private static Dictionary<PropertyInfo, object[]> _propertyAttributesCache = new Dictionary<PropertyInfo, object[]>();

    /// <summary>
    /// 获取指定属性上的特性
    /// </summary>
    /// <param name="property">要获取特性的属性</param>
    /// <returns>属性上所设置的特性</returns>
    private static object[] GetAttributes( PropertyInfo property )
    {
      lock ( sync )
      {
        object[] attributes;

        if ( _propertyAttributesCache.TryGetValue( property, out attributes ) )
          return attributes;

        attributes = property.GetCustomAttributes( false );

        _propertyAttributesCache[property] = attributes;

        return attributes;
      }
    }

    private static class ConverterCache<T>
    {
      public static Action<DataRow, T> Converter { get; set; }
    }




    private static class EntityConverterCache<T>
    {
      private static IEntityConverter<T> converterInstacne;

      private static Func<IEntityConverter<T>> createConverter;


      static EntityConverterCache()
      {
        var type = typeof( T );
        var attribute = type.GetCustomAttributes( typeof( EntityConvertAttribute ), false ).OfType<EntityConvertAttribute>().FirstOrDefault();

        if ( attribute != null )
          createConverter = () => attribute.CreateConverter<T>();   //缓存创建实例的方法
        else
          createConverter = () => new DefaultEntityConverter<T>();


        var instance = createConverter();

        if ( instance.IsReusable )
          converterInstacne = instance;                             //缓存可复用的实例
      }


      public static IEntityConverter<T> GetConverter()
      {

        if ( converterInstacne != null && converterInstacne.IsReusable )//如果有缓存的可复用的实例，则返回
          return converterInstacne;


        return createConverter();
      }
    }
  }
}
