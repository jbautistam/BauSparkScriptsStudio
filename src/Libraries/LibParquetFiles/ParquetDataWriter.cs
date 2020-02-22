using System;
using System.Collections.Generic;

using Parquet;
using Parquet.Data;
using Parquet.Data.Rows;

namespace Bau.Libraries.LibParquetFiles
{
	/// <summary>
	///		Clase de escritura sobre archivos Parquet
	/// </summary>
	public class ParquetDataWriter
	{
		// Eventos públicos
		public event EventHandler<EventArguments.AffectedEvntArgs> WriteBlock;
		/// <summary>
		///		Tipo del campo
		/// </summary>
		private enum FieldType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Valor lógico</summary>
			Boolean,
			/// <summary>Fecha / hora</summary>
			DateTime,
			/// <summary>Byte</summary>
			Byte,
			/// <summary>Entero</summary>
			Integer,
			/// <summary>Entero largo</summary>
			Long,
			/// <summary>Decimal</summary>
			Decimal,
			/// <summary>doble</summary>
			Double,
			/// <summary>Cadena</summary>
			String
		}
		private TimeZoneInfo _timeZoneCentral;

		public ParquetDataWriter(string fileName, int notifyAfter = 200_000)
		{
			FileName = fileName;
			NotifyAfter = notifyAfter;
		}

