using System;
using System.Collections.Generic;

using Bau.Libraries.LibLogger.Models;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibLogger.Core
{
	/// <summary>
	///		Manager de log
	/// </summary>
	public class LogManager : IDisposable
	{
		// Eventos
		public event EventHandler<Models.EventArguments.LogEventArgs> Logged;

		public LogManager(int maxItems = 30_000)
		{
			Default = CreateContext("Default", GetType().ToString());
			MaxItems = maxItems;
		}

		/// <summary>
		///		Crea un contexto
		/// </summary>
		public ContextModel CreateContext(string key, string app)
		{
			ContextModel context = new ContextModel(key, app);

				// Asigna el manejador de eventos
				context.Logged += (sender, args) => RaiseLog(args.Item);
				// Añade el contexto a la colección
				return Contexts.Add(key, context);
		}

		/// <summary>
		///		Obtiene un contexto
		/// </summary>
		public ContextModel GetContext(string key)
		{
			return Contexts.Get(key);
		}

		/// <summary>
		///		Añade un objeto de escritura del log
		/// </summary>
		public void AddWriter(Interfaces.ILogWriter writer)
		{
			Writers.Add(writer);
		}

		/// <summary>
		///		Lanza el evento de log
		/// </summary>
		private void RaiseLog(LogModel log)
		{
			// Lanza el evento de log
			Logged?.Invoke(this, new Models.EventArguments.LogEventArgs(log));
			// Escribe el log en los logger (para los que lo tienen que mostrar instantáneamente
			foreach (Interfaces.ILogWriter writer in Writers)
				writer.Logged(log);
			// Añade el item a la colección de elementos pendientes
			lock (Items)
			{
				Items.Add(log);
			}
			// Envía un flush automático si es necesario
			if (Items.Count >= MaxItems)
				Flush();
		}

		/// <summary>
		///		Envía a los objetos de escritura los elementos de log
		/// </summary>
		public void Flush()
		{
			// Lanza los últimos elementos de log
			foreach ((string _, ContextModel context) in Contexts.Enumerate())
				if (context.LogItems.LastLog != null)
					context.LogItems.Reset(); //? ... esto va a lanzar un RaiseLog, que en el caso que tengamos un número máximo de elementos lanzaría de nuevo un flush, por eso el if
			// Envía los elementos
			lock (Items)
			{
				if (Items.Count > 0)
				{
					// Envía los contextos al generador
					foreach (Interfaces.ILogWriter writer in Writers)
						foreach ((string _, ContextModel context) in Contexts.Enumerate())
							writer.Flush(Items);
					// Elimina los datos actuales de los contextos
					foreach ((string _, ContextModel context) in Contexts.Enumerate())
						context.Reset();
					// Limpia los elementos pendientes por escribir
					Items.Clear();
				}
			}
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Libera la memoria
				if (disposing)
					Flush();
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
		///		Objetos de persistencia de los elementos de log
		/// </summary>
		private List<Interfaces.ILogWriter> Writers { get; } = new List<Interfaces.ILogWriter>();

		/// <summary>
		///		Contextos
		/// </summary>
		private ContextModelCollection Contexts { get; } = new ContextModelCollection();

		/// <summary>
		///		Contexto predeterminado
		/// </summary>
		public ContextModel Default { get; }

		/// <summary>
		///		Número máximo de elementos que se mantienen en memoria antes de hacer un flush automático
		/// </summary>
		public int MaxItems { get; }

		/// <summary>
		///		Indica si se ha liberado el objeto
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		///		Elementos de log que quedan por escribir
		/// </summary>
		private List<LogModel> Items { get; } = new List<LogModel>();
	}
}
