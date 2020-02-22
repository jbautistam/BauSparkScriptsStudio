using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Files;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.BauSparkScripts.Application.Controllers.Databricks
{
	/// <summary>
	///		Exportador de archivos a notebooks de databricks
	/// </summary>
	internal class DatabrickExporter
	{
		internal DatabrickExporter(SolutionManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Exporta los archivos
		/// </summary>
		internal void Export(string sourcePath, string targetPath, NormalizedDictionary<object> parameters)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Debug, "Comienzo de la copia de directorios"))
			{
				// Copia los directorios
				CopyPath(block, sourcePath, targetPath, parameters);
				// Log
				block.Debug("Fin de la copia de directorios");
			}
		}

		/// <summary>
		///		Copia los archivos de un directorio
		/// </summary>
		private void CopyPath(BlockLogModel block, string sourcePath, string targetPath, NormalizedDictionary<object> parameters)
		{
			// Log
			block.Debug($"Copiando '{sourcePath}' a '{targetPath}'");
			// Crea el directorio
			HelperFiles.KillPath(targetPath);
			HelperFiles.MakePath(targetPath);
			// Copia los archivos
			CopyFiles(block, sourcePath, targetPath, parameters);
			// Copia recursivamente los directorios
			foreach (string path in System.IO.Directory.EnumerateDirectories(sourcePath))
				CopyPath(block, path, System.IO.Path.Combine(targetPath, System.IO.Path.GetFileName(path)), parameters);
		}

		/// <summary>
		///		Copia los archivos
		/// </summary>
		private void CopyFiles(BlockLogModel block, string sourcePath, string targetPath, NormalizedDictionary<object> parameters)
		{
			foreach (string file in System.IO.Directory.EnumerateFiles(sourcePath))
			{
				string targetFile = System.IO.Path.Combine(targetPath, System.IO.Path.GetFileName(file));

					// Copia los archivos de Python sin cambios, los de SQL los convierte
					if (file.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase) || file.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
					{
						block.Debug($"Copiando '{file}' a '{targetFile}'");
						HelperFiles.CopyFile(file, targetFile);
					}
					else if (file.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
					{
						string content = TransformSql(file, parameters);

							// Graba el contenido
							HelperFiles.SaveTextFile(targetFile, content, System.Text.Encoding.UTF8);
					}
			}
		}

		/// <summary>
		///		Transforma la SQL para un notebook
		/// </summary>
		private string TransformSql(string sourceFile, NormalizedDictionary<object> parameters)
		{
			List<Connections.ScriptSqlPartModel> scriptSqlParts = new Connections.ScriptSqlTokenizer().Parse(HelperFiles.LoadTextFile(sourceFile), parameters);
			System.Text.StringBuilder sbResult = new System.Text.StringBuilder();

				// Cabecera
				sbResult.AppendLine("-- Databricks notebook source");
				// Añade los scripts al resultado
				foreach (Connections.ScriptSqlPartModel scriptSqlPart in scriptSqlParts)
				{
					// Añade el contenido de la sección
					switch (scriptSqlPart.Type)
					{
						case Connections.ScriptSqlPartModel.PartType.Sql:
								sbResult.AppendLine(scriptSqlPart.Content);
							break;
						case Connections.ScriptSqlPartModel.PartType.Comment:
								if (!string.IsNullOrWhiteSpace(scriptSqlPart.Content))
								{
									string [] parts = scriptSqlPart.Content.Split('\r', '\n');
									bool first = true;

										foreach (string part in parts)
										{
											// Añade la cadena mágica de markdown
											sbResult.Append("-- MAGIC ");
											if (first)
												sbResult.Append("%md ");
											// Añade la línea de contenido
											sbResult.AppendLine(RemoveComment(part));
											// Indica que no es la primera vez
											first = false;
										}
								}
							break;
					}
					// Añade el separador de comandos
					sbResult.AppendLine();
					sbResult.AppendLine("-- COMMAND ----------");
					sbResult.AppendLine();
				}
				// Devuelve la cadena
				return sbResult.ToString();
		}

		/// <summary>
		///		Elimina los caracteres de comentario (/* */ --)
		/// </summary>
		private string RemoveComment(string part)
		{
			// Le quita los comentarios
			if (!string.IsNullOrWhiteSpace(part))
			{
				// Quita los espacios
				part = part.Trim();
				// Quita los caracteres iniciales
				if (part.StartsWith("--") || part.StartsWith("/*"))
				{
					if (part.Length > 2)
						part = part.Substring(2);
					else
						part = string.Empty;
				}
				// Quita los caracteres finales
				if (!string.IsNullOrWhiteSpace(part) && part.EndsWith("*/"))
				{
					if (part.Length > 2)
						part = part.Substring(0, part.Length - 2);
					else
						part = string.Empty;
				}
			}
			// Devuelve la cadena
			return part;
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		internal SolutionManager Manager { get; }
	}
}
