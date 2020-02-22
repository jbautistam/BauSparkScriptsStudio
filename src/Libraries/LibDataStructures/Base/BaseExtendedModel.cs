using System;

namespace Bau.Libraries.LibDataStructures.Base
{
	/// <summary>
	///		Clase base para los elementos con nombre y descripción
	/// </summary>
	public class BaseExtendedModel : BaseModel
	{	
		/// <summary>
		///		Nombre
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		///		Descripción
		/// </summary>
		public virtual string Description { get; set; }
	}
}
