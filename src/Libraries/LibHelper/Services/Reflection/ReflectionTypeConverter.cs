using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bau.Libraries.LibHelper.Services.Reflection
{
	/// <summary>
	///		Conversor para obtener todos los datos de un tipo en una cadena
	/// </summary>
	public class ReflectionTypeConverter
	{
		public ReflectionTypeConverter(ReflectionService reflectionService)
		{
			Service = reflectionService;
		}

		/// <summary>
		///		Obtiene una lista de elementos con el nombre de la propiedad y el valor
		/// </summary>
		public List<KeyValuePair<string, object>> GetParameters(object value)
		{
			List<PropertyInfo> properties = Service.GetProperties(value.GetType());
			List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();

				// Obtiene los valores de las propiedades
				foreach (PropertyInfo property in properties)
					parameters.Add(new KeyValuePair<string, object>(property.Name, Service.GetValue(value, property.Name)));
				// Devuelve el resultado
				return parameters;
		}

		/// <summary>
		///		Crea un objeto de un tipo y le asigna una serie de valores a sus propiedades. PropertiesValues contiene
		/// la lista de nombres de propiedad / valor
		/// </summary>
		public TypeData Convert<TypeData>(List<KeyValuePair<string, object>> propertiesValues) where TypeData : class, new()
		{
			TypeData result = new TypeData();

				// Antes de asignar los valores, se comprueba los resultados y el tipo ...
				if (propertiesValues == null || propertiesValues.Count == 0)
					throw new ArgumentException("No se encuentra ninguna propiedad");
				// Obtiene los valores de las propiedades
				foreach (KeyValuePair<string, object> property in propertiesValues)
				{
					PropertyInfo propertyInfo = Service.GetProperty(typeof(TypeData), property.Key);

						// Asigna el valor convertido al objeto
						propertyInfo.SetValue(result, ConvertFromString(propertyInfo.PropertyType.ToString(), ConvertToString(property.Value)));
						//? Habría estado bien hacer esto:
						//?		propertyInfo.SetValue(result, property.Value);
						//? pero cuando la propiedad es "int" y el valor es "double" da una excepción indicando que no se puede asignar
						//? un valor de tipo "double" a un valor "Int32".
						//? Por eso se hace una transformación a String y posteriormente al tipo destino
				}
				// Devuelve el objeto
				return result;
		}

		/// <summary>
		///		Convierte un objeto a una cadena invariante (<see cref="System.Globalization.CultureInfo.InvariantCulture"/>)
		/// </summary>
		public string ConvertToString(object value)
		{
			return value == null ? "" : System.Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		///		Convierte una cadena en un objeto dependiendo del tipo
		/// </summary>
		public object ConvertFromString(string typeName, string value)
		{ 
			// Valores nulos
			if (string.IsNullOrEmpty(value))
				return null;
			// Valores lógicos, caracteres, cadenas...
			if (typeName.Equals(typeof(bool).ToString()))
				return System.Convert.ToBoolean(value, System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(char).ToString()))
				return System.Convert.ToChar(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(DateTime).ToString()))
				return System.Convert.ToDateTime(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			// Valores decimales
			if (typeName.Equals(typeof(decimal).ToString()))
				return System.Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(double).ToString()))
				return System.Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(float).ToString()))
				return System.Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture);
			// Valores enteros (hay que quitarle los puntos a las cadenas)
			if (typeName.Equals(typeof(byte).ToString()))
				return System.Convert.ToByte(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(short).ToString()))
				return System.Convert.ToInt16(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(int).ToString()))
				return System.Convert.ToInt32(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(long).ToString()))
				return System.Convert.ToInt64(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(sbyte).ToString()))
				return System.Convert.ToSByte(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(ushort).ToString()))
				return System.Convert.ToUInt16(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(uint).ToString()))
				return System.Convert.ToUInt32(NormalizeToInt(value), System.Globalization.CultureInfo.InvariantCulture);
			if (typeName.Equals(typeof(ulong).ToString()))
				return System.Convert.ToUInt64(value, System.Globalization.CultureInfo.InvariantCulture);
			// En cualquier otro caso, devuelve la misma cadena
			return value;
		}

		/// <summary>
		///		Normaliza una cadena para números enteros (sin puntos)
		/// </summary>
		private string NormalizeToInt(string value)
		{ 
			// Si hay algo que tratar
			if (!string.IsNullOrEmpty(value))
			{
				string[] valueSplit = value.Split('.');

					if (valueSplit.Length > 0)
						return valueSplit[0];
			}
			// Si ha llegado hasta aquí es porque no había nada
			return "";
		}

		/// <summary>
		///		Compara dos objetos: source.Value.Equals(target.Value) devuelve false cuando los objetos son de distinto tipo
		/// aunque tengan el mismo valor, es decir, (0.0).Equals(0) -> false
		/// </summary>
		public bool CompareObjectValues(object source, object target)
		{ 
			// Compara valores numéricos
			if (IsNumeric(source) || IsNumeric(target))
				return Math.Abs(System.Convert.ToDouble(source) - System.Convert.ToDouble(target)) < 0.00001;
			// Compara fechas
			if (IsDateTime(source) || IsDateTime(target))
			{
				DateTime sourceDate = System.Convert.ToDateTime(source);
				DateTime targetDate = System.Convert.ToDateTime(target);

					return sourceDate.Date == targetDate.Date &&
						   sourceDate.Hour == targetDate.Hour &&
						   sourceDate.Minute == targetDate.Minute &&
						   sourceDate.Second == targetDate.Second;
			}
			// Compara el resto de datos
			return source.Equals(target);
		}

		/// <summary>
		///		Comprueba si un objeto es de tipo numérico
		/// </summary>
		private bool IsNumeric(object source)
		{
			return source is decimal || source is double || source is float ||
				   source is byte || source is sbyte ||
				   source is short || source is ushort || source is int || source is uint || source is long || source is ulong;
		}

		/// <summary>
		///		Comprueba si un objeto es de tipo fecha
		/// </summary>
		private bool IsDateTime(object source)
		{
			return source is DateTime;
		}

		/// <summary>
		/// Servicio de reflection utilizado internamente
		/// </summary>
		private ReflectionService Service { get; }
	}
}