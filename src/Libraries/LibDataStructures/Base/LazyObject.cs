using System;

namespace Bau.Libraries.LibDataStructures.Base
{
	/// <summary>
	///		Objetos Lazy
	/// </summary>
	/// <remarks>
	///		TypeData desciende de class en lugar de <see cref="BaseModel"/>  porque también se pueden definir desde
	///	<see cref="BaseModelCollection"/> 
	/// </remarks>
	public class LazyObject<TypeData> where TypeData : class, new()
	{ 
		// Variables privadas
		private int? _id;
		private TypeData _data;

		public LazyObject()
		{
			IsAssigned = false;
		}

		/// <summary>
		///		ID asociada a un elemento Lazy
		/// </summary>
		public int? ID
		{
			get
			{
				if (IsAssigned)
				{
					if (Data is BaseModel data)
						return data.ID;
					else
						return null;
				}
				else
					return _id;
			}
			set { _id = value; }
		}

		/// <summary>
		///		Datos asociados (Lazy)
		/// </summary>
		public Lazy<TypeData> LazyData { get; set; }

		/// <summary>
		///		Datos asociados
		/// </summary>
		public TypeData Data
		{
			get
			{ 
				// Carga los datos si no estaban en memoria
				if (!IsAssigned && _data == null)
				{ 
					// Carga desde el objeto Lazy o lo crea de nuevo
					if (LazyData != null && LazyData.Value != null)
						_data = LazyData.Value;
					else
						_data = new TypeData();
					// Indica que se ha asignado el valor
					IsAssigned = true;
				}
				// Devuelve el objeto
				return _data;
			}
			set
			{ 
				// Asigna el valor
				_data = value;
				// Quita los métodos Lazy
				LazyData = null;
				// Indica que ya se ha asignado el valor
				IsAssigned = true;
			}
		}

		/// <summary>
		///		Indica si se han asignado los datos
		/// </summary>
		public bool IsAssigned { get; private set; }
	}
}
