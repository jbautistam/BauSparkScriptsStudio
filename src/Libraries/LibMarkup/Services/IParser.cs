using System;

namespace Bau.Libraries.LibMarkupLanguage.Services
{
	/// <summary>
	///		Interface de los intérpretes
	/// </summary>
	public interface IParser
	{
		/// <summary>
		///		Interpreta un texto
		/// </summary>
		MLFile ParseText(string text);
	}
}
