using System;

namespace Bau.SparkScripts.Studio.Controllers
{
	/// <summary>
	///		Controlador para la configuración de la aplicación
	/// </summary>
	public class AppConfigurationController
	{
		/// <summary>
		///		Carga la configuración
		/// </summary>
		public void Load()
		{
			LastPathSelected = Properties.Settings.Default.LastPathSelected;
			LastThemeSelected = Properties.Settings.Default.LastThemeSelected;
		}

		/// <summary>
		///		Graba la configuración
		/// </summary>
		public void Save()
		{
			// Asigna las propiedades
			Properties.Settings.Default.LastPathSelected = LastPathSelected;
			Properties.Settings.Default.LastThemeSelected = LastThemeSelected;
			// Graba la configuración
			Properties.Settings.Default.Save();
		}

		/// <summary>
		///		Ultimo directorio seleccionado para grabación
		/// </summary>
		public string LastPathSelected { get; set; }

		/// <summary>
		///		Ultimo tema seleccionado
		/// </summary>
		public int LastThemeSelected { get; set; }
	}
}
