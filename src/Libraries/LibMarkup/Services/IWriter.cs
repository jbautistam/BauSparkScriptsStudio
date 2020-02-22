using System;

namespace Bau.Libraries.LibMarkupLanguage.Services
{
	/// <summary>
	///		Interface para el generador de un archivo o texto de un MLFile
	/// </summary>
	public interface IWriter
	{
		/// <summary>
		///		Convierte los datos en una cadena
		/// </summary>
		string ConvertToString(MLFile fileML, bool addHeader = true);
	}
}
