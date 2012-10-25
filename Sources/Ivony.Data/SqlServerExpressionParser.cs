using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Ivony.Data.Expressions;

namespace Ivony.Data
{

  /// <summary>
  /// 用于 SQL Server 的表达式解析器
  /// </summary>
  public class SqlServerExpressionParser : IDbExpressionParser
  {

    SqlDbUtility _dbUtility;

    internal SqlServerExpressionParser( SqlDbUtility dbUtility )
    {
      _dbUtility = dbUtility;
    }


    /// <summary>
    /// 解析查询表达式
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <returns>解析后创建的命令对象</returns>
    public IDbCommand Parse( IDbExpression expression )
    {
      var template = expression as TemplateExpression;
      if ( template != null )
        return ParseTemplate( template );

      var table = expression as TableExpression;
      if ( table != null )
        return ParseTable( table );

      var storedProcedure = expression as StoredProcedureExpression;

      if ( storedProcedure != null )
        return ParseStoredProcedure( storedProcedure );

      throw new NotSupportedException();
    }

    private IDbCommand ParseTable( TableExpression table )
    {
      throw new NotImplementedException();
    }

    private IDbCommand ParseStoredProcedure( StoredProcedureExpression storedProcedure )
    {
      var command = CreateCommand();

      command.CommandType = CommandType.StoredProcedure;
      command.CommandText = storedProcedure.Name;
      foreach ( var pair in storedProcedure.Parameters )
      {
        var name = pair.Key;
        if ( !name.StartsWith( "@" ) )
          name = "@" + name;

        command.Parameters.AddWithValue( name, pair.Value ?? DBNull.Value );
      }

      return command;
    }


    private IDbCommand ParseTemplate( TemplateExpression template )
    {
      var command = CreateCommand();

      var context = new SqlTemplateParseContext();
      command.CommandText = template.Parse( context );

      foreach ( var p in context.GetParameters() )
        command.Parameters.Add( p );

      return command;
    }

    private SqlCommand CreateCommand()
    {
      return _dbUtility.CreateCommand();
    }


    private class SqlTemplateParseContext : TemplateParseContext
    {

      private List<SqlParameter> list = new List<SqlParameter>();
      private object sync = new object();

      public override string CreateParameterExpression( ParameterExpression expression )
      {
        lock ( sync )
        {
          var name = "@Param" + list.Count;
          var parameter = CreateParameter( expression, name );
          list.Add( parameter );
          return name;
        }
      }

      public SqlParameter CreateParameter( ParameterExpression expression, string name )
      {
        return new SqlParameter( name, expression.Value ?? DBNull.Value );
      }


      public SqlParameter[] GetParameters()
      {
        return list.ToArray();
      }
    }


  }
}
