﻿using System;

namespace Bau.Controls.WebExplorers
{
	/// <summary>
	///		Helper para tratamiento de scripts en el navegador
	/// </summary>
	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
	[System.Runtime.InteropServices.ComVisible(true)]
	public class WebExplorerScriptingHelper
	{
		public WebExplorerScriptingHelper(WebExplorerExtended webExplorer)
		{
			WebExplorer = webExplorer;
		}

		/// <summary>
		///		Función a la que se llama cuando se invoca a una función externa desde JavaScript
		/// </summary>
		public void ExternalFunction(string javaScriptArguments)
		{
			WebExplorer.RaiseScriptArguments(javaScriptArguments);
		}

		/// <summary>
		///		Explorador al que se asocia el objeto
		/// </summary>
		public WebExplorerExtended WebExplorer { get; }
	}
}
