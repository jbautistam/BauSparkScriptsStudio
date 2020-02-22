using System;

namespace Bau.Libraries.LibDataStructures.Collections
{
	/// <summary>
	///		Datos de un parámetro
	/// </summary>
	public class ParameterModel
	{
		public ParameterModel(string id, object value)
		{
			Id = id;
			Value = value;
		}

		/// <summary>
		///		Clave del parámetro
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///		Obtiene una cadena a partir del valor
		/// </summary>
		public string GetValueString()
		{
			if (Value == null)
				return " ";
			else
				return Value.ToString();
		}

		/// <summary>
		///		Valor del parámetro
		/// </summary>
		public object Value { get; set; }
	}
}
