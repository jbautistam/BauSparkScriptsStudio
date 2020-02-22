using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibMarkupLanguage
{
	/// <summary>
	///		Colección de <see cref="MLItemBase"/>
	/// </summary>
	public class MLItemsBaseCollection<TypeData> : List<TypeData> where TypeData : MLItemBase, new()
	{
		/// <summary>
		///		Añade una colección de elementos
		/// </summary>
		public void Add(MLItemsBaseCollection<TypeData> items)
		{
			foreach (TypeData item in items)
				Add(item);
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string name)
		{
			return Add(null, name, string.Empty);
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string name, string value)
		{
			return Add(null, name, value);
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string name, bool value)
		{
			return Add(null, name, value);
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string name, double? value)
		{
			return Add(null, name, value);
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string name, DateTime? value)
		{
			return Add(null, name, value);
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string prefix, string name, bool value)
		{
			return Add(prefix, name, MLItemBase.Format(value));
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string prefix, string name, double? value)
		{
			return Add(prefix, name, MLItemBase.Format(value));
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string prefix, string name, DateTime? value)
		{
			return Add(prefix, name, MLItemBase.Format(value));
		}

		/// <summary>
		///		Añade un nodo a la colección
		/// </summary>
		public TypeData Add(string prefix, string name, string value)
		{
			TypeData data = new TypeData {
											Prefix = prefix,
											Name = name,
											Value = value
										 };

				// Añade el nodo
				Add(data);
				// Devuelve el objeto
				return data;
		}

		/// <summary>
		///		Busca un elemento en la colección
		/// </summary>
		public TypeData Search(string name)
		{ 
			// Busca el elemento en la colección
			foreach (TypeData data in this)
				if (data.Name.Equals(name))
					return data;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Indizador por el nombre del elemento
		/// </summary>
		public TypeData this [string name]
		{
			get
			{
				TypeData data = Search(name);

					if (data == null)
						return new TypeData();
					else
						return data;
			}
			set
			{
				TypeData data = Search(name);

					if (data != null)
						data = value;
			}
		}
	}
}
