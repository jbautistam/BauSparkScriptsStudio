using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibDbProviders.Base.Parameters;
using Bau.Libraries.LibDbProviders.Spark;

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
		///		Ejecuta los comandos de una cadena SQL
		/// </summary>
		internal void Execute(string sql, NormalizedDictionary<object> parameters, TimeSpan timeOut)
		{
			using (BlockLogModel block = Manager.SolutionManager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Execute script"))
			{
				if (string.IsNullOrWhiteSpace(sql))
					block.Error("The query is empty");
				else
				{
					List<ScriptSqlPartModel> scripts = new ScriptSqlTokenizer().Parse(sql, parameters);
					int scriptsExecuted = 0;

						// Ejecuta los scripts
						if (scripts.Count > 0)
							scriptsExecuted = ExecuteCommands(block, scripts, ConvertParameters(parameters), timeOut);
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
		private int ExecuteCommands(BlockLogModel block, List<ScriptSqlPartModel> commands, ParametersDbCollection parametersDb, TimeSpan timeOut)
		{
			int scriptsExecuted = 0;
			SparkSqlTools sqlTools = new SparkSqlTools();

				// Ejecuta los comandos
				using (SparkProvider provider = new SparkProvider(new SparkConnectionString(Manager.Connection.ConnectionString)))
				{
					// Abre la conexión
					provider.Open();
					// Ejecuta las consultas
					foreach (ScriptSqlPartModel command in commands)
						if (command.Type == ScriptSqlPartModel.PartType.Sql)
						{
							// Log
							block.Debug($"Execute: {command.Content}");
							// Ejecuta la cadena SQL
							provider.Execute(sqlTools.ConvertSqlNoParameters(command.Content, parametersDb, "$"), 
											 null, System.Data.CommandType.Text, timeOut);
							// Indica que se ha ejecutado una sentencia
							scriptsExecuted++;
						}
						else
							block.Debug($"Comment: {command.Content}");
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
		public ConnectionManager Manager { get; }
	}
}
