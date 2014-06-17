using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Ivony.Data.Common;
using Npgsql;

namespace Ivony.Data.PostgreSQL.PostgreSqlClient
{
	/// <summary>
	/// PostgreSql 数据库事务上下文对象
	/// </summary>
	public class NpgsqlDbTransactionContext : DbTransactionContextBase<NpgsqlDbExecutor, NpgsqlTransaction> 
	{
		private readonly NpgsqlDbExecutor _executor;

		internal NpgsqlDbTransactionContext(string connectionString, NpgsqlDbConfiguration configuration)
		{
			this.Connection = new NpgsqlConnection(connectionString);
			this._executor = new NpgsqlDbExecutorWithTransaction(this, configuration);
		}

		#region Overrides of DbTransactionContextBase<NpgsqlDbExecutor,NpgsqlTransaction>

		/// <summary>
		/// 派生类实现此方法以创建数据库事务对象
		/// </summary>
		/// <returns>数据库事务对象</returns>
		protected override NpgsqlTransaction CreateTransaction()
		{
			if (this.Connection.State == ConnectionState.Closed)
			{
				this.Connection.Open();
			}

			return this.Connection.BeginTransaction();
		}

		/// <summary>
		/// 获取在事务中执行查询的执行器
		/// </summary>
		public override NpgsqlDbExecutor DbExecutor
		{
			get { return this._executor; }
		}

		#endregion

		public NpgsqlConnection Connection { get; private set; }

		internal class NpgsqlDbExecutorWithTransaction : NpgsqlDbExecutor 
		{
			public NpgsqlDbExecutorWithTransaction(NpgsqlDbTransactionContext transaction, NpgsqlDbConfiguration configuration)
				: base( transaction.Connection.ConnectionString, configuration )
			{
				this.TransactionContext = transaction;
			}

			/// <summary>
			/// 当前所处的事务
			/// </summary>
			protected NpgsqlDbTransactionContext TransactionContext { get; private set; }

			/// <summary>
			/// 重写 ExecuteAsync 方法，在事务中异步执行查询
			/// </summary>
			/// <param name="command">要执行的查询命令</param>
			/// <param name="token">取消指示</param>
			/// <param name="tracing">用于追踪的追踪器</param>
			/// <returns>查询执行上下文</returns>
			protected sealed override async Task<IAsyncDbExecuteContext> ExecuteAsync(NpgsqlCommand command, CancellationToken token, IDbTracing tracing = null)
			{
				try
				{
					TryExecuteTracing(tracing, t => t.OnExecuting(command));

					command.Connection = TransactionContext.Connection;
					command.Transaction = TransactionContext.Transaction;

					var reader = await command.ExecuteReaderAsync(token);
					var context = new NpgsqlDbExecuteContext(TransactionContext, reader, tracing);

					TryExecuteTracing(tracing, t => t.OnLoadingData(context));

					return context;
				}
				catch (DbException exception)
				{
					TryExecuteTracing(tracing, t => t.OnException(exception));

					throw;
				}

			}


			/// <summary>
			/// 执行查询命令并返回执行上下文
			/// </summary>
			/// <param name="command">查询命令</param>
			/// <param name="tracing">用于追踪查询过程的追踪器</param>
			/// <returns>查询执行上下文</returns>
			protected sealed override IDbExecuteContext Execute(NpgsqlCommand command, IDbTracing tracing = null)
			{
				try
				{
					TryExecuteTracing(tracing, t => t.OnExecuting(command));

					command.Connection = TransactionContext.Connection;
					command.Transaction = TransactionContext.Transaction;

					var reader = command.ExecuteReader();
					var context = new NpgsqlDbExecuteContext(TransactionContext, reader, tracing);

					TryExecuteTracing(tracing, t => t.OnLoadingData(context));

					return context;
				}
				catch (DbException exception)
				{
					TryExecuteTracing(tracing, t => t.OnException(exception));

					throw;
				}
			}
		}
	}
}