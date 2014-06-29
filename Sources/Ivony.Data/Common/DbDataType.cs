using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 定义数据库字段数据类型
  /// </summary>
  public class DbDataType
  {


    /// <summary>
    /// 获取数据库类型描述符
    /// </summary>
    /// <param name="type">数据类型</param>
    /// <param name="size">最大数据大小</param>
    /// <param name="scale">数据精度</param>
    public DbDataType( DbType type, int size, byte scale )
    {
      DbType = type;
      size = Size;
      Scale = scale;
    }





    /// <summary>
    /// 字段类型
    /// </summary>
    public DbType DbType
    {
      get;
      private set;
    }

    /// <summary>
    /// 数据最大大小
    /// </summary>
    public int Size
    {
      get;
      private set;
    }

    /// <summary>
    /// 精度
    /// </summary>
    public byte Scale
    {
      get;
      private set;
    }



    /// <summary>
    /// 根据数据对象获取对应的数据类型
    /// </summary>
    /// <param name="value">要从中获取类型信息的数据对象</param>
    /// <returns>数据类型描述符</returns>
    public static DbDataType GetType( object value )
    {

      var type = value.GetType();

      if ( type == typeof( string ) )
      {
        return String( ((string) value).Length );
      }

      else if ( type == typeof( sbyte ) )
      {
        return _long;
      }

      else if ( type == typeof( byte ) )
      {
        return _byte;
      }

      else if ( type == typeof( short ) )
      {
        return _short;
      }

      else if ( type == typeof( int ) )
      {
        return _int;
      }

      else if ( type == typeof( long ) )
      {
        return _long;
      }

      else if ( type == typeof( float ) )
      {
        return _long;
      }

      else if ( type == typeof( double ) )
      {
        return _long;
      }

      else if ( type == typeof( decimal ) )
      {
        return Decimal( 17, 4 );
      }

      throw new InvalidOperationException( "不支持的数据类型" );
    }


    private static readonly DbDataType _sbyte = new DbDataType( DbType.SByte, 1, 0 );
    private static readonly DbDataType _byte = new DbDataType( DbType.Byte, 1, 0 );
    private static readonly DbDataType _short = new DbDataType( DbType.Int16, 2, 0 );
    private static readonly DbDataType _int = new DbDataType( DbType.Int32, 4, 0 );
    private static readonly DbDataType _long = new DbDataType( DbType.Int64, 8, 0 );

    private static readonly DbDataType _float = new DbDataType( DbType.Single, 4, 0 );
    private static readonly DbDataType _double = new DbDataType( DbType.Double, 8, 0 );

    private static DbDataType String( int size )
    {
      return new DbDataType( DbType.String, size, 0 );
    }

    private static DbDataType Decimal( int size, byte scale )
    {
      return new DbDataType( DbType.Decimal, size, scale );
    }
  }
}
