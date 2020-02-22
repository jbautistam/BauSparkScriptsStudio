using System;
using System.Runtime.CompilerServices;

namespace Bau.Libraries.LibLogger.Models.Log
{
	/// <summary>
	///		Bloque de elementos de log
	/// </summary>
	public class BlockLogModel : IDisposable
	{
		internal BlockLogModel(ContextModel context, BlockLogModel parent = null)
		{
			Context = context;
			Parent = parent;
			if (parent == null)
				Level = 0;
			else
				Level = parent.Level + 1;
		}

		/// <summary>
		///		Crea un bloque
		/// </summary>
		public BlockLogModel CreateBlock(LogModel.LogType type, string message, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.CreateBlock(this, type, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		public LogModel Debug(string message, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Debug(this, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		public LogModel Info(string message, [CallerFilePath] string fileName = null, 
							 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Info(this, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de advertencia
		/// </summary>
		public LogModel Warning(string message, [CallerFilePath] string fileName = null, 
								[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Warning(this, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de error
		/// </summary>
		public LogModel Error(string message, Exception exception = null, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Error(this, message, exception, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de progreso
		/// </summary>
		public LogModel Progress(string message, long actual, long total, [CallerFilePath] string fileName = null, 
								 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Progress(this, message, actual, total, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de traza
		/// </summary>
		public LogModel Trace(string message, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Trace(this, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade una aserción
		/// </summary>
		public LogModel Assert(bool withError, string message, [CallerFilePath] string fileName = null, 
							   [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Assert(this, withError, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de error en suposición
		/// </summary>
		public LogModel AssertError(string message, [CallerFilePath] string fileName = null, 
								    [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.AssertError(this, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de suposición correcta
		/// </summary>
		public LogModel AssertCorrect(string message, [CallerFilePath] string fileName = null, 
									  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.AssertCorrect(this, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade información destinada a consola
		/// </summary>
		public LogModel Console(string message, [CallerFilePath] string fileName = null, 
								[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Console(this, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un elemento de log
		/// </summary>
		public LogModel Log(LogModel.LogType type, string message, [CallerFilePath] string fileName = null, 
							[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Context.LogItems.Log(this, type, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un parámetro al log
		/// </summary>
		public void AddParameter(string name, object value)
		{
			Context.LogItems.AddParameter(name, value);
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		///		Libera la memoria, en este caso no hace nada, es simplemente para implementar la interface IDisposable y
		///	así poder utilizar using
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Libera la memoria
				if (disposing)
					Context.LogItems.DeleteBlock();
				// Indica que se ha liberado
				IsDisposed = true;
			}
		}

		/// <summary>
		///		Contexto del bloque
		/// </summary>
		public ContextModel Context { get; }

		/// <summary>
		///		Elemento padre
		/// </summary>
		public BlockLogModel Parent { get; }

		/// <summary>
		///		Id del bloque
		/// </summary>
		public string Id { get; } = Guid.NewGuid().ToString();

		/// <summary>
		///		Nivel
		/// </summary>
		public int Level { get; }

		/// <summary>
		///		Indica si se ha liberado la memoria
		/// </summary>
		public bool IsDisposed { get; private set; }
	}
}