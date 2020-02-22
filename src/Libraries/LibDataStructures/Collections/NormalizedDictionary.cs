using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDataStructures.Collections
{
	/// <summary>
	///		Diccionario normalizado
	/// </summary>
    public class NormalizedDictionary<TypeData> where TypeData : class
    {
		public NormalizedDictionary(bool replaceDuplicates = true)
		{
			ReplaceDuplicates = replaceDuplicates;
		}

		/// <summary>
		///		Añade un elemmento
		/// </summary>
		public void Add(string key, TypeData value)
		{
			// Normaliza la clave
			key = Normalize(key);
			// Añade / modifica la clave
			if (!InternalDictionary.ContainsKey(key))
				InternalDictionary.Add(key, value);
			else if (ReplaceDuplicates)
				this[key] = value;
			else
				throw new KeyNotFoundException();
		}

		/// <summary>
		///		Añade un diccionario a este diccionario
		/// </summary>
		public void AddRange(NormalizedDictionary<TypeData> data)
		{
			foreach ((string key, TypeData value) in data.Enumerate())
				Add(key, value);
		}

		/// <summary>
		///		Clona el diccionario
		/// </summary>
		public NormalizedDictionary<TypeData> Clone()
		{
			NormalizedDictionary<TypeData> target = new NormalizedDictionary<TypeData>();

				// Clona los registros
				foreach ((string key, TypeData value) in Enumerate())
					target.Add(key, value);
				// Devuelve el diccionario clonado
				return target;
		}

		/// <summary>
		///		Comprueba si contiene un elemento
		/// </summary>
		public bool ContainsKey(string key)
		{
			return InternalDictionary.ContainsKey(Normalize(key));
		}

		/// <summary>
		///		Elimina un elemento
		/// </summary>
		public void Remove(string key)
		{
			// Normaliza la clave
			key = Normalize(key);
			// Elimina el elemento
			if (InternalDictionary.ContainsKey(key))
				InternalDictionary.Remove(key);
		}

		/// <summary>
		///		Limpia los elementos
		/// </summary>
		public void Clear()
		{
			InternalDictionary.Clear();
		}

		/// <summary>
		///		Normaliza una clave
		/// </summary>
		private string Normalize(string key)
		{
			return key.ToUpperInvariant();
		}

		/// <summary>
		///		Obtiene los valores del diccionario
		/// </summary>
		public IEnumerable<(string Key, TypeData Value)> Enumerate()
		{
			foreach (KeyValuePair<string, TypeData> keyValue in InternalDictionary)
				yield return (keyValue.Key, keyValue.Value);
		}

		/// <summary>
		///		Obtiene un valor
		/// </summary>
		public TypeData this[string key]
		{
			get
			{
				// Normaliza la clave
				key = Normalize(key);
				// Devuelve el valor
				return InternalDictionary.ContainsKey(key) ? InternalDictionary[key] : null;
			}
			set
			{
				// Normaliza la clave
				key = Normalize(key);
				// Cambia el valor
				if (InternalDictionary.ContainsKey(key))
					InternalDictionary.Remove(key);
				InternalDictionary.Add(key, value);
			}
		}

		/// <summary>
		///		Obtiene el diccionario interno
		/// </summary>
		public Dictionary<string, TypeData> ToDictionary()
		{
			Dictionary<string, TypeData> converted = new Dictionary<string, TypeData>();

				// Convierte los valores
				foreach ((string key, TypeData value) in Enumerate())
					converted.Add(key, value);
				// Devuelve el diccionario
				return converted;
		}

		/// <summary>
		///		Número de elementos
		/// </summary>
		public int Count 
		{ 
			get { return InternalDictionary.Count; } 
		}

		/// <summary>
		///		Diccionario interno
		/// </summary>
		private Dictionary<string, TypeData> InternalDictionary { get; } = new Dictionary<string, TypeData>();

		/// <summary>
		///		Reemplaza los duplicados
		/// </summary>
		public bool ReplaceDuplicates { get; set; }
	}
}
