using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.DatabaseStructure
{

  /// <summary>
  /// 代表数据库的一个字段
  /// </summary>
  public sealed class DataField
  {

    public DataField( DataBase database, string tableName, string fieldName )
    {
      _fieldType = database.GetFieldType( tableName, fieldName );

      TableName = tableName;
      FieldName = fieldName;
    }


    private Type _fieldType;

    public Type ValueType { get { return _fieldType; } }


    public string FieldName { get; private set; }
    public string TableName { get; private set; }



  }
}
