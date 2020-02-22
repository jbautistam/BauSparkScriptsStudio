using System;
using System.Collections.Generic;
using System.Linq;

namespace Bau.Libraries.LibMarkupLanguage
{
	/// <summary>
	///		Colección de <see cref="MLNameSpace"/>
	/// </summary>
	public class MLNameSpacesCollection : List<MLNameSpace>
	{
		/// <summary>
		///		Añade un espacio de nombres
		/// </summary>
		public void Add(string prefix, string nameSpace)
		{
			if (!Exists(prefix))
				Add(new MLNameSpace(prefix, nameSpace));
		}

		/// <summary>
		///		Comprueba si existe un prefijo
		/// </summary>
		private bool Exists(string prefix)
		{ 
			// Recorre la colección
			foreach (MLNameSpace nameSpace in this)
				if (nameSpace.Prefix == prefix)
					return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Busca un espacio de nombre por su prefijo
		/// </summary>
		public MLNameSpace Search(string prefix)
		{
			return this.FirstOrDefault(nameSpace => nameSpace.Prefix.Equals(prefix));
		}

		/// <summary>
		///		Obtiene el espacio de nombres que coincide con un prefijo
		/// </summary>
		public MLNameSpace this [string prefix]
		{
			get
			{
				MLNameSpace nameSpace = Search(prefix);

					if (nameSpace == null)
						return new MLNameSpace(prefix, "");
					else
						return nameSpace;
			}
			set
			{
				MLNameSpace nameSpace = Search(prefix);

					if (nameSpace == null)
						Add(prefix, value.NameSpace);
					else
						nameSpace.NameSpace = value.NameSpace;
			}
		}
	}
}
