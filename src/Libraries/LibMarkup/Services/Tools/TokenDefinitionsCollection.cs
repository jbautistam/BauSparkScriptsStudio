using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibMarkupLanguage.Services.Tools
{
	/// <summary>
	///		Colección de <see cref="TokenDefinition"/>
	/// </summary>
	internal class TokenDefinitionsCollection : List<TokenDefinition>
	{
		/// <summary>
		///		Añade un token a la colección para datos numéricos
		/// </summary>
		internal void Add(int? type, string name)
		{
			Add(type, name, null, null);
		}

		/// <summary>
		///		Añade un token a la colección
		/// </summary>
		internal void Add(int? type, string name, string start)
		{
			Add(type, name, start, null);
		}

		/// <summary>
		///		Añade un token a la colección
		/// </summary>
		internal void Add(int? type, string name, string start, string end)
		{
			Add(new TokenDefinition(type, name, start, end));
		}

		/// <summary>
		///		Comprueba si existe algún token para los valores numéricos
		/// </summary>
		internal bool ExistsNumeric()
		{
			return SearchNumeric() != null;
		}

		/// <summary>
		///		Obtiene la definición de token para los valores numéricos
		/// </summary>
		internal TokenDefinition SearchNumeric()
		{ 
			// Recorre la colección
			foreach (TokenDefinition definition in this)
				if (definition.IsNumeric)
					return definition;
			// Si ha llegado hasta aquí es porque no ha encontrado ninguna definición para tokens numéricos
			return null;
		}
	}
}
