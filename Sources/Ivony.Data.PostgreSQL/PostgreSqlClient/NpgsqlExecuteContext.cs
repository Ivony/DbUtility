using System.Data.Common;
using Ivony.Data.Common;
using Npgsql;

namespace Ivony.Data.PostgreSQL.PostgreSqlClient
{
	public class NpgsqlDbExecuteContext : AsyncDbExecuteContextBase
	{
		/// <summary>
		/// 创建 NpgsqlExecuteContext 对象
		/// </summary>
		/// <param name="connection">PostgreSql 数据库连接</param>
		/// <param name="dataReader">PostgreSql 数据读取器</param>
		/// <param name="tracing">用于当前查询的追踪器</param>
		public NpgsqlDbExecuteContext(NpgsqlConnection connection, DbDataReader dataReader, IDbTracing tracing)
			: base( dataReader, connection, tracing )
		{
			this.NpgsqlDataReader = dataReader;
		}

		/// <summary>
		/// 创建 NpgsqlExecuteContext 对象
		/// </summary>
		/// <param name="transaction">PostgreSql 数据库事务上下文</param>
		/// <param name="dataReader">PostgreSql 数据读取器</param>
		/// <param name="tracing">用于当前查询的追踪器</param>
		public NpgsqlDbExecuteContext(NpgsqlDbTransactionContext transaction, DbDataReader dataReader, IDbTracing tracing)
			: base(dataReader, null, tracing)
		{
			this.TransactionContext = transaction;
			this.NpgsqlDataReader = dataReader;
		}

		public DbDataReader NpgsqlDataReader { get; private set; }

		/// <summary>
		/// 数据库事务上下文，如果有的话
		/// </summary>
		public NpgsqlDbTransactionContext TransactionContext { get; private set; }
	}
}