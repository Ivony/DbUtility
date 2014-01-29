using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data;

namespace Ivony.Data.Test
{
  [TestClass]
  public class DbUtilityTest
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
      Assert.AreEqual( query.ParameterValues.Length, 1, "单参数模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 123, "单参数模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = {0..0}", 123 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = #0#", "单参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 1, "单参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 123, "单参数列表模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID IN ( {0..2} )", 1, 2, 3 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN ( #0#,#1#,#2# )", "参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 3, "参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 1, "参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[1], 2, "参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[2], 3, "参数列表模板解析测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID IN ( {0..2}, {1} )", 1, 2, 3 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN ( #0#,#1#,#2#, #3# )", "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 4, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 1, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[1], 2, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[2], 3, "混合参数列表模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[3], 2, "混合参数列表模板解析测试失败" );


      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = '{{0}}'" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = '{0}'", "花括号转义测试失败" );

      query = TemplateParser.ParseTemplate( "SELECT * FROM Users WHERE ID = {{{0}}}", 1 );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = {#0#}", "花括号混合转义测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 1, "花括号混合转义测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 1, "花括号混合转义测试失败" );

    }
  }
}
