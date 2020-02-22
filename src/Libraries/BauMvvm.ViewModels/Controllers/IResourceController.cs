using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Controllers
{
	/// <summary>
	///		Controlador para el tratamiento de recursos
	/// </summary>
	public interface IResourceController
	{
		/// <summary>
		///		Obtiene una cadena de recursos localizada
		/// </summary>
		string Localize(string key, string defaultValue = null);

		/// <summary>
		///		Obtiene el valor de una propiedad de configuración
		/// </summary>
		string GetConfigurationProperty(string name, string defaultValue = null);

		/// <summary>
		///		Asigna un valor a una propiedad de configuración
		/// </summary>
		void SetConfigurationProperty(string name, string value);
	}
}
