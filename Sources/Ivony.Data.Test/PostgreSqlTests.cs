using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.PostgreSQL;
using Ivony.Data.PostgreSQL.PostgreSqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace Ivony.Data.Test
{
    //[TestClass]
    public class PostgreSqlTests
    {
        private NpgsqlDbExecutor _db;

        [TestInitialize]
        public void Initialize()
        {
            //this._db = PostgreSql.Create("Server=127.0.0.1;Port=5432;Database=InventoryDB;User Id=inventory_user;Password=sa", new NpgsqlDbConfiguration());
            this._db = PostgreSql.Connect("127.0.0.1", "InventoryDB", 5432, "inventory_user", "sa");
        }

        [TestMethod]
        public void NpgsqlTest()
        {
            //var dt = this._db.T("SELECT * FROM \"J_Log\" LIMIT 1 OFFSET 0").ExecuteDataTable();

            //Assert.IsNotNull(dt);
            //Assert.IsTrue(dt.Rows.Count == 1);

            //var affectedRows = this._db.T("INSERT INTO \"J_Log\" (\"LoginUser\",\"CreationDate\",\"Action\") VALUES ( {...} )", "oger", DateTime.Now, 1).ExecuteNonQuery();
            
            //Assert.AreEqual(affectedRows, 1, "插入数据失败");

            var dr = this._db.T("SELECT \"Id\" FROM \"J_Log\"").ExecuteFirstRow();

            Assert.IsNotNull(dr, "插入数据后查询测试失败");

            var dynamicVar = this._db.T("SELECT * FROM \"J_Log\"").ExecuteDynamics();
            
            Assert.AreEqual(dynamicVar.Length, 61, "插入数据后查询测试失败");
        }

        [TestMethod]
        public async Task NpgsqlAsyncTest()
        {
            var dt = await this._db.T("SELECT * FROM \"J_Log\" LIMIT 1 OFFSET 0").ExecuteDataTableAsync();

            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count == 1);

            //var affectedRows = await this._db.T("INSERT INTO \"J_Log\" (\"LoginUser\",\"CreationDate\",\"Action\") VALUES ( {...} )", "oger", DateTime.Now, 1).ExecuteNonQueryAsync();

            //Assert.AreEqual(affectedRows, 1, "插入数据失败");

            var dr = await this._db.T("SELECT \"Id\" FROM \"J_Log\"").ExecuteFirstRowAsync();

            Assert.IsNotNull(dr, "插入数据后查询测试失败");

            var dynamicVar = await this._db.T("SELECT * FROM \"J_Log\"").ExecuteDynamicsAsync();

            Assert.AreEqual(dynamicVar.Length, 62, "插入数据后查询测试失败");
        }

        [TestMethod]
        public void NpgsqlTransactionTest()
        {
            //using (var tran = this._db.BeginTransaction())
            //{
            //  var beforTranCount = this._db.T("SELECT \"count\"(\"Id\") FROM \"J_Log\"").ExecuteScalar<int>();
            //  var affectedRows = this._db.T("INSERT INTO \"J_Log\" (\"LoginUser\",\"CreationDate\",\"Action\") VALUES ( {...} )", "oger", DateTime.Now, 1).ExecuteNonQuery();

            //  tran.Commit();

            //  var afterCommitCount = this._db.T("SELECT \"count\"(\"Id\") FROM \"J_Log\"").ExecuteScalar<int>();

            //  Assert.IsTrue(affectedRows == 1, "插入数据失败");
            //  Assert.IsTrue(afterCommitCount - beforTranCount == 1, "提交事务失败");
            //}



            var beforTranCount = this._db.T("SELECT \"count\"(\"Id\") FROM \"J_Log\"").ExecuteScalar<int>();

            using (var tran = this._db.BeginTransaction())
            {
                var affectedRows = tran.T("INSERT INTO \"J_Log\" (\"LoginUser\",\"CreationDate\",\"Action\") VALUES ( {...} )", "oger", DateTime.Now, 1).ExecuteNonQuery();

                tran.Rollback();
                //tran.Commit();
            }

            var afterCommitCount = this._db.T("SELECT \"count\"(\"Id\") FROM \"J_Log\"").ExecuteScalar<int>();

            Assert.IsTrue(afterCommitCount == beforTranCount, "事务回滚失败");
        }

        [TestMethod]
        public void MyTestTransaction()
        {
            using (var conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=InventoryDB;User Id=inventory_user;Password=sa"))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    var sql = "INSERT INTO \"J_Log\" (\"LoginUser\",\"CreationDate\",\"Action\") VALUES ( 'oger-me','2014-06-14 12:38:00', 1 )";
                    var cmd = new NpgsqlCommand(sql, conn);
                    
                    cmd.Transaction = tran;

                    var affectedRows = cmd.ExecuteNonQuery();

                    Trace.WriteLine(affectedRows);

                    tran.Rollback();
                }
            }
        }
    }

    
}
