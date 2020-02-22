using System;
using System.Collections.Generic;
using System.Linq;

namespace Bau.Libraries.LibDataStructures.Collections
{
	/// <summary>
	///		Colección de <see cref="ParameterModel"/>
	/// </summary>
	public class ParameterModelCollection : List<ParameterModel>
	{
		/// <summary>
		///		Añade un parámetro del filtro
		/// </summary>
		public void Add(string id, object value)
		{
			ParameterModel parameter = Search(id);

				// Asigna un valor al parámetro localizado o lo añade a la colección
				if (parameter == null)
					Add(new ParameterModel(id, value));
				else
					parameter.Value = value;
		}

		/// <summary>
		///		Busca un elemento en la colección
		/// </summary>
		public ParameterModel Search(string id)
		{
			return this.FirstOrDefault(item => item.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
		}

		/// <summary>
		///		Indizador por clave
		/// </summary>
		public ParameterModel this[string id]
		{
			get 
			{ 
				ParameterModel parameter = Search(id);

					if (parameter == null)
						return new ParameterModel(id, null);
					else
						return parameter;
			}
			set 
			{ 
				ParameterModel parameter = Search(id);

					if (parameter == null)
						Add(id, value);
					else
						parameter.Value = value; 
			}
		}
	}
}
