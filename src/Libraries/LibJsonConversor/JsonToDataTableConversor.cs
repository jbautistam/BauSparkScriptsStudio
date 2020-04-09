using System;
using System.Collections.Generic;
using System.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bau.Libraries.LibJsonConversor
{
	/// <summary>
	///		Conversor de datos en Json a un DataTable
	/// </summary>
    public class JsonToDataTableConversor
    {
		/// <summary>
		///		Convierte una cadena json a una tabla de datos
		/// </summary>
		public DataTable ConvertToDataTable(string json)
		{
			try
			{
				return ConvertToDataTable(JsonConvert.DeserializeObject<List<JObject>>(json));
			}
			catch
			{
				return ConvertToDataTable(new List<JObject> { JsonConvert.DeserializeObject<JObject>(json) });
			}
		}

		/// <summary>
		///		Convierte los elementos de un json en un DataTable
		/// </summary>
		public DataTable ConvertToDataTable(IEnumerable<object> items)
		{
			DataTable table = new DataTable();

				// Recorre las filas
				if (items != null)
					foreach (object item in items)
						if (item is JObject jsonRecord && jsonRecord != null)
						{
							DataRow row = table.NewRow();

								// Añade los datos de la fila
								foreach (KeyValuePair<string, JToken> keyToken in jsonRecord)
								{
									// Crea la columna si no existía
									if (table.Columns.IndexOf(keyToken.Key) < 0)
									{
										DataColumn column = table.Columns.Add(keyToken.Key, ConvertJsonType(keyToken.Value.Type));

											// Asigna las propiedades adicionales de la columna
											column.AllowDBNull = true;
									}
									// Le asigna el valor
									row[keyToken.Key] = ConvertJsonObject(keyToken.Value.Type, keyToken.Value);
								}
								// Añade la fila
								table.Rows.Add(row);
						}
				// Devuelve la tabla creada
				return table;
		}

		/// <summary>
		///		Convierte un tipo de Json a un tipo de .Net
		/// </summary>
		private Type ConvertJsonType(JTokenType type)
		{
			switch (type)
			{ 
				case JTokenType.Integer:
					return typeof(int);
				case JTokenType.Float:
					return typeof(double);
				case JTokenType.String:
					return typeof(string);
				case JTokenType.Boolean:
					return typeof(bool);
				case JTokenType.Date:
					return typeof(DateTime);
				case JTokenType.Bytes:
					return typeof(byte[]);
				case JTokenType.Guid:
					return typeof(Guid);
				case JTokenType.Uri:
					return typeof(Uri);
				case JTokenType.TimeSpan:
					return typeof(TimeSpan);
				default: // JTokenType: None, Object, Array, Constructor, Property, Comment, Null, Undefined, Raw:
					return typeof(string);
					// throw new ArgumentException("Type unknown");
			}
		}

		/// <summary>
		///		Convierte un valor de Json en un valor .Net
		/// </summary>
		private object ConvertJsonObject(JTokenType? type, JToken value)
		{
			switch (type)
			{
				case JTokenType.Integer:
					return (int?) value;
				case JTokenType.Float:
					return (float?) value;
				case JTokenType.String:
					return (string) value;
				case JTokenType.Boolean:
					return (bool) value;
				case JTokenType.Date:
					return (DateTime?) value;
				case JTokenType.Bytes:
					return (byte[]) value;
				case JTokenType.Guid:
					return (Guid?) value;
				case JTokenType.Uri:
					return (Uri) value;
				case JTokenType.TimeSpan:
					return (TimeSpan?) value;
				case JTokenType.Null:
					return DBNull.Value;
				default:
					return null;
			}
		}
    }
}

