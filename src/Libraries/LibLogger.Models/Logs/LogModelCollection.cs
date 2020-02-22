using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bau.Libraries.LibLogger.Models.Log
{
	/// <summary>
	///		Colección de <see cref="LogModel"/>
	/// </summary>
	public class LogModelCollection : List<LogModel>
	{
		// Variables privadas
		private LogModel _lastLog = null;
		private Stack<BlockLogModel> _blocks = new Stack<BlockLogModel>();

		public LogModelCollection(ContextModel context)
		{
			Context = context;
		}

		/// <summary>
		///		Añade un bloque
		/// </summary>
		public BlockLogModel CreateBlock(LogModel.LogType type, string message, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return CreateBlock(null, type, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un bloque
		/// </summary>
		public BlockLogModel CreateBlock(BlockLogModel parent, LogModel.LogType type, string message, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			BlockLogModel block = new BlockLogModel(Context, GetLastBlock(parent));

				// Añade el bloque a la cola
				Enqueue(block);
				// Añade el log
				Log(block, type, message, fileName, methodName, lineNumber);
				// Devuelve el bloque generado
				return block;
		}

		/// <summary>
		///		Encola un bloque
		/// </summary>
		internal void Enqueue(BlockLogModel block)
		{
			_blocks.Push(block);
		}

		/// <summary>
		///		Elimina un bloque
		/// </summary>
		internal void DeleteBlock()
		{
			if (_blocks.Count > 0)
				_blocks.Pop();
		}

		/// <summary>
		///		Obtiene el bloque sobre el que se debe añadir el log
		/// </summary>
		private BlockLogModel GetLastBlock(BlockLogModel parent)
		{
			// Obtiene el último bloque de la cola si no se le ha pasado ninguno
			if (parent == null && _blocks.Count > 0)
				parent = _blocks.Peek();
			// Devuelve el último bloque
			return parent;
		}

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		public LogModel Debug(string message, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Debug(null, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		public LogModel Debug(BlockLogModel block, string message, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(block, LogModel.LogType.Debug, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		public LogModel Info(string message, [CallerFilePath] string fileName = null, 
							 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Info(null, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		public LogModel Info(BlockLogModel block, string message, [CallerFilePath] string fileName = null, 
							 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(block, LogModel.LogType.Info, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de advertencia
		/// </summary>
		public LogModel Warning(string message, [CallerFilePath] string fileName = null, 
								[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Warning(null, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de advertencia
		/// </summary>
		public LogModel Warning(BlockLogModel block, string message, [CallerFilePath] string fileName = null, 
								[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(block, LogModel.LogType.Warning, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de error
		/// </summary>
		public LogModel Error(string message, Exception exception = null, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Error(null, message, exception, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de error
		/// </summary>
		public LogModel Error(BlockLogModel block, string message, Exception exception = null, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			LogModel item = Log(block, LogModel.LogType.Error, message, fileName, methodName, lineNumber);

				// Asigna la excepción
				item.Exception = exception;
				// Devuelve el log creado
				return item;
		}

		/// <summary>
		///		Añade un mensaje de progreso
		/// </summary>
		public LogModel Progress(string message, long actual, long total, [CallerFilePath] string fileName = null, 
								 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Progress(null, message, actual, total, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de progreso
		/// </summary>
		public LogModel Progress(BlockLogModel block, string message, long actual, long total, [CallerFilePath] string fileName = null, 
								 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			LogModel item = Log(block, LogModel.LogType.Progress, message, fileName, methodName, lineNumber);

				// Asigna los datos de progreso
				item.ActualProgress = actual;
				item.TotalProgress = total;
				// Devuelve el elemento de log creado
				return item;
		}

		/// <summary>
		///		Añade un mensaje de traza
		/// </summary>
		public LogModel Trace(string message, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Trace(null, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de traza
		/// </summary>
		public LogModel Trace(BlockLogModel block, string message, [CallerFilePath] string fileName = null, 
							  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(block, LogModel.LogType.Trace, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade una aserción
		/// </summary>
		public LogModel Assert(bool withError, string message, [CallerFilePath] string fileName = null, 
							   [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Assert(null, withError, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade una aserción
		/// </summary>
		public LogModel Assert(BlockLogModel block, bool withError, string message, [CallerFilePath] string fileName = null, 
							   [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			if (withError)
				return AssertError(block, message, fileName, methodName, lineNumber);
			else
				return AssertCorrect(block, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de error en suposición
		/// </summary>
		public LogModel AssertError(string message, [CallerFilePath] string fileName = null, 
								    [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return AssertError(null, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de error en suposición
		/// </summary>
		public LogModel AssertError(BlockLogModel block, string message, [CallerFilePath] string fileName = null, 
								    [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(block, LogModel.LogType.AssertError, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de suposición correcta
		/// </summary>
		public LogModel AssertCorrect(string message, [CallerFilePath] string fileName = null, 
									  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return AssertCorrect(null, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje de suposición correcta
		/// </summary>
		public LogModel AssertCorrect(BlockLogModel block, string message, [CallerFilePath] string fileName = null, 
									  [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(block, LogModel.LogType.AssertCorrect, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade información destinada a consola
		/// </summary>
		public LogModel Console(string message, [CallerFilePath] string fileName = null, 
								[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Console(null, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade información destinada a consola
		/// </summary>
		public LogModel Console(BlockLogModel block, string message, [CallerFilePath] string fileName = null, 
								[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(block, LogModel.LogType.Console, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un elemento de log
		/// </summary>
		public LogModel Log(LogModel.LogType type, string message, [CallerFilePath] string fileName = null, 
							[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(null, type, message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un elemento de log (y lanza el evento)
		/// </summary>
		public LogModel Log(BlockLogModel block, LogModel.LogType type, string message, [CallerFilePath] string fileName = null, 
							[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			// Guarda el log
			LastLog = new LogModel(GetLastBlock(block), type, message, fileName, methodName, lineNumber);
			// Devuelve el elemento creado
			return LastLog;
		}

		/// <summary>
		///		Añade un parámetro al log
		/// </summary>
		public void AddParameter(string name, object value)
		{
			if (LastLog != null)
				LastLog.AddParameter(name, value);
		}

		/// <summary>
		///		Inicializa el log
		/// </summary>
		public void Reset()
		{
			LastLog = null;
		}

		/// <summary>
		///		Contexto de los elementos de log
		/// </summary>
		public ContextModel Context { get; }

		/// <summary>
		///		Ultimo elemento de log añadido
		/// </summary>
		public LogModel LastLog 
		{ 
			get { return _lastLog; }
			private set
			{
				// Lanza el evento con el último elemento de log
				if (_lastLog != null)
					Context.RaiseLog(_lastLog);
				// Guarda el elemento de log
				_lastLog = value;
			}
		}
	}
}
