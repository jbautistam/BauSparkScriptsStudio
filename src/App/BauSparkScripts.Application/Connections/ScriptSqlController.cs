using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;
using Bau.Libraries.LibDbProviders.Spark.Parser;

namespace Bau.Libraries.BauSparkScripts.Application.Connections
{
	/// <summary>
	///		Controlador para el proceso de scripts de SQL
	/// </summary>
	internal class ScriptSqlController
	{
		internal ScriptSqlController(ConnectionManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Obtiene el datatable de una consulta
		/// </summary>
		internal async Task<DataTable> GetDataTableAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
														 int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
		{
			DataTable result = null;

				// Obtiene la tabla
				using (BlockLogModel block = Manager.SolutionManager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Execute query"))
				{
					if (string.IsNullOrWhiteSpace(query))
						block.Error("The query is empty");
					else
					{
						List<ScriptSqlPartModel> scripts = new ScriptSqlTokenizer().Parse(query, arguments.Constants);
						SparkSqlTools sqlTools = new SparkSqlTools();
						ParametersDbCollection parametersDb = ConvertParameters(arguments.Parameters);

							// Obtiene el datatable
							foreach (ScriptSqlPartModel script in scripts)
								if (script.Type == ScriptSqlPartModel.PartType.Sql)
								{
									string sql = sqlTools.ConvertSqlNoParameters(script.Content, parametersDb, "$").TrimIgnoreNull();

										if (!string.IsNullOrWhiteSpace(sql))
										{
											// Log
											block.Info($"Executing: {sql}");
											// Obtiene la consulta
											if (sql.TrimIgnoreNull().StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase))
											{
												if (pageSize == 0)
													result = await provider.GetDataTableAsync(sql, null, CommandType.Text, timeout, cancellationToken);
												else
													result = await provider.GetDataTableAsync(sql, null, CommandType.Text, actualPage, pageSize, timeout, cancellationToken);
											}
											else
												result = await ExecuteScalarQueryAsync(provider, sql, timeout, cancellationToken);
										}
								}
							// Log
							block.Info("End query");
					}
				}
				// Devuelve la última tabla obtenida
				return result;
		}

		/// <summary>
		///		Ejecuta una consulta escalar
		/// </summary>
		private async Task<DataTable> ExecuteScalarQueryAsync(IDbProvider provider, string query, TimeSpan timeout, CancellationToken cancellationToken)
		{
			long rows = await provider.ExecuteAsync(query, null, CommandType.Text, timeout, cancellationToken);
			DataTable table = new DataTable();
			DataRow row = table.NewRow();

				// Añade la columna
				table.Columns.Add("Rows", typeof(long));
				// Añade el valor con el número de filas
				row[0] = rows;
				// Añade la fila a la tabla
				table.Rows.Add(row);
				// Devuelve la tabla resultante
				return table;
		}

		/// <summary>
		///		Ejecuta los comandos de una cadena SQL
		/// </summary>
		internal async Task ExecuteAsync(IDbProvider provider, string sql, Models.ArgumentListModel arguments, TimeSpan timeout, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = Manager.SolutionManager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Execute script"))
			{
				if (string.IsNullOrWhiteSpace(sql))
					block.Error("The query is empty");
				else
				{
					List<ScriptSqlPartModel> scripts = new ScriptSqlTokenizer().Parse(sql, arguments.Constants);
					int scriptsExecuted = 0;

						// Ejecuta los scripts
						if (scripts.Count > 0)
							scriptsExecuted = await ExecuteCommandsAsync(provider, block, scripts, ConvertParameters(arguments.Parameters), timeout, cancellationToken);
						// Log
						if (scriptsExecuted == 0)
							block.Error("The query is empty");
						else
							block.Info($"{scriptsExecuted} command/s executed");
				}
			}
		}

		/// <summary>
		///		Ejecuta una serie de comandos
		/// </summary>
		private async Task<int> ExecuteCommandsAsync(IDbProvider provider, BlockLogModel block, List<ScriptSqlPartModel> commands, ParametersDbCollection parametersDb, 
													 TimeSpan timeout, CancellationToken cancellationToken)
		{
			int scriptsExecuted = 0;
			SparkSqlTools sqlTools = new SparkSqlTools();

				// Ejecuta las consultas
				foreach (ScriptSqlPartModel command in commands)
					if (!cancellationToken.IsCancellationRequested && command.Type == ScriptSqlPartModel.PartType.Sql)
					{
						// Log
						block.Debug($"Execute: {command.Content}");
						// Ejecuta la cadena SQL
						await provider.ExecuteAsync(sqlTools.ConvertSqlNoParameters(command.Content, parametersDb, "$"), 
													null, CommandType.Text, timeout, cancellationToken);
						// Indica que se ha ejecutado una sentencia
						scriptsExecuted++;
					}
				// Devuelve el número de comandos ejecutados
				return scriptsExecuted;
		}

		//? Está comentado porque Spark SQL no admite las cadenas ODBC con marcadores de parámetros, por eso se utiliza la rutina anterior
		///// <summary>
		/////		Ejecuta una serie de comandos utilizando parámetros de base de datos
		///// </summary>
		//private int ExecuteCommands(List<string> commands, NormalizedDictionary<object> parameters, TimeSpan timeOut)
		//{
		//	int scriptsExecuted = 0;
		//	SparkSqlTools sqlTools = new SparkSqlTools();
		//	ParametersDbCollection parametersDb = ConvertParameters(parameters);

		//		// Ejecuta los comandos
		//		using (SparkProvider provider = new SparkProvider(new SparkConnectionString(Manager.Connection.ConnectionString)))
		//		{
		//			// Abre la conexión
		//			provider.Open();
		//			// Ejecuta las consultas
		//			foreach (string command in commands)
		//			{
		//				(string sqlNormalized, ParametersDbCollection parameterNormalizedDb) = sqlTools.NormalizeSql(command, parametersDb, "$");

		//					// Ejecuta la cadena SQL
		//					provider.Execute(sqlNormalized, parameterNormalizedDb, System.Data.CommandType.Text, timeOut);
		//					// Indica que se ha ejecutado una sentencia
		//					scriptsExecuted++;
		//			}
		//		}
		//		// Devuelve el número de comandos ejecutados
		//		return scriptsExecuted;
		//}

		/// <summary>
		///		Crea la lista de parámetros a pasar a la consulta
		/// </summary>
		private ParametersDbCollection ConvertParameters(NormalizedDictionary<object> parameters)
		{
			ParametersDbCollection parametersDb = new ParametersDbCollection();

				// Convierte los parámetros
				foreach ((string key, object value) in parameters.Enumerate())
					parametersDb.Add("$" + key, value);
				// Devuelve la colección de parámetros para la base de datos
				return parametersDb;
		}

		/// <summary>
		///		Manager de conexiones
		/// </summary>
		internal ConnectionManager Manager { get; }
	}
}