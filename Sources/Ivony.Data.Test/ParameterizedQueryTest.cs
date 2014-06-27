using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data;

namespace Ivony.Data.Test
{
  [TestClass]
  public class ParameterizedQueryTest
  {
    [TestMethod]
    public void TemplateParserTest()
    {

      var query = TemplateParser.ParseTemplate( "SELECT * FROM Users" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users", "无参数模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = '#123'" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = '##123'", "含有转义符的无参数模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = {0}", 123 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = #0#", "单参数模板解析测试失败" );
      Assert.AreEqual( query.Parameters.Count, 1, "单参数模板解析测试失败" );
      Assert.AreEqual( query.Parameters[0].Value, 123, "单参数模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = {0..0}", 123 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = #0#", "单参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters.Count, 1, "单参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[0].Value, 123, "单参数列表模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID IN ( {0..2} )", 1, 2, 3 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN ( #0#,#1#,#2# )", "参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters.Count, 3, "参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[0].Value, 1, "参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[1].Value, 2, "参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[2].Value, 3, "参数列表模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID IN ( {0..2}, {1} )", 1, 2, 3 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN ( #0#,#1#,#2#, #3# )", "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters.Count, 4, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[0].Value, 1, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[1].Value, 2, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[2].Value, 3, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.Parameters[3].Value, 2, "混合参数列表模板解析测试失败" );


      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = '{{0}}'" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = '{0}'", "花括号转义测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = {{{0}}}", 1 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = {#0#}", "花括号混合转义测试失败" );
      Assert.AreEqual( query.Parameters.Count, 1, "花括号混合转义测试失败" );
      Assert.AreEqual( query.Parameters[0].Value, 1, "花括号混合转义测试失败" );


      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = ( {0} )", Db.T( "SELECT ID FROM Users WHERE Username = {0}", "Ivony" ) );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = ( SELECT ID FROM Users WHERE Username = #0# )", "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.Parameters.Count, 1, "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.Parameters[0].Value, "Ivony", "一个参数化查询作为另一个参数化查询参数测试失败" );


      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = ( {1} ) AND Status = {0}", 3, Db.T( "SELECT ID FROM Users WHERE Username = {0}", "Ivony" ) );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = ( SELECT ID FROM Users WHERE Username = #0# ) AND Status = #1#", "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.Parameters.Count, 2, "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.Parameters[0].Value, "Ivony", "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.Parameters[1].Value, 3, "一个参数化查询作为另一个参数化查询参数测试失败" );

    }

    [TestMethod]
    public void ConcatTest()
    {

      var query = TemplateParser.ParseTemplate( "SELECT * FROM Users;" );

      Assert.AreEqual( (query + query).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users;", "两个纯文本模板连接测试失败" );
      Assert.AreEqual( query.Concat( query ).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users;", "两个纯文本模板连接测试失败" );


      Assert.AreEqual( (query + query + query).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users; SELECT * FROM Users;", "多个纯文本模板连接测试失败" );
      Assert.AreEqual( query.Concat( query, query ).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users; SELECT * FROM Users;", "多个纯文本模板连接测试失败" );


      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE UserID = {0};", 1 );

      Assert.AreEqual( (query + query + query).TextTemplate, "SELECT * FROM Users WHERE UserID = #0#; SELECT * FROM Users WHERE UserID = #1#; SELECT * FROM Users WHERE UserID = #2#;", "多个带参数模板连接测试失败" );
      Assert.AreEqual( query.Concat( query, query ).TextTemplate, "SELECT * FROM Users WHERE UserID = #0#; SELECT * FROM Users WHERE UserID = #1#; SELECT * FROM Users WHERE UserID = #2#;", "多个带参数模板连接测试失败" );


    }
  }
}
