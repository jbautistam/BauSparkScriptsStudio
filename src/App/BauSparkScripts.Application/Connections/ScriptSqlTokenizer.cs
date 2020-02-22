using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.BauSparkScripts.Application.Connections
{
	/// <summary>
	///		Tokenizador de una cadena SQL
	/// </summary>
	internal class ScriptSqlTokenizer
	{
		/// <summary>
		///		Estados del tokenizador
		/// </summary>
		private enum Mode
		{
			/// <summary>No estamos en ningún contenido aún (acaba de empezar el proceso)</summary>
			Unknown,
			/// <summary>En un comentario</summary>
			AtComment,
			/// <summary>Al final de un comentario de línea</summary>
			AtEndComment,
			/// <summary>En una consulta SQL</summary>
			AtSql,
			/// <summary>En un cierre de consulta SQL</summary>
			AtGo
		}

		/// <summary>
		///		Tipo de línea
		/// </summary>
		private enum LineType
		{
			/// <summary>Desconocido</summary>
			Unknown,
			/// <summary>Inicio de comentario multilínea</summary>
			StartCommentMultiline,
			/// <summary>Fin de un comentario multilínea</summary>
			EndCommentMultiline,
			/// <summary>Inicio de comentario de una línea</summary>
			StartCommentLine,
			/// <summary>Comando GO</summary>
			Go
		}

		/// <summary>
		///		Interpreta una cadena SQL separando los comentarios
		/// </summary>
		internal List<ScriptSqlPartModel> Parse(string sql, NormalizedDictionary<object> parameters)
		{
			List<ScriptSqlPartModel> sqlParts = Tokenize(sql);

				// Reemplaza las variables
				foreach (ScriptSqlPartModel sqlPart in sqlParts)
					if (sqlPart.Type == ScriptSqlPartModel.PartType.Sql)
						sqlPart.Content	= ReplaceVariables(sqlPart.Content, parameters);
				// Devuelve las secciones de la cadena SQL
				return sqlParts;
		}

		/// <summary>
		///		Divide la cadena SQL entre comentarios y comandos
		/// </summary>
		private List<ScriptSqlPartModel> Tokenize(string sql)
		{
			List<ScriptSqlPartModel> sqlParts = new List<ScriptSqlPartModel>();
			Mode mode = Mode.Unknown;
			string content = string.Empty;

				// Si hay algo que interpretar...
				if (!string.IsNullOrEmpty(sql))
				{
					// Separa la cadena por la palabra reservada GO
					foreach (string part in sql.Split('\r', '\n'))
						if (!string.IsNullOrEmpty(part))
						{
							// Actua dependiendo del modo
							switch (mode)
							{
								case Mode.Unknown:
										switch (GetLineType(part))
										{
											case LineType.StartCommentMultiline:
													mode = Mode.AtComment;
												break;
											case LineType.StartCommentLine:
													mode = Mode.AtEndComment;
												break;
											default:
													mode = Mode.AtSql;
												break;
										}
										content += part + Environment.NewLine;
									break;
								case Mode.AtSql:
										switch (GetLineType(part))
										{
											case LineType.Go:
											case LineType.StartCommentLine:
											case LineType.StartCommentMultiline:
													mode = Mode.AtGo;
												break;
											default:
													content += part + Environment.NewLine;
												break;
										}
									break;
								case Mode.AtComment:
										switch (GetLineType(part))
										{
											case LineType.EndCommentMultiline:
													mode = Mode.AtEndComment;
												break;
											default:
													content += part + Environment.NewLine;
												break;
										}
									break;
							}
							// Si estamos en un final de comentario de línea, añadimos el contenido directamente y limpiamos
							// Si estamos en un final de SQL, añadimos el contenido y limpiamos
							if (mode == Mode.AtEndComment)
							{
								sqlParts.Add(new ScriptSqlPartModel(ScriptSqlPartModel.PartType.Comment, content));
								content = string.Empty;
								mode = Mode.Unknown;
							}
							else if (mode == Mode.AtGo)
							{
								sqlParts.Add(new ScriptSqlPartModel(ScriptSqlPartModel.PartType.Sql, content));
								content = string.Empty;
								mode = Mode.Unknown;
							}
						}
					// Añade la última cadena
					if (!string.IsNullOrWhiteSpace(content))
					{
						if (mode == Mode.AtComment)
							sqlParts.Add(new ScriptSqlPartModel(ScriptSqlPartModel.PartType.Comment, content));
						else
							sqlParts.Add(new ScriptSqlPartModel(ScriptSqlPartModel.PartType.Sql, content));
					}
				}
				// Devuelve la lista de comandos de salida
				return sqlParts;
		}

		/// <summary>
		///		Obtiene el tipo de línea
		/// </summary>
		private LineType GetLineType(string content)
		{
			// Quita los espacios
			content = content.TrimIgnoreNull();
			// Comprueba los datos
			if (content.StartsWith("/*"))
			{
				if (content.EndsWith("*/"))
					return LineType.StartCommentLine;
				else
					return LineType.StartCommentMultiline;
			}
			if (content.StartsWith("--"))
				return LineType.StartCommentLine;
			if (content.EndsWith("*/"))
				return LineType.EndCommentMultiline;
			if (content.Equals("GO", StringComparison.CurrentCultureIgnoreCase))
				return LineType.Go;
			// Si ha llegado hasta aquí es porque no es nada especial
			return LineType.Unknown;
		}

		/// <summary>
		///		Elimina los comentarios
		/// </summary>
		private string RemoveComments(string sql)
		{
			string result = string.Empty;

				// Quita los comentarios de línea de la cadena SQL
				foreach (string part in sql.Split('\r', '\n'))
					if (!string.IsNullOrWhiteSpace(part))
					{
						if (!part.TrimIgnoreNull().StartsWith("--"))
							result += part + Environment.NewLine;
					}
				// Devuelve la cadena sin comentarios
				return result;
		}

		/// <summary>
		///		Reemplaza las variables que aparezcan entre {{ }}
		/// </summary>
		private string ReplaceVariables(string sql, NormalizedDictionary<object> parameters)
		{
			// Reemplaza las variables
			foreach ((string key, object value) in parameters.Enumerate())
				sql = sql.ReplaceWithStringComparison("{{" + key + "}}", value.ToString());
			// Reemplaza los valores de escape (\{\{ y \}\}
			sql = sql.ReplaceWithStringComparison("\\{\\{", "{{");
			sql = sql.ReplaceWithStringComparison("\\}\\}", "}}");
			// Devuelve la cadena convertida
			return sql;
		}
	}
}
