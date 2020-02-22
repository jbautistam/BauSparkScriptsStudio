using System;
using System.Collections.Generic;

using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibLogger.Interfaces
{
	/// <summary>
	///		Base para las clases de escritura de log
	/// </summary>
	public abstract class LogWriterBase : ILogWriter
	{
		public LogWriterBase(int recordsPerBlock = 5_000, List<LogModel.LogType> typesToLog = null)
		{
			RecordsPerBlock = recordsPerBlock;
			if (typesToLog != null)
				TypesToLog.AddRange(typesToLog);
		}

		/// <summary>
		///		Escribe una serie de elementos de log
		/// </summary>
		public abstract void Flush(List<LogModel> items);

		/// <summary>
		///		Trata el evento de log (algunos Writers, como la consola) escriben todos los logs según ocurren
		/// </summary>
		public void Logged(LogModel logItem)
		{
			if (MustWrite(logItem.Type))
				WriteStream(logItem);
		}

		/// <summary>
		///		Escribe el elemento de log según ocurre (sólo para algunos casos)
		/// </summary>
		protected abstract void WriteStream(LogModel log);

		/// <summary>
		///		Comprueba si se debe escribir un tipo de log
		/// </summary>
		protected bool MustWrite(LogModel.LogType type)
		{
			// Comprueba si se debe escribir
			if (TypesToLog.Count == 0)
				return true;
			else
				foreach (LogModel.LogType typeToLog in TypesToLog)
					if (typeToLog == type)
						return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Registros a escribir por bloque
		/// </summary>
		public int RecordsPerBlock { get; }

		/// <summary>
		///		Tipos de log a escribir
		/// </summary>
		public List<LogModel.LogType> TypesToLog { get; } = new List<LogModel.LogType>();
	}
}
