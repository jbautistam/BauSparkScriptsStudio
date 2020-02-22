using System;
using System.Collections.Generic;

using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibLogger.Writer.Console
{
	/// <summary>
	///		Clase para mostrar el log en una consola
	/// </summary>
	public class LogConsoleWriter : Interfaces.LogWriterBase
	{
		public LogConsoleWriter(List<LogModel.LogType> typesToLog = null) : base(1, typesToLog) {}

		/// <summary>
		///		Escribe los datos del contexto
		/// </summary>
		public override void Flush(List<LogModel> logItems)
		{
			// ... en este caso no hace nada porque ya los ha escrito uno por uno en WriteStream
		}

		/// <summary>
		///		Escribe un mensaje informativo
		/// </summary>
		protected override void WriteStream(LogModel log)
		{
			Write(GetColor(log.Type), log.ToString(LogModel.WriteMode.Full, log.Level));
		}

		/// <summary>
		///		Escribe un mensaje desde la aplicación principal
		/// </summary>
		public void Write(ConsoleColor color, string message)
		{
			System.Console.ForegroundColor = color;
			System.Console.WriteLine(message);
		}

		/// <summary>
		///		Escribe un mensaje
		/// </summary>
		private ConsoleColor GetColor(LogModel.LogType type)
		{
			switch (type)
			{
				case LogModel.LogType.AssertCorrect:
					return ConsoleColor.Gray;
				case LogModel.LogType.AssertError:
					return ConsoleColor.Red;
				case LogModel.LogType.Console:
					return ConsoleColor.Black;
				case LogModel.LogType.Debug:
					return ConsoleColor.Gray;
				case LogModel.LogType.Error:
					return ConsoleColor.Red;
				case LogModel.LogType.Info:
					return ConsoleColor.White;
				case LogModel.LogType.Progress:
					return ConsoleColor.White;
				case LogModel.LogType.Trace:
					return ConsoleColor.White;
				case LogModel.LogType.Warning:
					return ConsoleColor.DarkMagenta;
				default:
					return ConsoleColor.White;
			}
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
