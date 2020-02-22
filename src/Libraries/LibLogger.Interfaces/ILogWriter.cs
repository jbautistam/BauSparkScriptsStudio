using System;

namespace Bau.Libraries.LibLogger.Interfaces
{
	/// <summary>
	///		Interface para los manejadores de log
	/// </summary>
	public interface ILogWriter
	{
		/// <summary>
		///		Escribe un elemento de log
		/// </summary>
		void Logged(Models.Log.LogModel logItem);

		/// <summary>
		///		Libera todos los registros de log
		/// </summary>
		void Flush(System.Collections.Generic.List<Models.Log.LogModel> items);
	}
}
