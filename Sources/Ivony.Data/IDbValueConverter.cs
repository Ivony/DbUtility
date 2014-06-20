using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 定义数据值类型转换器
  /// </summary>
  /// <typeparam name="T">要转换的类型</typeparam>
  public interface IDbValueConverter<T>
  {

    /// <summary>
    /// 从数据对象转换为特定类型
    /// </summary>
    /// <param name="dataValue">数据值</param>
    /// <param name="dbDataType">数据库数据类型</param>
    /// <returns>目标类型的值</returns>
    T ConvertValueFrom( object dataValue );


    /// <summary>
    /// 转换目标类型值为数据
    /// </summary>
    /// <param name="value">特定类型的值</param>
    /// <param name="dbDataType">参考的数据库数据类型（如果有的话）</param>
    /// <returns>数据值</returns>
    object ConvertValueTo( T value );


  }
}
