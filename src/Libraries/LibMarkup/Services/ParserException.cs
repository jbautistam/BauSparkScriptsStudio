using System;

namespace Bau.Libraries.LibMarkupLanguage.Services
{
	/// <summary>
	///		Excepciones del intérprete
	/// </summary>
	public class ParserException : Exception
	{
		public ParserException() {}

		public ParserException(string message) : base(message) { }

		public ParserException(string message, Exception innerException) : base(message, innerException) { }
		
		protected ParserException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
	}
}