using System;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bau.Libraries.LibJsonConversor
{
	/// <summary>
	///		Conversor de un <see cref="DataTable"/> a una cadena JSon
	/// </summary>
    public class DataTableToJsonConversor
    {
		/// <summary>
		///		Convierte una tabla a JSon
		/// </summary>
		public string ConvertToJson(DataTable table, int startRow = 0, int rowsNumber = 0)
		{
			if (table.Rows.Count == 0)
				return ConvertHeaders(table);
			else
				return ConvertDataTable(table, startRow, rowsNumber);
		}

		/// <summary>
		///		Convierte un <see cref="DataTable"/> con datos a JSon
		/// </summary>
		private string ConvertDataTable(DataTable table, int startRow = 0, int rowsNumber = 0)
		{
			JsonSerializer js = JsonSerializer.CreateDefault();
			JArray rows = JArray.FromObject(table, js);

				// Devuelve los datos de la tabla
				if (startRow > 0 && rowsNumber > 0)
					return rows.Skip(startRow).Take(rowsNumber)?.ToString();
				else
					return rows.ToString();
		}

		/// <summary>
		///		Convierte las cabeceras de la tabla a JSon (la tabla no tiene ningún dato, el conversor a JSon devuelve una cadena vacía)
		/// </summary>
		private string ConvertHeaders(DataTable table)
		{
			var sb = new System.Text.StringBuilder();

				// Cabecera
				sb.Append("[{");
				// Convierte las cabeceras de la tabla
				foreach (DataColumn column in table.Columns)
				{
					sb.Append($"'{column.ColumnName}': null");
					if (table.Columns.IndexOf(column) < table.Columns.Count - 1)
						sb.Append(",");
				}
				// Cierre
				sb.Append("}]");
				// Devuelve la cadena
				return sb.ToString();
		}
    }
}