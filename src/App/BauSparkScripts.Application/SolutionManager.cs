using System;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Core;
using Bau.Libraries.BauSparkScripts.Models;
using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.Application
{
	/// <summary>
	///		Manager de soluciones
	/// </summary>
	public class SolutionManager
	{
		public SolutionManager(LogManager logger, string pathConfiguration)
		{
			Logger = logger;
			PathConfiguration = pathConfiguration;
		}

		/// <summary>
		///		Carga los datos de configuración
		/// </summary>
		public SolutionModel LoadConfiguration()
		{
			return new Repository.SolutionRepository().Load(GetConfigurationFileName());
		}

		/// <summary>
		///		Graba los datos de una solución
		/// </summary>
		public void SaveSolution(SolutionModel solution)
		{
			new Repository.SolutionRepository().Save(solution, GetConfigurationFileName());
		}

		/// <summary>
		///		Obtiene el nombre del archivo de configuración
		/// </summary>
		private string GetConfigurationFileName()
		{
			// Obtiene el directorio de configuración si no existía
			if (string.IsNullOrWhiteSpace(PathConfiguration))
				PathConfiguration = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			// Devuevle el nombre del archivo de configuración
			return System.IO.Path.Combine(PathConfiguration, "Studio.Configuration.xml");
		}

		/// <summary>
		///		Carga el esquema de una conexión
		/// </summary>
		public void LoadSchema(ConnectionModel connection)
		{
			new Connections.ConnectionManager(this, connection).LoadSchema();
		}

		/// <summary>
		///		Ejecuta una consulta sobre una conexión
		/// </summary>
		public async Task ExecuteQueryAsync(ConnectionModel connection, string query, LibDataStructures.Collections.NormalizedDictionary<object> parameters, TimeSpan timeOut)
		{
			await Task.Run(() => new Connections.ConnectionManager(this, connection).ExecuteQuery(query, parameters, timeOut));
		}

		/// <summary>
		///		Obtiene un <see cref="System.Data.DataColumn"/> con una consulta sobre una conexión
		/// </summary>
		public System.Data.DataTable GetDatatableQuery(ConnectionModel connection, string query, TimeSpan? timeOut)
		{
			return new Connections.ConnectionManager(this, connection).GetDatatableQuery(query, timeOut);
		}

		/// <summary>
		///		Exporta un directorio de archivos al formato de notebooks de Databricks
		/// </summary>
		public void ExportToDataBricks(string path, string targetPath, LibDataStructures.Collections.NormalizedDictionary<object> parameters)
		{
			new Controllers.Databricks.DatabrickExporter(this).Export(path, targetPath, parameters);
		}

		/// <summary>
		///		Manager de log
		/// </summary>
		public LogManager Logger { get; }

		/// <summary>
		///		Directorio de configuración
		/// </summary>
		public string PathConfiguration { get; private set; }
	}
}
