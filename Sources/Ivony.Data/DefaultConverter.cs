using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public partial class DbValueConverter
  {
    private class DefaultConverter<T>
    {
      public static Converter<object, T> Converter = CreateConverter<T>();
    }


    private static Converter<object, T> CreateConverter<T>()
    {
      var type = typeof( T );

      var underlyingType = Nullable.GetUnderlyingType( type );
      if ( underlyingType != null )
      {
        var methodInfo = typeof( DbValueConverter ).GetMethod( "ConvertNullable", BindingFlags.Static | BindingFlags.NonPublic ).MakeGenericMethod( new Type[] { underlyingType } );
        return (Converter<object, T>) Delegate.CreateDelegate( typeof( Converter<object, T> ), methodInfo );
      }


      else if ( type.IsEnum )
        return ConvertEnum<T>;

      else if ( IsConvertiableType( type ) )
        return ConvertConvertible<T>;


      else if ( type.IsValueType )
        return ConvertValueType<T>;

      else
        return ConvertObject<T>;
    }



    private static T? ConvertNullable<T>( object value ) where T : struct
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return null;

      else
        return new T?( ConvertFrom<T>( value ) );
    }


    private static T ConvertEnum<T>( object value )
    {
      var enumType = typeof( T );


      var str = value as string;
      if ( str != null )
        return (T) Enum.Parse( enumType, str );


      return (T) Enum.ToObject( enumType, value );
    }





    private static bool IsConvertiableType( Type type )
    {
      var typeCode = Type.GetTypeCode( type );

      switch ( typeCode )
      {
        case TypeCode.String:
        case TypeCode.Boolean:
        case TypeCode.Byte:
        case TypeCode.Char:
        case TypeCode.DateTime:
        case TypeCode.Decimal:
        case TypeCode.Double:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.SByte:
        case TypeCode.Single:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
          return true;

        default:
          return false;
      }
    }

    private static T ConvertConvertible<T>( object value )
    {

      var type = typeof( T );

      if ( DisableConvertible )
        return (T) value;

      return (T) ConvertConvertible( value, type );

    }

    private static object ConvertConvertible( object value, Type type )
    {
      var convertible = value as IConvertible;
      if ( convertible == null )
        throw new InvalidCastException( string.Format( CultureInfo.InvariantCulture, "无法将 {0} 类型的实例转换为 {1} 类型", value.GetType(), type ) );

      if ( Convert.IsDBNull( value ) )
      {
        if ( type.IsValueType )
          throw new InvalidCastException( string.Format( CultureInfo.InvariantCulture, "无法将 DBNull 转换为 {0} 类型", type ) );

        return null;
      }

      return convertible.ToType( type, CultureInfo.InvariantCulture );

    }



    private static T ConvertObject<T>( object value )
    {
      return (T) value;
    }

    private static T ConvertValueType<T>( object value )
    {
      if ( value == null || Convert.IsDBNull( value ) )
        throw new InvalidCastException( string.Format( CultureInfo.InvariantCulture, "不能将 null 值转换为 {0} 类型对象", typeof( T ).FullName ) );

      return (T) value;

    }
  }
}
