using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bau.Libraries.LibHelper.Services
{
	/// <summary>
	/// Servicio para el uso de Reflection
	/// </summary>
	/// <remarks>
	///		* Permite obtener los valores de claves anidadas (Client.Name, por ejemplo)
	///		* Crea una caché de propiedades para llamada a Reflection lo menos posible
	/// </remarks>
	public class ReflectionService
	{ 
		// Diccionario de propiedades para Reflection
		private readonly Dictionary<Type, List<PropertyInfo>> dctProperties = new Dictionary<Type, List<PropertyInfo>>();

		/// <summary>
		/// Obtiene el valor de un campo de un elemento
		/// </summary>
		/// <param name="item">
		/// Objeto del que se van a obtener las propiedades
		/// </param>
		/// <param name="field">
		/// Campo buscado. Puede indicarse un campo anidado, por ejemplo Client.Name obtiene el valor de la propiedad Name de 
		/// la propiedad Client del objeto item
		/// </param>
		public object GetValue(object item, string field)
		{
			object result = null;

				// Obtiene el valor de la propiedad
				if (item != null && !string.IsNullOrEmpty(field))
					result = GetDataValue(item, field);
				// Devuelve el valor localizado de la propiedad
				return result;
		}

		/// <summary>
		/// Obtiene el valor del elemento cuando éste es de un tipo <see cref="object"/>
		/// </summary>
		private object GetDataValue(object item, string field)
		{
			object result = null;
			string prefix = GetToken(ref field);
			List<PropertyInfo> properties = GetProperties(item.GetType());

				// Recorre las propiedades
				foreach (PropertyInfo property in properties)
					try
					{
						if (property.Name.Equals(prefix, StringComparison.CurrentCultureIgnoreCase))
						{
							object value = property.GetValue(item);

								if (value != null)
								{
									if (!string.IsNullOrEmpty(field)) // ... si queda algo en la cadena con el nombre del campo (Objeto.Propiedad)
										result = GetValue(value, field);
									else
										result = value;
								}
						}
					}
					catch
					{
						result = null;
					}
				// Devuelve el valor
				return result;
		}

		/// <summary>
		/// Parte el nombre de un campo por los puntos (por ejemplo: Data.Empresa)
		/// </summary>
		private string GetToken(ref string field)
		{
			string first = field;

				// Separa la cadena por el primer punto
				if (!string.IsNullOrEmpty(field))
				{
					int indexPoint = field.IndexOf('.');

						if (indexPoint > 0)
						{
							first = field.Substring(0, indexPoint);
							if (field.Length > indexPoint)
								field = field.Substring(indexPoint + 1);
							else
								field = "";
						}
						else
							field = "";
				}
				else
					first = "";
				// Devuelve la primera parte de la cadena
				return first;
		}

		/// <summary>
		/// Obtiene la información de una propiedad en concreto
		/// </summary>
		public PropertyInfo GetProperty(Type type, string property)
		{
			List<PropertyInfo> properties = GetProperties(type);

				// Obtiene la propiedad
				foreach (PropertyInfo propertyInfo in properties)
					if (propertyInfo.Name == property)
						return propertyInfo;
				// Si ha llegado hasta aquí es porque no ha encontrado nada
				return null;
		}

		/// <summary>
		/// Obtiene las propiedades de un tipo. Si ya se habían leído, las recoge del diccionario, si no se habían leído, 
		/// utiliza Reflection para obtener las propiedades del objeto
		/// </summary>
		public List<PropertyInfo> GetProperties(Type type)
		{
			// Obtiene las propiedades del diccionario
			if (!dctProperties.TryGetValue(type, out List<PropertyInfo> properties))
			{ 
				// Crea la colección buscando los elementos desde reflection
				properties = new List<PropertyInfo>();
				properties.AddRange(SearchPropertiesReflection(type));
				// Añade las propiedades al diccionario en memoria
				dctProperties.Add(type, properties);
			}
			// Devuelve las propiedades
			return properties;
		}

		/// <summary>
		/// Busca las propiedades recusivamente de un tipo buscando por Reflection
		/// </summary>
		private IEnumerable<PropertyInfo> SearchPropertiesReflection(Type type)
		{
			List<PropertyInfo> properties = new List<PropertyInfo>();
			TypeInfo typeInfo = type.GetTypeInfo();

				// Crea la colección y obtiene las propiedades utilizando Reflection
				properties.AddRange(typeInfo.DeclaredProperties);
				if (typeInfo.BaseType != null)
					properties.AddRange(SearchPropertiesReflection(typeInfo.BaseType));
				// Devuelve la colección de propiedades
				return properties;
		}
	}
}