		/// <summary>
		///		Procesa una exportación de una consulta a un archivo parquet
		/// </summary>
		public long Write(System.Data.IDataReader reader, int rowGroupSize = 45_000)
		{
			long records = 0;
			List<(string, FieldType)> columns = GetColumnsSchema(reader);

				// Obtiene la información de zona horario
				try
				{
					_timeZoneCentral = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
				}
				catch
				{
					_timeZoneCentral = TimeZoneInfo.Local;
				}
				// Escribe en el archivo
				using (System.IO.FileStream stream = System.IO.File.Open(FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					Schema schema = GetSchema(columns);

						using (ParquetWriter writer = new ParquetWriter(schema, stream))
						{
							Table table = new Table(schema);

								// Establece el método de compresión
								//? No asigna el nivel de compresión: deja el predeterminado para el método
								writer.CompressionMethod = CompressionMethod.Snappy;
								// Carga los registros y los va añadiendo a la lista para meterlos en un grupo de filas
								while (reader.Read())
								{
									// Escribe la tabla en el archivo si se ha superado el número máximo de filas
									if (table.Count >= rowGroupSize)
									{
										// Escribe la tabla en un grupo de filas
										FlushRowGroup(writer, table);
										// Limpia la tabla
										table.Clear();
									}
									// Añade los datos del registro a la lista
									table.Add(ConvertData(reader, columns));
									// Lanza el evento de progreso
									if (++records % NotifyAfter == 0)
										WriteBlock?.Invoke(this, new EventArguments.AffectedEvntArgs(records));
								}
								// Graba las últimas filas
								FlushRowGroup(writer, table);
						}
				}
				// Devuelve el número de registros escritos
				return records;
		}

		/// <summary>
		///		Escribe un grupo de filas en el archivo
		/// </summary>
		private void FlushRowGroup(ParquetWriter writer, Table table)
		{
			if (table.Count > 0)
				using (ParquetRowGroupWriter rowGroupWriter = writer.CreateRowGroup())
				{
					rowGroupWriter.Write(table);
				}
		}

		/// <summary>
		///		Obtiene las columnas asociadas al <see cref="System.Data.IDataReader"/>
		/// </summary>
		private List<(string, FieldType)> GetColumnsSchema(System.Data.IDataReader reader)
		{
			List<(string, FieldType)> columns = new List<(string, FieldType)>();
			System.Data.DataTable schema = reader.GetSchemaTable();

				// Obtiene las columnas del dataReader
				foreach (System.Data.DataRow dataRow in schema.Rows)
				{
					(string name, FieldType type) column = (string.Empty, FieldType.Unknown);

						// Busca las propiedades en las columnas
						foreach (System.Data.DataColumn readerColumn in schema.Columns)
							if (readerColumn.ColumnName.Equals("ColumnName", StringComparison.CurrentCultureIgnoreCase))
								column.name = dataRow[readerColumn].ToString();
							else if (readerColumn.ColumnName.Equals("DataType", StringComparison.CurrentCultureIgnoreCase))
								column.type = GetColumnSchemaType((Type) dataRow[readerColumn]);
						// Añade la columna a la lista
						if (!string.IsNullOrWhiteSpace(column.name) && column.type != FieldType.Unknown)
							columns.Add(column);
				}
				// Devuelve la colección de columnas
				return columns;
		}

		/// <summary>
		///		Obtiene el tipo de columna
		/// </summary>
		private FieldType GetColumnSchemaType(Type dataType)
		{
			if (IsDataType(dataType, "byte[]")) // ... no vamos a convertir los arrays de bytes
				return FieldType.Unknown;
			else if (IsDataType(dataType, "int64"))
				return FieldType.Long;
			else if (IsDataType(dataType, "byte"))
				return FieldType.Byte;
			else if (IsDataType(dataType, "int")) // int, int16, int32, int64
				return FieldType.Integer;
			else if (IsDataType(dataType, "decimal"))
				return FieldType.Decimal;
			else if (IsDataType(dataType, "double") || IsDataType(dataType, "float"))
				return FieldType.Double;
			else if (IsDataType(dataType, "date"))
				return FieldType.DateTime;
			else if (IsDataType(dataType, "bool"))
				return FieldType.Boolean;
			else
				return FieldType.String;
		}

		/// <summary>
		///		Comprueba si un nombre de tipo contiene un valor determinado
		/// </summary>
		private bool IsDataType(Type dataType, string search)
		{
			return dataType.FullName.IndexOf("." + search, StringComparison.CurrentCultureIgnoreCase) >= 0;
		}

		/// <summary>
		///		Obtiene el esquema Parquet a partir del dataReader
		/// </summary>
		private Schema GetSchema(List<(string name, FieldType type)> columns)
		{
			Field[] fields = new Field[columns.Count];

				// Obtiene los campos (indica que todos admiten nulos)
				for (int index = 0; index < columns.Count; index++)
					if (columns[index].type == FieldType.DateTime)
						fields[index] = new DateTimeDataField(columns[index].name, DateTimeFormat.Date);
					else
						fields[index] = new DataField(columns[index].name, ConvertType(columns[index].type), true);
				// Devuelve la colección de campos
				return new Schema(fields);
		}

		/// <summary>
		///		Convierte el tipo de datos
		/// </summary>
		private DataType ConvertType(FieldType type)
		{
			switch (type)
			{
				case FieldType.Boolean:
					return DataType.Boolean;
				case FieldType.Decimal:
					return DataType.Decimal;
				case FieldType.Double:
					return DataType.Double;
				case FieldType.Byte: // ... los byte no se transforman en enteros por un problema en el intérprete de Spark
					// return DataType.Byte;
				case FieldType.Integer:
					return DataType.Int32;
				case FieldType.Long:
					return DataType.Int64;
				default:
					return DataType.String;
			}
		}

		/// <summary>
		///		Convierte los datos de un campo
		/// </summary>
		private Row ConvertData(System.Data.IDataReader reader, List<(string name, FieldType type)> columns)
		{
			List<object> values = new List<object>();

				// Obtiene los campos del registro
				for (int index = 0; index < columns.Count; index++)
				{
					object value;

						// Obtiene el valor del campo
						if (!reader.IsDBNull(index))
							value = reader.GetValue(index);
						else
							value = null;
						// Convierte el dato del campo
						switch (columns[index].type)
						{
							case FieldType.Boolean:
									values.Add(value as bool?);
								break;
							case FieldType.Byte:
									values.Add((int) (value as byte?));
								break;
							case FieldType.Integer:
									values.Add(value as int?);
								break;
							case FieldType.Long:
									values.Add(value as long?);
								break;
							case FieldType.Decimal:
									values.Add(value as decimal?);
								break;
							case FieldType.Double:
									values.Add(value as double?);
								break;
							case FieldType.DateTime:
									if (value == null)
										values.Add(null as DateTimeOffset?);
									else
									{
										DateTime date = Convert.ToDateTime(value);

											values.Add(new DateTimeOffset(date, _timeZoneCentral.GetUtcOffset(date)));
									}
								break;
							default:
									values.Add(value?.ToString());
								break;
						}
				}
				// Devuelve la fila de valores
				return new Row(values);
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; }

		/// <summary>
		///		Número de registros después de los que se debe notificar
		/// </summary>
		public int NotifyAfter { get; }
	}
}