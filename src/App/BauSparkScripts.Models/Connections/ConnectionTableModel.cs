using System;

namespace Bau.Libraries.BauSparkScripts.Models.Connections
{
	/// <summary>
	///		Datos de una tabla
	/// </summary>
	public class ConnectionTableModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Nombre del esquema
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///		Nombre completo de la tabla
		/// </summary>
		public string FullName
		{
			get 
			{
				if (!string.IsNullOrWhiteSpace(Schema))
					return $"{Schema}.{Name}";
				else
					return Name;
			}
		}

		/// <summary>
		///		Campos
		/// </summary>
		public System.Collections.Generic.List<ConnectionTableFieldModel> Fields { get; } = new System.Collections.Generic.List<ConnectionTableFieldModel>();
	}
}
