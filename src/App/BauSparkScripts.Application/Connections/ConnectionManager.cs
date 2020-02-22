using System;
using System.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDbProviders.Spark;
using Bau.Libraries.BauSparkScripts.Models.Connections;
using Bau.Libraries.LibDbProviders.Base.Schema;

namespace Bau.Libraries.BauSparkScripts.Application.Connections
{
	/// <summary>
	///		Manager de conexiones
	/// </summary>
	public class ConnectionManager
	{
		public ConnectionManager(SolutionManager solutionManager, ConnectionModel connection)
		{
			SolutionManager = solutionManager;
			Connection = connection;
		}

		/// <summary>
		///		Carga el esquema de la conexión
		/// </summary>
		public void LoadSchema()
		{
			// Limpia las tablas de la conexión
			Connection.Tables.Clear();
			// Carga el esquema del proveedor
			using (SparkProvider provider = new SparkProvider(new SparkConnectionString(Connection.ConnectionString)))
			{
				SchemaDbModel schema = provider.GetSchema();

					// Agrega los campos
					foreach (TableDbModel tableSchema in schema.Tables)
					{
						ConnectionTableModel table = new ConnectionTableModel();

							// Asigna las propiedades
							table.Name = tableSchema.Name;
							table.Description = tableSchema.Description;
							table.Schema = tableSchema.Schema;
							// Asigna los campos
							foreach (FieldDbModel fieldSchema in tableSchema.Fields)
							{
								ConnectionTableFieldModel field = new ConnectionTableFieldModel();

									// Asigna las propiedades
									field.Name = fieldSchema.Name;
									field.Description = fieldSchema.Description;
									field.Type = fieldSchema.Type.ToString();
									field.Length = fieldSchema.Length;
									field.IsRequired = fieldSchema.IsRequired;
									field.IsKey = fieldSchema.IsKey;
									field.IsIdentity = fieldSchema.IsIdentity;
									// Añade el campo
									table.Fields.Add(field);
							}
							// Añade la tabla a la colección
							Connection.Tables.Add(table);
					}
			}
		}

		/// <summary>
		///		Ejecuta una consulta
		/// </summary>
		public DataTable GetDatatableQuery(string query, TimeSpan? timeOut)
		{	
			if (IsDataQuery(query))
				return ExecuteDataQuery(query, timeOut);
			else
				return ExecuteScalarQuery(query, timeOut);
		}

		/// <summary>
		///		Comprueba si es una consulta de datos
		/// </summary>
		private bool IsDataQuery(string query)
		{
			return query.TrimIgnoreNull().StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Ejecuta una consulta de datos
		/// </summary>
		private DataTable ExecuteDataQuery(string query, TimeSpan? timeOut)
		{
			DataTable result = null;

				// Carga los datos
				using (SparkProvider provider = new SparkProvider(new SparkConnectionString(Connection.ConnectionString)))
				{
					// Abre la conexión
					provider.Open();
					// Ejecuta la conexión
					result = provider.GetDataTable(query, null, CommandType.Text, timeOut);
				}
				// Devuelve la tabla cargada
				return result;
		}

		/// <summary>
		///		Ejecuta una consulta escalar
		/// </summary>
		private DataTable ExecuteScalarQuery(string query, TimeSpan? timeOut)
		{
			long rows = 0;

				// Ejecuta la consulta
				using (SparkProvider provider = new SparkProvider(new SparkConnectionString(Connection.ConnectionString)))
				{
					// Abre la conexión
					provider.Open();
					// Ejecuta la consulta
					rows = provider.Execute(query, null, CommandType.Text, timeOut);
				}
				// Añade el resultado a la tabla
				return CreateTable("Rows", rows);
		}

		/// <summary>
		///		Crea una tabla con información de campos
		/// </summary>
		private DataTable CreateTable(string field, long rows)
		{
			DataTable table = new DataTable();
			DataRow row = table.NewRow();

				// Añade la columna
				table.Columns.Add(field, typeof(long));
				// Añade el valor con el número de filas
				row[0] = rows;
				// Añade la fila a la tabla
				table.Rows.Add(row);
				// Devuelve la tabla resultante
				return table;
		}

		/// <summary>
		///		Ejecuta una consulta
		/// </summary>
		public void ExecuteQuery(string query, LibDataStructures.Collections.NormalizedDictionary<object> parameters, TimeSpan timeOut)
		{	
			new ScriptSqlController(this).Execute(query, parameters, timeOut);
		}

		/// <summary>
		///		Manager de la solución
		/// </summary>
		public SolutionManager SolutionManager { get; }

		/// <summary>
		///		Datos de la conexión
		/// </summary>
		public ConnectionModel Connection { get; }
	}
}
