using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供 IDbValueConverter&lt;T&gt; 的注册点。
  /// </summary>
  public static class DbValueConverters
  {

    private static class ConverterCache<T>
    {
      public static IDbValueConverter<T> ConverterInstance { get; set; }
    }



    private static object sync = new object();

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


    internal static T Convert<T>( object value )
    {
      var converter = GetConverter<T>();
      if ( converter != null )
        return converter.ConvertValueFrom( value );

      else
        return (T) value;
    }
  }
}
