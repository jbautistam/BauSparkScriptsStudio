using System;
using System.Runtime.Serialization;

namespace Bau.Libraries.LibLogger.Writer.AppInsights
{
	/// <summary>
	///		Excepción para los elementos de log
	/// </summary>
	internal class AppException : Exception
	{
		public AppException() {}

		public AppException(string message) : base(message) {}

		public AppException(string message, Exception innerException) : base(message, innerException) {}

		protected AppException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public AppException(Models.Log.LogModel log) : base(log.Message, log.Exception)
		{
		}
	}
}
