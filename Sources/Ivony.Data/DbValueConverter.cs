using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供 IDbValueConverter&lt;T&gt; 的注册点。
  /// </summary>
  public static class DbValueConverter
  {


    private static class ConverterCache<T>
    {
      public static IDbValueConverter<T> ConverterInstance { get; set; }
    }



    private static object sync = new object();

    private static Dictionary<Type, Converter> converterDictionary = new Dictionary<Type, Converter>();

    private delegate object Converter( object value, string dataType );

    /// <summary>
    /// 注册一个数据值类型转换器
    /// </summary>
    /// <typeparam name="T">需要转换的类型</typeparam>
    /// <param name="converter">需要注册的转换器</param>
    /// <param name="overwrite">是否覆盖现有的转换器</param>
    public static void Register<T>( IDbValueConverter<T> converter, bool overwrite = false )
    {
      lock ( sync )
      {

        if ( !overwrite && ConverterCache<T>.ConverterInstance != null )
          throw new InvalidOperationException();

        ConverterCache<T>.ConverterInstance = converter;

        converterDictionary[typeof( T )] = converter.ConvertValueTo;
      }
    }


    /// <summary>
    /// 解除数据值类型转换器的注册
    /// </summary>
    /// <typeparam name="T">转换的类型</typeparam>
    public static void Unregister<T>()
    {
      lock ( sync )
      {
        ConverterCache<T>.ConverterInstance = null;

        converterDictionary.Remove( typeof( T ) );
      }
    }


    /// <summary>
    /// 获取数据值类型转换器
    /// </summary>
    /// <typeparam name="T">需要转换的类型</typeparam>
    /// <returns></returns>
    public static IDbValueConverter<T> GetConverter<T>()
    {
      lock ( sync )
      {
        return ConverterCache<T>.ConverterInstance;
      }
    }


    internal static object ConvertTo( object value, string dataTypeName = null )
    {
      var type = value.GetType();

      lock ( sync )
      {

        foreach ( var _type in converterDictionary.Keys )
        {
          if ( _type.IsAssignableFrom( type ) )
            return converterDictionary[_type]( value, dataTypeName );
        }
      }

      return value;
    }


    internal static T ConvertFrom<T>( object value, string dataTypeName = null )
    {
      var converter = GetConverter<T>();
      if ( converter != null )
        return converter.ConvertValueFrom( value, dataTypeName );

      return DefaultConverter<T>.Converter( value );
    }



    private class DefaultConverter<T>
    {
      public static Converter<object, T> Converter = CreateConverter<T>();
    }

    private static Converter<object, T> CreateConverter<T>()
    {
      var type = typeof( T );

      if ( type.IsGenericType && !type.IsGenericTypeDefinition && typeof( Nullable<> ) == type.GetGenericTypeDefinition() )
      {
        var methodInfo = typeof( DbValueConverter ).GetMethod( "NullableConverter", BindingFlags.Static | BindingFlags.NonPublic ).MakeGenericMethod( new Type[] { type.GetGenericArguments()[0] } );
        return (Converter<object, T>) Delegate.CreateDelegate( typeof( Converter<object, T> ), methodInfo );
      }

      else if ( type.IsValueType )
      {
        return ConvertValueType<T>;
      }

      else
        return ConvertObject<T>;

    }


    private static T ConvertObject<T>( object value )
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return default(T);

      else
        return (T) value;

    }

    private static T ConvertValueType<T>( object value )
    {
      return (T) value;
    }


    private static T? ConvertNullable<T>( object value ) where T : struct
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return null;

      else
        return new T?( (T) value );
    }



  }
}
