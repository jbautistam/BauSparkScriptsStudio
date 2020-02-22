using System;

namespace Bau.Libraries.LibDataStructures.Base
{
	/// <summary>
	///		Colección de <see cref="BaseExtendedModel"/>
	/// </summary>
	public class BaseExtendedModelCollection<TypeData> : BaseModelCollection<TypeData> where TypeData : BaseExtendedModel
	{
		/// <summary>
		///		Ordena por nombre
		/// </summary>
		public void SortByName(bool ascending = true)
		{ 
			Sort(new Tools.Comparers.BaseExtendedModelComparerByName(ascending));
		}

		/// <summary>
		///		Busca un registro en la colección a partir del nombre
		/// </summary>
		public virtual TypeData SearchByName(string name) 
		{ 
			// Normaliza el nombre a buscar
			if (!string.IsNullOrEmpty(name))
				name = name.Trim();
			// Busca el nombre en la colección
			foreach (TypeData item in this)
				if (!string.IsNullOrEmpty(item.Name) && item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
					return item;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Comprueba si existe una carpeta por nombre
		/// </summary>
		public virtual bool ExistsByName(string name)
		{
			return SearchByName(name) != null;
		}
	}
}
