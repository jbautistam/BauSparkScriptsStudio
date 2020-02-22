using System;

namespace Bau.Controls.WebExplorers
{
	/// <summary>
	///		Argumentos del evento de llamada a una función externa desde el explorador
	/// </summary>
	public class WebExplorerFunctionEventArgs : EventArgs
	{
		public WebExplorerFunctionEventArgs(string parameters)
		{
			Parameters = parameters;
		}

		/// <summary>
		///		Parámetros de la llamada a función
		/// </summary>
		public string Parameters { get; private set; }
	}
}
