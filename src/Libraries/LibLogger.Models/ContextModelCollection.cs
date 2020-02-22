using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibLogger.Models
{
	/// <summary>
	///		Colección de <see cref="ContextModel"/>
	/// </summary>
	public class ContextModelCollection
	{
		/// <summary>
		///		Añade un elemento
		/// </summary>
		public ContextModel Add(ContextModel item)
		{
			return Add(item.Key, item);
		}

		/// <summary>
		///		Añade un elemento
		/// </summary>
		public ContextModel Add(string key, ContextModel item)
		{
			// Añade el elemento a la colección
			Dictionary.Add(Normalize(key), item);
			// Asigna el manejador de eventos para quitar el elemento del diccionario cuando sea necesario
			item.Disposed += (sender, args) => Dictionary.Remove(item.Key);
			// Devuelve el elemento añadido
			return item;
		}

		/// <summary>
		///		Comprueba si contiene una clave
		/// </summary>
		public bool Contains(string key)
		{
			return Dictionary.ContainsKey(Normalize(key));
		}

		/// <summary>
		///		Obtiene un elemento
		/// </summary>
		public ContextModel Get(string key)
		{
			return Dictionary[Normalize(key)];
		}

		/// <summary>
		///		Borra un elemento
		/// </summary>
		public void Remove(string key)
		{
			Dictionary.Remove(Normalize(key));
		}

		/// <summary>
		///		Normaliza la clave
		/// </summary>
		private string Normalize(string key)
		{
			return key.Trim().ToUpper();
		}

		/// <summary>
		///		Enumera el contenido del diccionario
		/// </summary>
		public IEnumerable<(string key, ContextModel value)> Enumerate()
		{
			foreach (KeyValuePair<string, ContextModel> item in Dictionary)
				yield return (item.Key, item.Value);
		}

		/// <summary>
		///		Diccionario
		/// </summary>
		protected Dictionary<string, ContextModel> Dictionary { get; } = new Dictionary<string, ContextModel>();

		/// <summary>
		///		Cuenta el número de elementos
		/// </summary>
		public int Count 
		{
			get { return Dictionary.Count; }
		}
	}
}