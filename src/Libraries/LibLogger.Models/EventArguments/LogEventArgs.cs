using System;

namespace Bau.Libraries.LibLogger.Models.EventArguments
{
	/// <summary>
	///		Argumentos de los eventos de log
	/// </summary>
	public class LogEventArgs : EventArgs
	{
		public LogEventArgs(Log.LogModel item)
		{
			Item = item;
		}

		/// <summary>
		///		Elemento de log
		/// </summary>
		public Log.LogModel Item { get; }
	}
}
