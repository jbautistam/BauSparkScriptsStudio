using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibLogger.Models.Metrics.Base
{
	/// <summary>
	///		Diccionario normalizado
	/// </summary>
	public class NormalizedModelDictionary<TypeData> where TypeData : class
	{
		/// <summary>
		///		Añade un elemento
		/// </summary>
		public TypeData Add(string key, TypeData item)
		{
			// Normaliza la clave
			key = Normalize(key);
			// Añade o modifica un elemento del diccionario
			if (!Dictionary.ContainsKey(key))
				Dictionary.Add(key, item);
			else
				Dictionary[key] = item;
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
		public TypeData Get(string key)
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
		///		Enumera los valores del diccionario
		/// </summary>
		public IEnumerable<(string key, TypeData value)> Enumerate()
		{
			foreach (KeyValuePair<string, TypeData> item in Dictionary)
				yield return (item.Key, item.Value);
		}

		/// <summary>
		///		Limpia los datos
		/// </summary>
		public void Clear()
		{
			Dictionary.Clear();
		}

		/// <summary>
		///		Normaliza la clave
		/// </summary>
		private string Normalize(string key)
		{
			return key.Trim().ToUpper();
		}

		/// <summary>
		///		Diccionario
		/// </summary>
		protected Dictionary<string, TypeData> Dictionary { get; } = new Dictionary<string, TypeData>();

		/// <summary>
		///		Número de elementos
		/// </summary>
		public int Count 
		{ 
			get { return Dictionary.Count; }
		}

		/// <summary>
		///		Indizador de elementos
		/// </summary>
		public TypeData this[string key]
		{
			get { return Get(key); }
			set { Add(key, value); }
		}
	}
}
