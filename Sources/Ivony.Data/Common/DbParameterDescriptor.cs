using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{


  /// <summary>
  /// 定义一个参数化查询的参数描述符
  /// </summary>
  public sealed class DbParameterDescriptor
  {


    /// <summary>
    /// 创建 DbParameterDescriptor 对象
    /// </summary>
    /// <param name="name">可选的参数名称</param>
    /// <param name="value">参数值</param>
    /// <param name="type">参数数据类型</param>
    /// <param name="direction">参数传输方向</param>
    /// <param name="valueChangedCallback">当输出参数值改变时应当回调的通知方法</param>
    public DbParameterDescriptor( string name, object value, DbDataType type, ParameterDirection direction, Action<DbParameterDescriptor, object> valueChangedCallback = null )
    {
      Name = name;
      DbDataType = type;
      Direction = direction;


      if ( direction != ParameterDirection.Input && direction != ParameterDirection.InputOutput && direction != ParameterDirection.Output )
        throw new ArgumentException( "direction 的值只能是 Input、InputOutput 和 Output 中的一个", "direction" );


      if ( direction == ParameterDirection.Input || direction == ParameterDirection.InputOutput )
      {
        if ( value == null )
          value = DBNull.Value;
      }



      if ( direction == ParameterDirection.InputOutput || direction == ParameterDirection.Output )
      {

        callback = valueChangedCallback;//设置参数值回调
      }
    }



    private Action<DbParameterDescriptor, object> callback;


    /// <summary>
    /// 可选的参数名称，如果没有则会自动创建一个
    /// </summary>
    public string Name
    {
      get;
      private set;
    }

    /// <summary>
    /// 参数值
    /// </summary>
    public object Value
    {
      get;
      private set;
    }


    /// <summary>
    /// 数据类型描述
    /// </summary>
    public DbDataType DbDataType
    {
      get;
      private set;
    }

    /// <summary>
    /// 参数传输方向，定义是输入或输出参数
    /// </summary>
    public ParameterDirection Direction
    {
      get;
      private set;
    }

    /// <summary>
    /// 设置参数值
    /// </summary>
    /// <param name="value">参数值</param>
    internal void SetValue( object value )
    {
      Contract.Assert( Direction == ParameterDirection.InputOutput || Direction == ParameterDirection.Output );

      Value = value;
      callback( this, value );
    }
  }
}
