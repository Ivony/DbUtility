using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 提供面向 Entity 的扩展方法
  /// </summary>
  public static class EntityExtensions
  {



    /// <summary>
    /// 查询数据库并将最后一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbUtility">DbUtility 实例</param>
    /// <param name="expression">查询表达式</param>
    /// <returns>实体集</returns>
    public static T[] Entities<T>( this DbUtility dbUtility, IDbExpression expression ) where T : new()
    {
      var data = dbUtility.Data( expression );
      return data.Rows.Cast<DataRow>().Select( dataItem => dataItem.ToEntity<T>() ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbUtility">DbUtility 实例</param>
    /// <param name="expression">查询表达式</param>
    /// <returns>实体</returns>
    public static T Entity<T>( this DbUtility dbUtility, IDbExpression expression ) where T : new()
    {
      var dataItem = dbUtility.FirstRow( expression );
      return dataItem.ToEntity<T>();
    }


    /// <summary>
    /// 将 DataRow 转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dataItem">包含数据的 DataRow</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this DataRow dataItem ) where T : new()
    {
      var method = CreateEntityConverter<T>();
      var entity = new T();
      method( dataItem, entity );
      return entity;
    }

    private static Action<DataRow, T> CreateEntityConverter<T>()
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
        il.Emit( OpCodes.Call, typeof( DataRowExtensions ).GetMethod( "Field", new[] { typeof( DataRow ), typeof( string ) } ).MakeGenericMethod( p.PropertyType ) );
        il.Emit( OpCodes.Callvirt, p.GetSetMethod() );
        il.MarkLabel( label );
      }

      il.Emit( OpCodes.Ret );

      method = (Action<DataRow, T>) dynamicMethod.CreateDelegate( typeof( Action<DataRow, T> ) );
      ConverterCache<T>.Converter = method;
      return method;
    }

    private static string GetFieldname( PropertyInfo p )
    {
      var attribute = GetAttributes( p ).OfType<FieldNameAttribute>().FirstOrDefault();

      if ( attribute != null )
        return attribute.FieldName;

      return p.Name;
    }


    private static object sync = new object();
    private static Dictionary<PropertyInfo, object[]> _propertyAttributesCache = new Dictionary<PropertyInfo,object[]>();

    private static object[] GetAttributes( PropertyInfo p )
    {
      lock ( sync )
      {
        object[] attributes;

        if ( _propertyAttributesCache.TryGetValue( p, out attributes ) )
          return attributes;

        attributes = p.GetCustomAttributes( false );

        _propertyAttributesCache[p] = attributes;

        return attributes;
      }
    }

    private static class ConverterCache<T>
    {
      public static Action<DataRow, T> Converter { get; set; }
    }
  }

  /// <summary>
  /// 用于指定字段名称的特性
  /// </summary>
  [AttributeUsage( AttributeTargets.Property, Inherited = false )]
  public class FieldNameAttribute : Attribute
  {
    /// <summary>
    /// 创建 FieldNameAttribute 对象
    /// </summary>
    /// <param name="name">字段名</param>
    public FieldNameAttribute( string name )
    {
      FieldName = name;
    }

    /// <summary>
    /// 字段名
    /// </summary>
    public string FieldName
    {
      get;
      set;
    }
  }


  /// <summary>
  /// 用于指示属性与任何字段没有关系
  /// </summary>
  [AttributeUsage( AttributeTargets.Property, Inherited = false )]
  public class NonFieldAttribute : Attribute
  {
  }

}
