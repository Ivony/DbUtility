using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data
{


  /// <summary>
  /// 动态数据值，为数据值提供 Dynamic 支持
  /// </summary>
  public class DynamicDataValue : IDynamicMetaObjectProvider
  {

    /// <summary>
    /// 数据行
    /// </summary>
    public DataRow DataRow { get; private set; }

    /// <summary>
    /// 数据列
    /// </summary>
    public DataColumn DataColumn { get; private set; }

    /// <summary>
    /// 数据值
    /// </summary>
    public object DataValue { get; private set; }



    internal DynamicDataValue( DataRow row, DataColumn column )
    {
      DataRow = row;
      DataColumn = column;

      DataValue = DataRow[DataColumn];
    }


    /// <summary>
    /// 获取强类型的数据值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns></returns>
    public T GetValue<T>()
    {
      return DbValueConverter.ConvertFrom<T>( DataValue );
    }



    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject( Expression parameter )
    {
      return new DynamicValueMetaObject( this, parameter );
    }

    private class DynamicValueMetaObject : DynamicMetaObject
    {

      public DynamicValueMetaObject( DynamicDataValue value, Expression expression ) : base( expression, BindingRestrictions.Empty, value ) { Value = value; }


      protected new DynamicDataValue Value { get; private set; }


      public override DynamicMetaObject BindConvert( ConvertBinder binder )
      {

        var expression = Expression;
        expression = Expression.Convert( expression, typeof( DynamicDataValue ) );
        expression = Expression.Call( expression, typeof( DynamicDataValue ).GetMethod( "GetValue" ).MakeGenericMethod( binder.Type ) );

        return new DynamicMetaObject( expression, BindingRestrictions.GetTypeRestriction( expression, binder.ReturnType ) );
      }

      public override DynamicMetaObject BindBinaryOperation( BinaryOperationBinder binder, DynamicMetaObject arg )
      {


        Type targetType, resultType;


        switch ( binder.Operation )
        {
          case ExpressionType.Add:
          case ExpressionType.AddAssign:
          case ExpressionType.AddAssignChecked:
          case ExpressionType.AddChecked:

          case ExpressionType.Subtract:
          case ExpressionType.SubtractAssign:
          case ExpressionType.SubtractAssignChecked:
          case ExpressionType.SubtractChecked:

          case ExpressionType.Divide:
          case ExpressionType.DivideAssign:

          case ExpressionType.Multiply:
          case ExpressionType.MultiplyAssign:
          case ExpressionType.MultiplyAssignChecked:
          case ExpressionType.MultiplyChecked:

          case ExpressionType.Negate:
          case ExpressionType.NegateChecked:

          case ExpressionType.Power:
          case ExpressionType.PowerAssign:
            targetType = resultType = arg.LimitType;

            break;



          case ExpressionType.And:
          case ExpressionType.AndAssign:

          case ExpressionType.Or:
          case ExpressionType.OrAssign:

          case ExpressionType.ExclusiveOr:
          case ExpressionType.ExclusiveOrAssign:

          case ExpressionType.LeftShift:
          case ExpressionType.LeftShiftAssign:
          case ExpressionType.RightShift:
          case ExpressionType.RightShiftAssign:

          case ExpressionType.Modulo:
          case ExpressionType.ModuloAssign:
            {
              var dataType = this.Value.DataColumn.DataType;
              if ( dataType == typeof( int ) || dataType == typeof( long ) || dataType == typeof( short ) || dataType == typeof( byte )
                || dataType == typeof( uint ) || dataType == typeof( ulong ) || dataType == typeof( ushort ) || dataType == typeof( sbyte ) )

                resultType = targetType = dataType;


              else
                resultType = targetType = null;
            }

            break;



          case ExpressionType.Not:
            {
              var dataType = this.Value.DataColumn.DataType;
              if ( dataType == typeof( int ) || dataType == typeof( long ) || dataType == typeof( short ) || dataType == typeof( byte )
                || dataType == typeof( uint ) || dataType == typeof( ulong ) || dataType == typeof( ushort ) || dataType == typeof( sbyte )
                || dataType == typeof( bool ) )

                resultType = targetType = dataType;

              else
                resultType = targetType = null;
            }

            break;



          case ExpressionType.AndAlso:
          case ExpressionType.OrElse:
            targetType = resultType = typeof( bool );

            break;



          case ExpressionType.GreaterThan:
          case ExpressionType.GreaterThanOrEqual:
          case ExpressionType.LessThan:
          case ExpressionType.LessThanOrEqual:
          case ExpressionType.Equal:
          case ExpressionType.NotEqual:
            targetType = arg.LimitType ?? typeof( object );
            resultType = typeof( bool );

            break;



          default:
            targetType = resultType = null;

            break;

        }


        if ( targetType == null || resultType == null )
        {
          var message = string.Format( "{0} 类型的值无法进行该操作", Value.DataColumn.DataType.FullName );
          Expression<Action> temp = () => new InvalidOperationException( message );
          return new DynamicMetaObject( Expression.Throw( temp.Body ), BindingRestrictions.Empty );
        }

        var expression = Expression;
        expression = Expression.Convert( expression, typeof( DynamicDataValue ) );
        expression = Expression.Call( expression, typeof( DynamicDataValue ).GetMethod( "GetValue" ).MakeGenericMethod( targetType ) );
        expression = Expression.MakeBinary( binder.Operation, expression, arg.Expression );
        expression = Expression.Convert( expression, binder.ReturnType );

        return new DynamicMetaObject( expression, BindingRestrictions.GetTypeRestriction( expression, resultType ) );
      }

    }
  }
}
