using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供 IDbValueConverter&lt;T&gt; 的注册点。
  /// </summary>
  public static partial class DbValueConverter
  {



    /// <summary>
    /// 是否禁用 Convertible 对象转换（原生对象之间的转换）
    /// </summary>
    public static bool DisableConvertible { get; set; }

    private static class ConverterCache<T>
    {
      public static IDbValueConverter<T> ConverterInstance { get; set; }
    }



    private static object sync = new object();

    private static Dictionary<Type, Converter> convertToMethodDictionary = new Dictionary<Type, Converter>();
    private static Dictionary<Type, Converter> convertFromMethodDictionary = new Dictionary<Type, Converter>();

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

        convertToMethodDictionary[typeof( T )] = converter.ConvertValueTo;
        convertFromMethodDictionary[typeof( T )] = ( value, dataType ) => converter.ConvertValueFrom( value, dataType );
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

        convertToMethodDictionary.Remove( typeof( T ) );
        convertFromMethodDictionary.Remove( typeof( T ) );
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


    /// <summary>
    /// 从数据类型转换为目标类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value">数据值</param>
    /// <param name="dataTypeName">数据类型名称</param>
    /// <returns>类型转换后的结果</returns>
    internal static T ConvertFrom<T>( object value, string dataTypeName = null )
    {
      var converter = GetConverter<T>();
      if ( converter != null )
        return converter.ConvertValueFrom( value, dataTypeName );

      return DefaultConverter<T>.Converter( value );
    }




    /// <summary>
    /// 将值转换为数据库可以接受的类型
    /// </summary>
    /// <param name="value">所需要转换的值对象</param>
    /// <param name="dataTypeName">数据库类型名称</param>
    /// <returns>数据库可接受的类型</returns>
    internal static object ConvertTo( object value, string dataTypeName = null )
    {
      if ( value == null )
        return DBNull.Value;


      var type = value.GetType();

      lock ( sync )
      {

        foreach ( var _type in convertToMethodDictionary.Keys )
        {
          if ( _type.IsAssignableFrom( type ) )
            return convertToMethodDictionary[_type]( value, dataTypeName );
        }
      }

      return value;
    }
  }
}
