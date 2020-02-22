using System;

namespace Bau.Libraries.BauSparkScripts.Application.Connections
{
	/// <summary>
	///		Datos de una parte de una cadena SQL
	/// </summary>
	internal class ScriptSqlPartModel
	{
		/// <summary>
		///		Tipo de sección
		/// </summary>
		internal enum PartType
		{
			/// <summary>Comando SQL</summary>
			Sql,
			/// <summary>Comentario</summary>
			Comment
		}

		internal ScriptSqlPartModel(PartType type, string content)
		{
			Type = type;
			Content = content;
		}

		/// <summary>
		///		Tipo
		/// </summary>
		internal PartType Type { get; }

		/// <summary>
		///		Contenido
		/// </summary>
		internal string Content { get; set; }
	}
}
