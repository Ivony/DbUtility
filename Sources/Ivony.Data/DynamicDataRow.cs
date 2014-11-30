using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public class DynamicDataRow : IDynamicMetaObjectProvider
  {


    public DataRow DataRow { get; private set; }

    public DynamicDataRow( DataRow dataRow )
    {
      DataRow = dataRow;
    }



    public DynamicDataValue GetValue( string name, bool ignoreCase )
    {
      var column = DataRow.Table.Columns[name];
      if ( column == null )
        return null;

      return new DynamicDataValue( DataRow, column );
    }


    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject( Expression parameter )
    {
      return new DataRowMetaObject( this, parameter );
    }


    private class DataRowMetaObject : DynamicMetaObject
    {
      public DynamicDataRow DataRow { get; private set; }
      public Expression Expression { get; private set; }

      public DataRowMetaObject( DynamicDataRow row, Expression expression )
        : base( expression, BindingRestrictions.Empty, row )
      {
        DataRow = row;
        Expression = expression;
      }





      private static readonly MethodInfo GetValueMethod = typeof( DynamicDataRow ).GetMethod( "GetValue" );



      public override DynamicMetaObject BindGetMember( GetMemberBinder binder )
      {

        var expression = Expression;
        expression = Expression.Convert( expression, typeof( DynamicDataRow ) );
        expression = Expression.Call( expression, GetValueMethod, Expression.Constant( binder.Name, typeof( string ) ), Expression.Constant( binder.IgnoreCase, typeof( bool ) ) );

        return new DynamicMetaObject( expression, BindingRestrictions.GetTypeRestriction( Expression, typeof( DynamicDataRow ) ) );
      }

      public override DynamicMetaObject BindGetIndex( GetIndexBinder binder, DynamicMetaObject[] indexes )
      {

        if ( indexes.Length != 1 || indexes[0].LimitType != typeof( string ) )
          return null;

        var name = indexes[0];
        return new DynamicMetaObject( Expression.Call( Expression, GetValueMethod, name.Expression ), BindingRestrictions.Empty );
      }

      public override DynamicMetaObject BindConvert( ConvertBinder binder )
      {

        if ( binder.ReturnType.IsAssignableFrom( typeof( DataRow ) ) )
        {
          var expression = Expression.Property( Expression, "DataRow" );
          return new DynamicMetaObject( expression, BindingRestrictions.Empty );
        }

        return base.BindConvert( binder );
      }
    }
  }
}
