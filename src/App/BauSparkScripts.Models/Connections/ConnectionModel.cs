using System;

namespace Bau.Libraries.BauSparkScripts.Models.Connections
{
	/// <summary>
	///		Clase con los datos de la conexión
	/// </summary>
	public class ConnectionModel : LibDataStructures.Base.BaseExtendedModel
	{
		public ConnectionModel(SolutionModel solution)
		{
			Solution = solution;
		}

		/// <summary>
		///		Solución a la que se asocia la conexión
		/// </summary>
		public SolutionModel Solution { get; }

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		///		Tablas
		/// </summary>
		public System.Collections.Generic.List<ConnectionTableModel> Tables { get; } = new System.Collections.Generic.List<ConnectionTableModel>();
	}
}
