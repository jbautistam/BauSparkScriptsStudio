﻿using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDbProviders.Spark;
using Bau.Libraries.BauSparkScripts.Models.Connections;
using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.BauSparkScripts.Application.Connections
{
	/// <summary>
	///		Manager de conexiones
	/// </summary>
	internal class ConnectionManager
	{
		internal ConnectionManager(SolutionManager solutionManager)
		{
			SolutionManager = solutionManager;
		}

		/// <summary>
		///		Carga el esquema de la conexión
		/// </summary>
		internal async Task LoadSchemaAsync(ConnectionModel connection, CancellationToken cancellationToken)
		{
			SchemaDbModel schema = await GetDbProvider(connection).GetSchemaAsync(TimeSpan.FromMinutes(5), cancellationToken);

				// Limpia las tablas de la conexión
				connection.Tables.Clear();
				// Agrega los campos
				foreach (TableDbModel tableSchema in schema.Tables)
				{
					ConnectionTableModel table = new ConnectionTableModel(connection);

						// Asigna las propiedades
						table.Name = tableSchema.Name;
						table.Description = tableSchema.Description;
						table.Schema = tableSchema.Schema;
						// Asigna los campos
						foreach (FieldDbModel fieldSchema in tableSchema.Fields)
						{
							ConnectionTableFieldModel field = new ConnectionTableFieldModel(table);

								// Asigna las propiedades
								field.Name = fieldSchema.Name;
								field.Description = fieldSchema.Description;
								field.Type = fieldSchema.DbType; // fieldSchema.Type.ToString();
								field.Length = fieldSchema.Length;
								field.IsRequired = fieldSchema.IsRequired;
								field.IsKey = fieldSchema.IsKey;
								field.IsIdentity = fieldSchema.IsIdentity;
								// Añade el campo
								table.Fields.Add(field);
						}
						// Añade la tabla a la colección
						connection.Tables.Add(table);
				}
		}

		/// <summary>
		///		Obtiene un proveedor de base de datos
		/// </summary>
		private IDbProvider GetDbProvider(ConnectionModel connection)
		{
			IDbProvider provider = CacheProviders.GetProvider(connection);

				// Si no se ha encontrado el proveedor en el diccionario, se crea uno ...
				if (provider == null)
				{
					// Crea el proveedor
					switch (connection.Type)
					{
						case ConnectionModel.ConnectionType.Spark:
								provider = new SparkProvider(new SparkConnectionString(connection.Parameters.ToDictionary()));
							break;
						case ConnectionModel.ConnectionType.SqlServer:
								provider = new LibDbProviders.SqlServer.SqlServerProvider(new LibDbProviders.SqlServer.SqlServerConnectionString(connection.Parameters.ToDictionary()));
							break;
						case ConnectionModel.ConnectionType.Odbc:
								provider = new LibDbProviders.ODBC.OdbcProvider(new LibDbProviders.ODBC.OdbcConnectionString(connection.Parameters.ToDictionary()));
							break;
						default:
							throw new ArgumentOutOfRangeException($"Cant find provider for '{connection.Name}'");
					}
					// Abre el proveedor
					provider.Open();
					// Lo añade a la caché
					CacheProviders.Add(connection, provider);
				}
				// Devuelve el proveedor
				return provider;
		}

		/// <summary>
		///		Ejecuta una consulta
		/// </summary>
		internal async Task<DataTable> GetDatatableQueryAsync(ConnectionModel connection, string query, 
															  Models.ArgumentListModel arguments, 
															  int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
		{	
			return await Task.Run(() => new ScriptSqlController(this).GetDataTableAsync(GetDbProvider(connection),
																						query, arguments, 
																						actualPage, pageSize, timeout, cancellationToken));
		}

		/// <summary>
		///		Ejecuta una consulta
		/// </summary>
		internal async Task ExecuteQueryAsync(ConnectionModel connection, string query, 
											  Models.ArgumentListModel arguments, 
											  TimeSpan timeout, CancellationToken cancellationToken)
		{	
			await Task.Run(() => new ScriptSqlController(this).ExecuteAsync(GetDbProvider(connection), query, arguments, timeout, cancellationToken));
		}

		/// <summary>
		///		Manager de la solución
		/// </summary>
		internal SolutionManager SolutionManager { get; }

		/// <summary>
		///		Proveedores de datos
		/// </summary>
		private Cache.ProvidersCache CacheProviders = new Cache.ProvidersCache();
	}
}
