using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDataStructures.Base
{
	/// <summary>
	///		Clase base
	/// </summary>
	public class BaseModel
	{	
		/// <summary>
		///		Comprueba si un objeto es nulo (o está vacío)
		/// </summary>
		public static bool IsNull(BaseModel value)
		{ 
			return value == null || value.ID == null;
		}

		/// <summary>
		///		Asigna un valor a otro si no tienen la misma referencia y en su caso lanza el evento de 
		///	modificación de propiedad (para objetos LazyObject&lt;BaseModel&gt;)
		/// </summary>
		protected void CheckProperty<TypeData>(LazyObject<TypeData> target, TypeData value) where TypeData : BaseModel, new()
		{	
			if (!target.IsAssigned || !ReferenceEquals(target.Data, value))
				target.Data = value;
		}

		/// <summary>
		///		Asigna un valor a otro si no tienen la mismma referencia y en su caso lanza el evento de 
		///	modificación de propiedad (para objetos LazyObject&lt;BaseModelCollection&gt;)
		/// </summary>
		protected void CheckProperty<TypeCollection, TypeData>(LazyObject<TypeCollection> target, TypeCollection value) 
												where TypeCollection : BaseModelCollection<TypeData>, new()
												where TypeData : BaseModel, new()
		{	
			if (!target.IsAssigned || !ReferenceEquals(target.Data, value))
				target.Data = value;
		}

		/// <summary>
		///		ID del elemento
		/// </summary>
		public int? ID { get; set; }

		/// <summary>
		///		Identificador global
		/// </summary>
		public string GlobalId { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		///		Indica si el elemento se considera vacío (aún no tiene ID)
		/// </summary>
		public bool IsEmpty
		{ 
			get { return ID == null; }
		}
	}
}
