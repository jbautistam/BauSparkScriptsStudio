using System;
using System.Collections.Generic;

using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibLogger.Writer.Files
{
	/// <summary>
	///		Escritor de log sobre un archivo
	/// </summary>
	public class LogFileWriter : Interfaces.LogWriterBase
	{
		public LogFileWriter(string path, string appName, int recordsPerBlock = 5_000, List<LogModel.LogType> typesToLog = null) : base(recordsPerBlock, typesToLog)
		{
			PathBase = path;
			AppName = appName;
		}

		/// <summary>
		///		Escribe los datos del contexto
		/// </summary>
		public override void Flush(List<LogModel> logItems)
		{
			// Crea el directorio si no existe
			MakePath(PathBase);
			// Graba el archivo
			if (logItems.Count > 0 && System.IO.Directory.Exists(PathBase))
				using (System.IO.StreamWriter stmLog = new System.IO.StreamWriter(GetFileName(), true, System.Text.Encoding.UTF8))
				{
					// Escribe el log
					foreach (LogModel log in logItems)
						if (MustWrite(log.Type))
							stmLog.WriteLine(log.ToString(LogModel.WriteMode.Full, log.Level));
					// Cierra el archivo
					stmLog.Close();
				}
		}

		/// <summary>
		///		Escribe el elemento de log que se acaba de lanzar. Para este caso de escritura en archivo, no hace nada
		/// </summary>
		protected override void WriteStream(LogModel log)
		{
			// ... en este caso no hace nada, simplemente implementa la interface
		}

		/// <summary>
		///		Crea el directorio donde se va a almacenar el log
		/// </summary>
		private void MakePath(string path)
		{
			if (!System.IO.Directory.Exists(path))
				try
				{
					System.IO.Directory.CreateDirectory(path);
				}
				catch (Exception exception)
				{
					System.Diagnostics.Trace.WriteLine($"Can't create the path '{path}'. {exception.Message}");
				}
		}

		/// <summary>
		///		Obtiene el nombre de archivo de log
		/// </summary>
		private string GetFileName()
		{
			// Normaliza un nombre de archivo
			string Normalize(string file)
			{
				string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,;_ ";
				string result = string.Empty;

					// Añade los caracteres
					foreach (char chr in file)
						if (allowedChars.IndexOf(chr) >= 0)
							result += chr;
						else
							result += '_';
					// Devuelve la cadena normalizada
					return result;
			}

			return System.IO.Path.Combine(PathBase, Normalize($"log_{AppName}_{DateTime.Now.Date:yyyy-MM-dd}.log"));
		}

		/// <summary>
		///		Directorio base del log
		/// </summary>
		public string PathBase { get; }

		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		public string AppName { get; }
	}
}
