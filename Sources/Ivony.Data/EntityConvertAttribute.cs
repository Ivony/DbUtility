using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
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


  /// <summary>
  /// 指定类型所应使用的实体转换器
  /// </summary>
  [AttributeUsage( AttributeTargets.Class, Inherited = false )]
  public class EntityConvertAttribute : Attribute
  {

    private Type _convertType;

    /// <summary>
    /// 创建 EntityConvertAttribute 对象
    /// </summary>
    /// <param name="convertType">实体转换器类型</param>
    public EntityConvertAttribute( Type convertType )
    {
      _convertType = convertType;
    }

    /// <summary>
    /// 创建实体转换器实例
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>实体转换器实例</returns>
    public IEntityConverter<T> CreateConverter<T>()
    {
      return (IEntityConverter<T>) Activator.CreateInstance( _convertType );
    }

  }

  /// <summary>
  /// 定义实体转换器类型
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  public interface IEntityConverter<T>
  {
    /// <summary>
    /// 将数据写入实体
    /// </summary>
    /// <param name="dataItem">数据行</param>
    /// <returns>转换的实体对象</returns>
    T Convert( DataRow dataItem );

    /// <summary>
    /// 是否可重用
    /// </summary>
    bool IsReusable { get; }

  }
}
