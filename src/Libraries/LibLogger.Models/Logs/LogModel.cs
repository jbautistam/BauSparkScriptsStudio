using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bau.Libraries.LibLogger.Models.Log
{
	/// <summary>
	///		Clase con los datos de un elemento de log
	/// </summary>
	public class LogModel
	{
		/// <summary>
		///		Tipo de log
		/// </summary>
		public enum LogType
		{
			/// <summary>Depuración</summary>
			Debug,
			/// <summary>Información</summary>
			Info,
			/// <summary>Información para consola</summary>
			Console,
			/// <summary>Advertencia</summary>
			Warning,
			/// <summary>Error</summary>
			Error,
			/// <summary>Traza</summary>
			Trace,
			/// <summary>Progreso de tarea</summary>
			Progress,
			/// <summary>Error en suposición</summary>
			AssertError,
			/// <summary>Suposición correcta</summary>
			AssertCorrect
		}
		/// <summary>
		///		Modelos de conversión del log en cadena
		/// </summary>
		public enum WriteMode
		{
			/// <summary>Log corto</summary>
			Short,
			/// <summary>Log completo</summary>
			Full
		}

		public LogModel(BlockLogModel parent, LogType type, string message, [CallerFilePath] string fileName = null, 
						[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			// Propiedades del log
			Parent = parent;
			Type = type;
			Message = message;
			if (parent == null)
				Level = 0;
			else
				Level = parent.Level + 1;
			// Asigna los datos del método de llamada
			FileName = fileName;
			MethodName = methodName;
			LineNumber = lineNumber;
		}

		/// <summary>
		///		Añade un parámetro al log
		/// </summary>
		public void AddParameter(string name, object value)
		{
			Parameters.Add(name, value);
		}

		/// <summary>
		///		Convierte el log en una cadena
		/// </summary>
		public string ToString(WriteMode mode, int indent = 0)
		{
			string line = $"{GetIndent(indent)}[{Type.ToString().ToUpper()}] [{CreatedAt:HH:mm:ss}] [{Id.ToString()}] [{Parent?.Id.ToString()}]";

				// Añade los datos de log
				line += $" {FileName} | {MethodName} | {LineNumber.ToString()}" + Environment.NewLine;
				line += $"{GetIndent(indent)}{Message}".Replace('\r', ' ').Replace('\n', ' ');
				if (mode == WriteMode.Full)
					line += Convert(Parameters, indent + 1);
				line += Convert(Exception, mode, indent + 1);
				// Devuelve la línea convertida
				return line;
		}

		/// <summary>
		///		Obtiene una cadena de indentación
		/// </summary>
		private string GetIndent(int indent)
		{
			if (indent == 0)
				return string.Empty;
			else
				return new string('\t', indent);
		}

		/// <summary>
		///		Convierte los datos de una excepción para el log
		/// </summary>
		private string Convert(Exception exception, WriteMode mode, int indent)
		{
			string line = string.Empty;

				// Obtiene la línea con los datos de la excepción
				if (exception != null)
					switch (mode)
					{
						case WriteMode.Short:
								line += Environment.NewLine + $"{GetIndent(indent)}{exception.Message}";
							break;
						case WriteMode.Full:
								Exception inner = exception;

									while (inner != null)
									{
										// Añade la línea
										line += Environment.NewLine + $"{GetIndent(indent)}{inner.Message}";
										// Pasa a la excepción interna
										inner = inner.InnerException;
										// E incrementa la indentación
										indent++;
									}
									// Añade la traza
									line += Environment.NewLine + inner.StackTrace;
							break;
					}
				// Devuelve la línea
				return line;
		}

		/// <summary>
		///		Convierte los parámetros de log
		/// </summary>
		private string Convert(Dictionary<string, object> parameters, int indent)
		{
			string line = string.Empty;

				// Convierte los parámetros
				foreach (KeyValuePair<string, object> parameter in parameters)
					line += Environment.NewLine + $"{GetIndent(indent)}- {parameter.Key}: {(parameter.Value ?? "").ToString()}";
				// Devuelve la línea
				return line;
		}

		/// <summary>
		///		Id del elemento
		/// </summary>
		public Guid Id { get; private set; } = Guid.NewGuid();

		/// <summary>
		///		Bloque padre
		/// </summary>
		public BlockLogModel Parent { get; }

		/// <summary>
		///		Nivel del elemento
		/// </summary>
		public int Level { get; }

		/// <summary>
		///		Tipo de log
		/// </summary>
		public LogType Type { get; }

		/// <summary>
		///		Mensaje de log
		/// </summary>
		public string Message { get; }

		/// <summary>
		///		Nombre del archivo desde el que se ha creado el elemento de log
		/// </summary>
		public string FileName { get; }

		/// <summary>
		///		Nombre del método desde el que se ha creado el elemento de log
		/// </summary>
		public string MethodName { get; }

		/// <summary>
		///		Número de línea del archivo desde el que se ha creado el elemento de log
		/// </summary>
		public int LineNumber { get; }

		/// <summary>
		///		Progreso actual
		/// </summary>
		public long ActualProgress { get; set; }

		/// <summary>
		///		Final del progreso
		/// </summary>
		public long TotalProgress { get; set; }

		/// <summary>
		///		Fecha de creación del elemento
		/// </summary>
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		/// <summary>
		///		Excepción
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		///		Parámetros del elemento de log
		/// </summary>
		public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
	}
}
