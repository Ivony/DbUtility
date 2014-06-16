using System;
using System.Data;
using System.Linq;
using Ivony.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;

namespace Ivony.Data.Test
{
    [TestClass]
    public class MySqlTest
    {
        private static readonly string connectionString = @"Server=localhost;Database=Test;Uid=root;Pwd=;";
        private TestTraceService traceService;
        private MySqlDbExecutor db;
        public MySqlTest()
        {
            traceService = new TestTraceService();
            db = MySqlDb.Create(connectionString, new MySqlDbConfiguration() { TraceService = traceService });
            db.T("DROP TABLE IF EXISTS testTable").ExecuteNonQuery();
            db.T(@"
CREATE TABLE testTable(
Id int(11) NOT NULL,
Name varchar(50) DEFAULT NULL,
Content text,
PRIMARY KEY (Id)
)").ExecuteNonQuery();
        }
        [TestInitialize]
        public void Initialize()
        {
            db.T("TRUNCATE TABLE testTable").ExecuteNonQuery();
        }
        [TestMethod]
        public void StandardTest()
        {
            Assert.IsNull(db.T("SELECT * FROM testTable").ExecuteScalar(), "空数据表查询测试失败！");
            Assert.IsNull(db.T("SELECT * FROM testTable").ExecuteFirstRow(), "空数据表查询测试失败！");

            Assert.AreEqual(db.T("SELECT COUNT(*) FROM testTable").ExecuteScalar<int>(), 0, "空数据表查询测试失败");
            Assert.AreEqual(db.T("INSERT INTO testTable ( Name, Content) VALUES ( {...} )", "Ivony", "Test").ExecuteNonQuery(), 1, "插入数据测试失败");
            Assert.AreEqual(db.T("SELECT * FROM testTable").ExecuteDynamics().Length, 1, "插入数据后查询测试失败");
            Assert.IsNotNull(db.T("SELECT ID FROM testTable").ExecuteFirstRow(), "插入数据后查询测试失败");
            
            var dataItem = db.T("SELECT * FROM testTable").ExecuteDynamicObject();
            Assert.AreEqual(dataItem.Name, "Ivony", "插入数据后查询测试失败");
            Assert.AreEqual(dataItem["Content"], "Test", "插入数据后查询测试失败");
        }

        [TestMethod]
        public void TransactionTest()
        {

            using (var transaction = db.BeginTransaction())
            {
                Assert.AreEqual(transaction.T("INSERT INTO testTable ( Name, Content ) VALUES ( {...} )", "Ivony", "Test").ExecuteNonQuery(), 1, "插入数据测试失败");
                Assert.AreEqual(transaction.T("SELECT * FROM testTable").ExecuteDynamics().Length, 1, "插入数据后查询测试失败");
            }

            Assert.AreEqual(db.T("SELECT * FROM testTable").ExecuteDynamics().Length, 0, "自动回滚事务测试失败");

            using (var transaction = db.BeginTransaction())
            {
                Assert.AreEqual(transaction.T("INSERT INTO testTable ( Name, Content) VALUES ( {...} )", "Ivony", "Test").ExecuteNonQuery(), 1, "插入数据测试失败");
                Assert.AreEqual(transaction.T("SELECT * FROM testTable").ExecuteDynamics().Length,1, "插入数据后查询测试失败");

                transaction.Rollback();
            }

            Assert.AreEqual(db.T("SELECT * FROM testTable").ExecuteDynamics().Length, 0, "手动回滚事务测试失败");



            using (var transaction = db.BeginTransaction())
            {
                Assert.AreEqual(transaction.T("INSERT INTO testTable ( Name, Content) VALUES ( {...} )", "Ivony", "Test").ExecuteNonQuery(), 1, "插入数据测试失败");
                Assert.AreEqual(transaction.T("SELECT * FROM testTable").ExecuteDynamics().Length, 1, "插入数据后查询测试失败");

                transaction.Commit();
            }

            Assert.AreEqual(db.T("SELECT * FROM testTable").ExecuteDynamics().Length,1, "手动提交事务测试失败");



            {
                Exception exception = null;
                var transaction = (MySqlDbTransactionContext)db.BeginTransaction();

                try
                {
                    using (transaction)
                    {
                        transaction.T("SELECT * FROM Nothing").ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }

                Assert.IsNotNull(exception, "事务中出现异常测试失败");
                Assert.AreEqual(transaction.Connection.State, ConnectionState.Closed);
            }
        }

        [TestMethod]
        public void TraceTest()
        {

            db.T("SELECT * FROM testTable").ExecuteDataTable();

            var tracing = traceService.Last();

            var logs = tracing.GetLogEntries();
            Assert.AreEqual(logs.Length, 3);

            Assert.AreEqual(logs[0].Message, "OnExecuting");
            Assert.AreEqual(logs[1].Message, "OnLoadingData");
            Assert.AreEqual(logs[2].Message, "OnComplete");


            try
            {
                db.T("SELECT * FROM Nothing").ExecuteDynamics();
            }
            catch
            {

            }

            tracing = traceService.Last();

            logs = tracing.GetLogEntries();
            Assert.AreEqual(logs.Length, 2);

            Assert.AreEqual(logs[0].Message, "OnExecuting");
            Assert.AreEqual(logs[1].Message, "OnException");
        }
    }
}
