using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data
{
  public class DynamicDataValue : IDynamicMetaObjectProvider
  {
    public DataRow DataRow { get; private set; }
    public DataColumn DataColumn { get; private set; }
    public object DataValue { get; private set; }



    internal DynamicDataValue( DataRow row, DataColumn column )
    {
      DataRow = row;
      DataColumn = column;

      DataValue = DataRow[DataColumn];
    }



    public T GetValue<T>()
    {
      return (T) DataValue;
    }



    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject( Expression parameter )
    {
      return new DynamicValueMetaObject( this, parameter );
    }

    private class DynamicValueMetaObject : DynamicMetaObject
    {

      public DynamicValueMetaObject( DynamicDataValue value, Expression expression ) : base( expression, BindingRestrictions.Empty, value ) { }


      public override DynamicMetaObject BindConvert( ConvertBinder binder )
      {
        var type = binder.Type;
        throw new NotImplementedException();
      }

      public override DynamicMetaObject BindBinaryOperation( BinaryOperationBinder binder, DynamicMetaObject arg )
      {

        var expression = Expression;
        expression = Expression.Convert( expression, typeof( DynamicDataValue ) );
        expression = Expression.Call( expression, typeof( DynamicDataValue ).GetMethod( "GetValue" ).MakeGenericMethod( arg.LimitType ) );
        expression = Expression.MakeBinary( binder.Operation, expression, arg.Expression );
        expression = Expression.Convert( expression, typeof( object ) );

        return new DynamicMetaObject( expression, BindingRestrictions.GetTypeRestriction( expression, typeof( bool ) ) );
      }
    }
  }
}
