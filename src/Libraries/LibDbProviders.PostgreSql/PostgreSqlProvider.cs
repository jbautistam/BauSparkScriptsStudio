﻿using System;
using System.Data;

using NpgsqlTypes;
using Npgsql;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.PostgreSql
{
	/// <summary>
	///		Proveedor para PostgreSql
	/// </summary>
	public class PostgreSqlProvider : DbProviderBase
	{
		public PostgreSqlProvider(IConnectionString connectionString) : base(connectionString) 
		{ 
			SqlParser = new Parser.PostgreSqlSelectParser();
		}

		/// <summary>
		///		Crea la conexión
		/// </summary>
		protected override IDbConnection GetInstance()
		{
			return new NpgsqlConnection(ConnectionString.ConnectionString);
		}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text, TimeSpan? timeout = null)
		{
			return new NpgsqlCommand(text, Connection as NpgsqlConnection, Transaction as NpgsqlTransaction);
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDb parameter)
		{
			// Convierte el parámetro
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Integer);
			if (parameter.Value == null)
				return new NpgsqlParameter(parameter.Name, null);
			if (parameter.IsText)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Varchar);
			if (parameter.Value is bool?)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Boolean);
			if (parameter.Value is int?)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Integer);
			if (parameter.Value is double?)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Double);
			if (parameter.Value is string)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Varchar, parameter.Length);
			if (parameter.Value is byte[])
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Bit);
			if (parameter.Value is DateTime?)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Date);
			if (parameter.Value is Enum)
				return new NpgsqlParameter(parameter.Name, NpgsqlDbType.Integer);
			// Si ha llegado hasta aquí, lanza una excepción
			throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}

		/// <summary>
		///		Obtiene el esquema
		/// </summary>
		public async override System.Threading.Tasks.Task<Base.Schema.SchemaDbModel> GetSchemaAsync(TimeSpan timeout, System.Threading.CancellationToken cancellationToken)
		{
			return await new Parser.PostgreSqlSchemaReader().GetSchemaAsync(this, timeout, cancellationToken);
		}
	}
}