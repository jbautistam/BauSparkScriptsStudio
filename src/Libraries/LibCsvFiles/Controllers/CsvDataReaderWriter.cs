using System;
using System.Collections.Generic;
using System.Data;

namespace Bau.Libraries.LibCsvFiles.Controllers
{
	/// <summary>
	///		Generador de un archivo CSV a partir de un <see cref="IDataReader"/>
	/// </summary>
	public class CsvDataReaderWriter
	{
		public CsvDataReaderWriter(Models.FileModel fileParameters = null)
		{
			FileParameters = fileParameters ?? new Models.FileModel();
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void Save(IDataReader reader, string targetFileName)
		{
			List<string> headers = GetColumns(reader);

				// Escribe los datos
				using (CsvWriter writer = new CsvWriter(new Models.FileModel()))
				{
					// Abre el archivo destino
					writer.Open(targetFileName);
					// Añade las cabeceras
					writer.WriteHeaders(headers);
					// Escribe las filas
					while (reader.Read())
					{
						List<object> values = new List<object>();

							// Asigna los valores
							for (int index = 0; index < reader.FieldCount; index++)
								values.Add(reader.GetValue(index));
							// Escribe los datos
							writer.WriteRow(values);
					}
				}
		}

		/// <summary>
		///		Obtiene las columnas
		/// </summary>
		private List<string> GetColumns(IDataReader reader)
		{
			List<string> columns = new List<string>();

				// Obtiene las columnas
				for (int index = 0; index < reader.FieldCount; index++)
					columns.Add(reader.GetName(index));
				// Devuelve la colección
				return columns;
		}

		/// <summary>
		///		Parámetros el archivo
		/// </summary>
		public Models.FileModel FileParameters { get; }
	}
}
