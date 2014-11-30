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


    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject( Expression parameter )
    {
      return new DynamicValueMetaObject( this, Expression.Convert( parameter, typeof( DynamicDataValue ) ) );
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
        expression = Expression.Property( expression, "DataValue" );
        expression = Expression.MakeBinary( binder.Operation, expression, arg.Expression );

        return new DynamicMetaObject( expression, BindingRestrictions.Empty );
      }
    }
  }
}
