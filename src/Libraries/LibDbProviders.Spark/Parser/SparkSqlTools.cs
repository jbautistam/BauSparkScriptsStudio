﻿using System;

using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.Spark.Parser
{
	/// <summary>
	///		Herramientas SQL con particularidades asociadas a las consultas de Spark
	/// </summary>
	public class SparkSqlTools
	{
		/// <summary>
		///	Normaliza la SQL que se va a enviar a Spark cambiando los parámetros especificados
		///	Obtiene los parámetros de base de datos buscando en la cadena SQL las cadenas: @xxxx (@ es el prefijo del argumento de consulta)
		/// Añade un parámetro a la colección cada vez que se encuentre un nombre de variable
		/// Se hace así porque OleDb y ODBC no admiten parámetros por nombre si no que sustituye los nombres
		/// de parámetros por posición (utilizando el marcador ?)
		/// </summary>
		public (string sql, ParametersDbCollection parametersDb) NormalizeSql(string sql, ParametersDbCollection inputParametersDb, string parameterPrefix)
		{
			ParametersDbCollection ouputParametersDb = new ParametersDbCollection();
			string sqlOutput = string.Empty;
			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(sql, "\\" + parameterPrefix + "\\w*",
																									System.Text.RegularExpressions.RegexOptions.IgnoreCase,
																									TimeSpan.FromSeconds(1));
			int lastIndex = 0;

				// Mientras haya una coincidencia
				while (match.Success)
				{
					// Añade lo anterior del SQL a la cadena de salida y cambia el índice de último elemento encontrado
					sqlOutput += sql.Substring(lastIndex, match.Index - lastIndex);
					lastIndex = match.Index + match.Length;
					// Añade el marcador de parámetro
					sqlOutput += "? ";
					// Añade el parámetro a la colección de parámetros necesarios
					ouputParametersDb.Add(GetParameterValue(parameterPrefix, sql.Substring(match.Index, match.Length), inputParametersDb, ouputParametersDb.Count));
					// Pasa a la siguiente coincidencia
					match = match.NextMatch();
				}
				// Añade el resto de la cadena inicial
				if (lastIndex < sql.Length)
					sqlOutput += sql.Substring(lastIndex);
				// Devuelve la colección de parámetros para la base de datos
				return (sqlOutput, ouputParametersDb);
		}

		/// <summary>
		///		Convierte un parámetro al índice de ODBC
		/// </summary>
		private ParameterDb GetParameterValue(string parameterPrefix, string key, ParametersDbCollection parametersDb, int index)
		{
			// Busca el parámetro en la colección
			foreach (ParameterDb parameter in parametersDb)
				if (key.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase) ||
						key.Equals(parameterPrefix + parameter.Name, StringComparison.CurrentCultureIgnoreCase))
					return new ParameterDb("@" + parameter.Name + index.ToString(), parameter.Value, parameter.Direction, parameter.Length);
			// Si ha llegado hasta aquí, devuelve un parámetro nulo
			return new ParameterDb("@" + key + index.ToString(), null, System.Data.ParameterDirection.Input);
		}

		/// <summary>
		///		Obtiene una cadena SQL con los valores de los parámetros insertados en la propia cadena
		/// </summary>
		public string ConvertSqlNoParameters(string sql, ParametersDbCollection inputParametersDb, string parameterPrefix)
		{
			string sqlOutput = string.Empty;
			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(sql, "\\" + parameterPrefix + "\\w*",
																									System.Text.RegularExpressions.RegexOptions.IgnoreCase,
																									TimeSpan.FromSeconds(1));
			int lastIndex = 0;

				// Mientras haya una coincidencia
				while (match.Success)
				{
					// Añade lo anterior del SQL a la cadena de salida y cambia el índice de último elemento encontrado
					sqlOutput += sql.Substring(lastIndex, match.Index - lastIndex);
					lastIndex = match.Index + match.Length;
					// Añade el valor del parámetro a la cadena de salida
					sqlOutput += ConvertToSqlValue(GetParameterValue(parameterPrefix, sql.Substring(match.Index, match.Length), inputParametersDb, 0));
					// Pasa a la siguiente coincidencia
					match = match.NextMatch();
				}
				// Añade el resto de la cadena inicial
				if (lastIndex < sql.Length)
					sqlOutput += sql.Substring(lastIndex);
				// Devuelve la colección de parámetros para la base de datos
				return sqlOutput;
		}

		/// <summary>
		///		Convierte un objeto en una cadena para el contenido de una variable de SqlCmd
		/// </summary>
		private string ConvertToSqlValue(ParameterDb parameterDb)
		{
			if (parameterDb.Value == null || parameterDb.Value == DBNull.Value)
				return "NULL";
			else
				switch (parameterDb.Value)
				{
					case int valueInteger:
						return ConvertIntToSql(valueInteger);
					case short valueInteger:
						return ConvertIntToSql(valueInteger);
					case long valueInteger:
						return ConvertIntToSql(valueInteger);
					case double valueDecimal:
						return ConvertDecimalToSql(valueDecimal);
					case float valueDecimal:
						return ConvertDecimalToSql(valueDecimal);
					case decimal valueDecimal:
						return ConvertDecimalToSql((double) valueDecimal);
					case string valueString:
						return ConvertStringToSql(valueString);
					case DateTime valueDate:
						return ConvertDateToSql(valueDate);
					case bool valueBool:
						return ConvertBooleanToSql(valueBool);
					default:
						return ConvertStringToSql(parameterDb.Value.ToString());
				}
		}

		/// <summary>
		///		Convierte un valor lógico a SQL
		/// </summary>
		private string ConvertBooleanToSql(bool value)
		{
			if (value)
				return "1";
			else
				return "0";
		}

		/// <summary>
		///		Convierte una fecha a SQL
		/// </summary>
		private string ConvertDateToSql(DateTime valueDate)
		{
			return $"'{valueDate:yyyy-MM-dd}'";
		}

		/// <summary>
		///		Convierte un valor decimal a Sql
		/// </summary>
		private string ConvertDecimalToSql(double value)
		{
			return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		///		Convierte un entero en una cadena
		/// </summary>
		private string ConvertIntToSql(long value)
		{
			return value.ToString();
		}

		/// <summary>
		///		Convierte una cadena a SQL
		/// </summary>
		private string ConvertStringToSql(string value)
		{
			return "'" + value.Replace("'", "''") + "'";
		}
	}
}
