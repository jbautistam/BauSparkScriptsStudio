using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibLogger.Models.Metrics;

namespace Bau.Libraries.LibLogger.Models
{
	/// <summary>
	///		Modelo con el contexto de log
	/// </summary>
	public class ContextModel : IDisposable
	{
		// Eventos públicos
		public event EventHandler<EventArguments.LogEventArgs> Logged;
		internal event EventHandler Disposed;

		public ContextModel(string key, string app)
		{
			Key = key;
			App = app;
			LogItems = new LogModelCollection(this);
			Metrics = new MetricGroupModelCollection(this);
		}

		/// <summary>
		///		Crea un bloque de log
		/// </summary>
		public BlockLogModel CreateBlock(LogModel.LogType type, string message, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return LogItems.CreateBlock(type, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Inicializa el contexto
		/// </summary>
		public void Reset()
		{
			LogItems.Clear();
			Metrics.Reset();
		}

		/// <summary>
		///		Lanza un evento de log
		/// </summary>
		internal void RaiseLog(LogModel item)
		{
			Logged?.Invoke(this, new EventArguments.LogEventArgs(item));
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Lanza el evento para indicar al manager que puede destruir este objeto
				if (disposing)
					Disposed?.Invoke(this, EventArgs.Empty);
				// Indica que se ha liberado
				IsDisposed = true;
			}
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		///		Clave del elemento
		/// </summary>
		public string Key { get; }

		/// <summary>
		///		Nombre de aplicación a la que se asocia el contexto
		/// </summary>
		public string App { get; }

		/// <summary>
		///		Parámetros globales del contexto
		/// </summary>
		public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

		/// <summary>
		///		Elementos de log
		/// </summary>
		public LogModelCollection LogItems { get; }

		/// <summary>
		///		Grupos de métricas
		/// </summary>
		public MetricGroupModelCollection Metrics { get; }

		/// <summary>
		///		Indica si se ha liberado la memoria del contexto
		/// </summary>
		public bool IsDisposed { get; private set; }
	}
}