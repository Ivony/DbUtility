using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供实体转换的静态方法
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public static class EntityConvert<T>
  {



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




    private static Action<DataRow, T> fillMethod;

    /// <summary>
    /// 获取实体填充方法
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>针对指定实体的转换方法</returns>
    internal static Action<DataRow, T> GetFillMethod()
    {

      lock ( sync )
      {
        return fillMethod ?? (fillMethod = CreateFillMethod());
      }

    }

    private static Action<DataRow, T> CreateFillMethod()
    {
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
        var setMethod = p.GetSetMethod();

        if ( setMethod == null )
          continue;

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
        il.Emit( OpCodes.Callvirt, setMethod );
        il.MarkLabel( label );
      }

      il.Emit( OpCodes.Ret );

      return (Action<DataRow, T>) dynamicMethod.CreateDelegate( typeof( Action<DataRow, T> ) );
    }



    /// <summary>
    /// 用指定的数据对象填充实体对象
    /// </summary>
    /// <param name="dataItem">数据对象</param>
    /// <param name="entity">要填充的实体对象</param>
    /// <returns>填充好的实体对象</returns>
    public static T FillEntity( DataRow dataItem, T entity )
    {
      GetFillMethod()( dataItem, entity );
      return entity;
    }




    static EntityConvert()
    {
      thisFillMethod = typeof( EntityConvert<T> ).GetMethod( "FillEntity" );

      var type = typeof( T );
      var attribute = type.GetCustomAttributes( typeof( EntityConvertAttribute ), false ).OfType<EntityConvertAttribute>().FirstOrDefault();

      if ( attribute != null )
        converterActivator = () => attribute.CreateConverter<T>();   //缓存创建实例的方法
      else
        converterActivator = () => CreateConverter();
    }



    private static MethodInfo thisFillMethod;
    private static Func<IEntityConverter<T>> converterActivator;
    private static IEntityConverter<T> converter;




    /// <summary>
    /// 获取实体转换器
    /// </summary>
    /// <returns>实体转换器</returns>
    internal static IEntityConverter<T> GetConverter()
    {

      lock ( sync )
      {
        if ( converter != null )
          return converter;

      }


      var instance = converterActivator();

      lock ( sync )
      {
        if ( instance.IsReusable && converter == null )
          converter = instance;


        return converter ?? instance;
      }
    }


    private static bool CheckMethodSignature( MethodBase method, params Type[] types )
    {
      return new HashSet<Type>( method.GetParameters().Select( p => p.ParameterType ) ).SetEquals( new HashSet<Type>( types ) );
    }


    /// <summary>
    /// 创建实体转换器
    /// </summary>
    /// <param name="type">实体类型</param>
    /// <returns></returns>
    private static IEntityConverter<T> CreateConverter()
    {

      var type = typeof( T );
      {
        var methods = from i in type.GetMethods( BindingFlags.Static | BindingFlags.Public )
                      where string.Equals( i.Name, "CreateInstance", StringComparison.OrdinalIgnoreCase )
                      where CheckMethodSignature( i, typeof( DataRow ) )
                      where i.ReturnType == typeof( T )
                      select i;

        if ( methods.Count() == 1 )
          return CreateConverter( methods.First() );
      }


      {



        var constructor = type.GetConstructor( new[] { typeof( DataRow ) } );
        if ( constructor != null )
          return CreateConverter( constructor, true );

        constructor = type.GetConstructor( new Type[0] );
        if ( constructor != null )
          return CreateConverter( constructor, false );

      }

      throw new NotSupportedException( string.Format( "不支持 {0} 类型的实体转换，因为该类型没有公开的无参或 DataRow 类型的构造函数，也没有指定了自定义实体类型转换器。", typeof( T ).AssemblyQualifiedName ) );
    }

    private static IEntityConverter<T> CreateConverter( MethodInfo method )
    {
      return new EntityConverter( (Func<DataRow, T>) Delegate.CreateDelegate( typeof( Func<DataRow, T> ), method ) );

    }



    private static IEntityConverter<T> CreateConverter( ConstructorInfo constructorInfo, bool withDataRow )
    {

      Expression body;
      var parameter = Expression.Parameter( typeof( DataRow ), "dataItem" );

      if ( withDataRow )
        body = Expression.New( constructorInfo, parameter );

      else
        body = Expression.Call( thisFillMethod, parameter, Expression.New( constructorInfo ) );


      var func = Expression.Lambda<Func<DataRow, T>>( body, parameter );
      return new EntityConverter( func.Compile() );
    }



    private class EntityConverter : IEntityConverter<T>
    {

      private Func<DataRow, T> _method;

      public EntityConverter( Func<DataRow, T> method )
      {
        _method = method;
      }


      public T Convert( DataRow dataItem )
      {
        return _method( dataItem );
      }

      public bool IsReusable
      {
        get { return true; }
      }
    }




    private static class EntityConverterCache
    {
      private static IEntityConverter<T> converterInstacne;

      private static Func<IEntityConverter<T>> createConverter;


      static EntityConverterCache()
      {
      }


      /// <summary>
      /// 获取指定类型的实体转换器
      /// </summary>
      /// <returns></returns>
      public static IEntityConverter<T> GetConverter()
      {

        if ( converterInstacne != null && converterInstacne.IsReusable )//如果有缓存的可复用的实例，则返回
          return converterInstacne;


        return createConverter();
      }
    }


    /// <summary>
    /// 提供默认的 EntityConverter 对象，这个对象什么都不做，并且被设置为可重用和需要预转换。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class DefaultEntityConverter : IEntityConverter<T>
    {
      public T Convert( DataRow dataItem )
      {

        throw new NotImplementedException();
      }

      public bool IsReusable { get { return true; } }
    }




  }
}
