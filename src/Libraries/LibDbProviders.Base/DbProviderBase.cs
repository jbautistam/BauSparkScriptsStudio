﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.Base
{
	/// <summary>
	///		Clase base para los proveedores de base de datos
	/// </summary>
	public abstract class DbProviderBase : IDbProvider
	{   
		protected DbProviderBase(IConnectionString connectionString)
		{ 
			// Inicializa la conexión
			ConnectionString = connectionString;
			// Inicializa los objetos protegidos
			Connection = null;
			Transaction = null;
		}

		/// <summary>
		///		Abre la conexión a la base de datos
		/// </summary>
		public void Open()
		{
			if (Connection == null || Connection.State != ConnectionState.Open)
			{
				// Crea la conexión
				Connection = GetInstance();
				// Abre la conexión
				Connection.Open();
			}
		}

		/// <summary>
		///		Abre la conexión a la base de datos de forma asíncrona
		/// </summary>
		public async Task OpenAsync(CancellationToken cancellationToken)
		{
			if (Connection == null || Connection.State != ConnectionState.Open)
			{
				// Crea la conexión
				Connection = GetInstance();
				// Abre la conexión
				await (Connection as DbConnection).OpenAsync(cancellationToken);
			}
		}

		/// <summary>
		///		Crea la conexión
		/// </summary>
		protected abstract IDbConnection GetInstance();

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected abstract IDbCommand GetCommand(string sql, TimeSpan? timeOut);

		/// <summary>
		///		Cierra la conexión a la base de datos
		/// </summary>
		public virtual void Close()
		{
			if (Connection?.State == ConnectionState.Open && Transaction == null)
				Connection.Close();
		}

		/// <summary>
		///		Ejecuta una sentencia o un procedimiento sobre la base de datos
		/// </summary>
		public int Execute(string sql, ParametersDbCollection parameters, CommandType commandType, TimeSpan? timeOut = null)
		{
			int rows;

				// Ejecuta la consulta
				using (IDbCommand command = GetCommand(sql, timeOut))
				{ 
					// Indica el tipo del comando
					command.CommandType = commandType;
					command.CommandTimeout = GetTimeout(timeOut);
					// Añade los parámetros al comando
					AddParameters(command, parameters);
					// Ejecuta la consulta
					rows = command.ExecuteNonQuery();
					// Pasa los valores de salida de los parámetros del comando a la colección de parámetros de entrada
					if (parameters != null)
					{
						parameters.Clear();
						parameters.AddRange(ReadOutputParameters(command.Parameters));
					}
				}
				// Devuelve el número de registros afectados
				return rows;
		}

		/// <summary>
		///		Ejecuta una sentencia o un procedimiento sobre la base de datos de forma asíncrona
		/// </summary>
		public async Task<int> ExecuteAsync(string sql, ParametersDbCollection parameters, CommandType commandType, TimeSpan? timeOut = null,
											CancellationToken? cancellationToken = null)
		{
			int rows;

				// Ejecuta la consulta
				using (IDbCommand command = GetCommand(sql, timeOut))
				{ 
					// Indica el tipo del comando
					command.CommandType = commandType;
					command.CommandTimeout = GetTimeout(timeOut);
					// Añade los parámetros al comando
					AddParameters(command, parameters);
					// Ejecuta la consulta
					rows = await (command as DbCommand).ExecuteNonQueryAsync(cancellationToken ?? CancellationToken.None);
					// Pasa los valores de salida de los parámetros del comando a la colección de parámetros de entrada
					if (parameters != null)
					{
						parameters.Clear();
						parameters.AddRange(ReadOutputParameters(command.Parameters));
					}
				}
				// Devuelve el número de registros afectados
				return rows;
		}

		/// <summary>
		///		Obtiene un DataReader
		/// </summary>
		public IDataReader ExecuteReader(string sql, ParametersDbCollection parametersDB, CommandType commandType, TimeSpan? timeOut = null)
		{
			IDataReader reader;

				// Ejecuta el comando
				using (IDbCommand command = GetCommand(sql, timeOut))
				{ 
					// Indica el tipo de comando
					command.CommandType = commandType;
					command.CommandTimeout = GetTimeout(timeOut);
					// Añade los parámetros
					AddParameters(command, parametersDB);
					// Obtiene el dataReader
					reader = command.ExecuteReader();
				}
				// Devuelve el dataReader
				return reader;
		}

		/// <summary>
		///		Obtiene un DataReader de forma asíncrona
		/// </summary>
		public async Task<DbDataReader> ExecuteReaderAsync(string sql, ParametersDbCollection parametersDB, CommandType commandType, 
														  TimeSpan? timeOut = null, CancellationToken? cancellationToken = null)
		{
			DbDataReader reader;

				// Ejecuta el comando
				using (IDbCommand command = GetCommand(sql, timeOut))
				{ 
					// Indica el tipo de comando
					command.CommandType = commandType;
					command.CommandTimeout = GetTimeout(timeOut);
					// Añade los parámetros
					AddParameters(command, parametersDB);
					// Obtiene el dataReader
					reader = await (command as DbCommand).ExecuteReaderAsync(cancellationToken ?? CancellationToken.None);
				}
				// Devuelve el dataReader
				return reader;
		}

		/// <summary>
		///		Ejecuta una sentencia o procedimiento sobre la base de datos y devuelve un escalar
		/// </summary>
		public object ExecuteScalar(string sql, ParametersDbCollection parameters, CommandType commandType, TimeSpan? timeOut = null)
		{
			object result;

				// Ejecuta el comando
				using (IDbCommand command = GetCommand(sql, timeOut))
				{ 
					// Indica el tipo de comando
					command.CommandType = commandType;
					command.CommandTimeout = GetTimeout(timeOut);
					// Añade los parámetros al comando
					AddParameters(command, parameters);
					// Ejecuta la consulta
					result = command.ExecuteScalar();
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Ejecuta una sentencia o procedimiento sobre la base de datos y devuelve un escalar de forma asíncrona
		/// </summary>
		public async Task<object> ExecuteScalarAsync(string sql, ParametersDbCollection parameters, CommandType commandType, 
													 TimeSpan? timeOut = null, CancellationToken? cancellationToken = null)
		{
			object result;

				// Ejecuta el comando
				using (IDbCommand command = GetCommand(sql, timeOut))
				{ 
					// Indica el tipo de comando
					command.CommandType = commandType;
					command.CommandTimeout = GetTimeout(timeOut);
					// Añade los parámetros al comando
					AddParameters(command, parameters);
					// Ejecuta la consulta
					result = await (command as DbCommand).ExecuteScalarAsync(cancellationToken ?? CancellationToken.None);
				}
				// Devuelve el resultado
				return result;

		}

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros
		/// </summary>
		public DataTable GetDataTable(string sql, ParametersDbCollection parameters, CommandType commandType, TimeSpan? timeOut = null)
		{
			DataTable table = new DataTable();

				// Carga los datos de la tabla
				table.Load(ExecuteReader(sql, parameters, commandType, timeOut), LoadOption.OverwriteChanges);
				// Devuelve la tabla
				return table;
		}

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros de forma asíncrona
		/// </summary>
		public async Task<DataTable> GetDataTableAsync(string sql, ParametersDbCollection parameters, CommandType commandType, 
													   TimeSpan? timeOut = null, CancellationToken? cancellationToken = null)
		{
			DataTable table = new DataTable();

				// Carga los datos de la tabla
				table.Load(await ExecuteReaderAsync(sql, parameters, commandType, timeOut, cancellationToken), LoadOption.OverwriteChanges);
				// Devuelve la tabla
				return table;
		}

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros
		/// </summary>
		public DataTable GetDataTable(string sql, ParametersDbCollection parameters, CommandType commandType, int pageNumber, int pageSize, TimeSpan? timeOut = null)
		{
			if (SqlParser == null || commandType != CommandType.Text) // ... si no hay un intérprete de paginación en servidor, se obtiene el DataTable directamente de la SQL
				return GetDataTable(sql, parameters, commandType, timeOut);
			else
				return GetDataTable(SqlParser.GetSqlPagination(sql, pageNumber, pageSize), parameters, commandType, timeOut);
		}

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros
		/// </summary>
		public async Task<DataTable> GetDataTableAsync(string sql, ParametersDbCollection parameters, CommandType commandType, int pageNumber, int pageSize, 
													   TimeSpan? timeOut = null, CancellationToken? cancellationToken = null)
		{
			if (SqlParser == null || commandType != CommandType.Text) // ... si no hay un intérprete de paginación en servidor, se obtiene el DataTable directamente de la SQL
				return await GetDataTableAsync(sql, parameters, commandType, timeOut, cancellationToken);
			else
				return await GetDataTableAsync(SqlParser.GetSqlPagination(sql, pageNumber, pageSize), parameters, commandType, timeOut, cancellationToken);
		}

		/// <summary>
		///		Obtiene un IDataReader a partir de un nombre de una sentencia o procedimiento y sus parámetros paginando
		///	en el servidor
		/// </summary>
		/// <remarks>
		///		Sólo está implementado totalmente para los comandos de texto, no para los procedimientos almacenados
		/// </remarks>
		public IDataReader ExecuteReader(string sql, ParametersDbCollection parameters, CommandType commandType, int pageNumber, int pageSize, TimeSpan? timeOut = null)
		{
			if (commandType == CommandType.Text && SqlParser != null)
			{ 
				// Crea una colección de parámetros si no existía
				if (parameters == null)
					parameters = new ParametersDbCollection();
				// Obtiene el dataReader
				return ExecuteReader(SqlParser.GetSqlPagination(sql, pageNumber, pageSize), parameters, commandType, timeOut);
			}
			else
				return ExecuteReader(sql, parameters, commandType, timeOut);
		}

		/// <summary>
		///		Obtiene un IDataReader a partir de un nombre de una sentencia o procedimiento y sus parámetros paginando
		///	en el servidor de forma asíncrona
		/// </summary>
		/// <remarks>
		///		Sólo está implementado totalmente para los comandos de texto, no para los procedimientos almacenados
		/// </remarks>
		public async Task<DbDataReader> ExecuteReaderAsync(string sql, ParametersDbCollection parameters, CommandType commandType, int pageNumber, int pageSize, 
														   TimeSpan? timeOut = null, CancellationToken? cancellationToken = null)
		{
			if (commandType == CommandType.Text && SqlParser != null)
			{ 
				// Crea una colección de parámetros si no existía
				if (parameters == null)
					parameters = new ParametersDbCollection();
				// Obtiene el dataReader
				return await ExecuteReaderAsync(SqlParser.GetSqlPagination(sql, pageNumber, pageSize), parameters, commandType, timeOut, cancellationToken);
			}
			else
				return await ExecuteReaderAsync(sql, parameters, commandType, timeOut, cancellationToken);
		}

		/// <summary>
		///		Obtiene el número de registro de una consulta
		/// </summary>
		public long? GetRecordsCount(string sql, ParametersDbCollection parametersDB, TimeSpan? timeOut = null)
		{
			if (SqlParser == null)
				return null;
			else
			{
				object result = ExecuteScalar(SqlParser.GetSqlCount(sql), parametersDB, CommandType.Text, timeOut);

					if (result == null)
						return null;
					else if (result is long)
						return (long?) result;
					else
						return (int?) result;
			}
		}

		/// <summary>
		///		Obtiene el número de registro de una consulta
		/// </summary>
		public async Task<long?> GetRecordsCountAsync(string sql, ParametersDbCollection parametersDB, 
													  TimeSpan? timeOut = null, CancellationToken? cancellationToken = null)
		{
			if (SqlParser == null)
				return null;
			else
			{
				object result = await ExecuteScalarAsync(SqlParser.GetSqlCount(sql), parametersDB, CommandType.Text, timeOut, cancellationToken);

					if (result == null)
						return null;
					else if (result is long)
						return (long?) result;
					else
						return (int?) result;
			}
		}

		/// <summary>
		///		Copia masiva de datos en una tabla
		/// </summary>
		public virtual long BulkCopy(IDataReader reader, string table, System.Collections.Generic.Dictionary<string, string> mappings, 
									 int recordsPerBlock = 30_000, TimeSpan? timeout = null)
		{
			return new SqlTools.SqlBulkCopy().Process(this, reader, table, mappings, recordsPerBlock, timeout);
		}

		/// <summary>
		///		Copia masiva de datos en una tabla de forma asíncrona
		/// </summary>
		public async Task<long> BulkCopyAsync(IDataReader reader, string table, Dictionary<string, string> mappings, int recordsPerBlock = 30000, 
											  TimeSpan? timeout = null, CancellationToken? cancellationToken = null)
		{
			return await new SqlTools.SqlBulkCopy().ProcessAsync(this, reader, table, mappings, recordsPerBlock, 
																 timeout ?? TimeSpan.FromMinutes(60), cancellationToken ?? CancellationToken.None);
		}

		/// <summary>
		///		Añade a un comando los parámetros de una clase <see cref="ParametersDbCollection"/>
		/// </summary>
		protected void AddParameters(IDbCommand command, ParametersDbCollection parameters)
		{ 
			// Limpia los parámetros antiguos
			command.Parameters.Clear();
			// Añade los parámetros nuevos
			if (parameters != null)
				foreach (ParameterDb parameter in parameters)
					command.Parameters.Add(GetSQLParameter(parameter));
		}

		/// <summary>
		///		Obtiene un parámetro a partir de un parámetro genérico
		/// </summary>
		private IDataParameter GetSQLParameter(ParameterDb parameter)
		{
			IDataParameter parameterDB = ConvertParameter(parameter);

				// Asigna el valor
				if ((parameterDB.DbType == DbType.AnsiString || parameterDB.DbType == DbType.AnsiStringFixedLength ||
						 parameterDB.DbType == DbType.String || parameterDB.DbType == DbType.StringFixedLength) &&
						 string.IsNullOrEmpty(parameter.Value as string))
					parameterDB.Value = DBNull.Value;
				else
					parameterDB.Value = parameter.GetDBValue();
				// Asigna la dirección
				parameterDB.Direction = parameter.Direction;
				// Devuelve el parámetro
				return parameterDB;
		}

		/// <summary>
		///		Método abstracto para convertir un parámetro
		/// </summary>
		protected abstract IDataParameter ConvertParameter(ParameterDb parameter);

		/// <summary>
		///		Lee los parámetros de salida
		/// </summary>
		private ParametersDbCollection ReadOutputParameters(IDataParameterCollection outputParameters)
		{
			ParametersDbCollection parameters = new ParametersDbCollection();

				// Recupera los parámetros
				foreach (IDataParameter outputParameter in outputParameters)
				{
					ParameterDb parameter = new ParameterDb();

						// Asigna los datos
						parameter.Name = outputParameter.ParameterName;
						parameter.Direction = outputParameter.Direction;
						if (outputParameter.Value == DBNull.Value)
							parameter.Value = null;
						else
							parameter.Value = outputParameter.Value;
						// Añade el parámetro a la colección
						parameters.Add(parameter);
				}
				// Devuelve la colección de parámetros
				return parameters;
		}

		/// <summary>
		///		Inicia una transacción
		/// </summary>
		public void BeginTransaction()
		{ 
			// Abre la conexión si no estaba abierta
			Open();
			// Inicia la transacción
			Transaction = Connection.BeginTransaction();
		}

		/// <summary>
		///		Confirma una transacción
		/// </summary>
		public void Commit()
		{
			Transaction?.Commit();
			Transaction = null;
		}

		/// <summary>
		///		Deshace una transacción
		/// </summary>
		public void RollBack()
		{
			Transaction?.Rollback();
			Transaction = null;
		}

		/// <summary>
		///		Obtiene el timeout de la conexión
		/// </summary>
		private int GetTimeout(TimeSpan? timeOut)
		{
			return (int) (timeOut ?? TimeSpan.FromMinutes(1)).TotalSeconds;
		}

		/// <summary>
		///		Obtiene el esquema de base de datos de forma asíncrona
		/// </summary>
		public abstract Task<Schema.SchemaDbModel> GetSchemaAsync(TimeSpan timeout, CancellationToken cancellationToken);

		/// <summary>
		///		Desconecta la conexión
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		Desconecta la conexión
		/// </summary>
		private void Dispose(bool disposing)
		{
			if (disposing && Connection != null)
			{
				Close();
				Connection.Dispose();
			}
		}

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public IConnectionString ConnectionString { get; set; }

		/// <summary>
		///		Parser para consultas SQL
		/// </summary>
		protected SqlTools.SqlSelectParserBase SqlParser { get; set; }

		/// <summary>
		///		Conexión
		/// </summary>
		protected IDbConnection Connection { get; set; }

		/// <summary>
		///		Transacción
		/// </summary>
		public IDbTransaction Transaction { get; protected set; }
	}
}