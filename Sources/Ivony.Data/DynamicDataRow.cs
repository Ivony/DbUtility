using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
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

    public DynamicMetaObject GetMetaObject( Expression parameter )
    {
      return new DataRowMetaObject( this, parameter );
    }


    private class DataRowMetaObject : DynamicMetaObject
    {
      public DynamicDataRow DataRow { get; private set; }
      public Expression Expression { get; private set; }

      public DataRowMetaObject( DynamicDataRow row, Expression expression )
        : base( expression, BindingRestrictions.Empty )
      {
        DataRow = row;
        Expression = expression;
      }


      public override DynamicMetaObject BindGetMember( GetMemberBinder binder )
      {
        return base.BindGetMember( binder );
      }

      public override DynamicMetaObject BindGetIndex( GetIndexBinder binder, DynamicMetaObject[] indexes )
      {
        return base.BindGetIndex( binder, indexes );
      }

      public override DynamicMetaObject BindConvert( ConvertBinder binder )
      {

        if ( binder.ReturnType.IsAssignableFrom( typeof( DataRow ) ) )
          return new DynamicMetaObject( Expression, BindingRestrictions.GetTypeRestriction( Expression, typeof( DataRow ) ) );

        return base.BindConvert( binder );
      }
    }
  }
}
