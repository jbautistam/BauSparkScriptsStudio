﻿using System;

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
			EditorFontName = Properties.Settings.Default.EditorFontName;
			EditorFontSize = Properties.Settings.Default.EditorFontSize;
			EditorShowLinesNumber = Properties.Settings.Default.EditorShowLinesNumber;
			EditorZoom = Properties.Settings.Default.EditorZoom;
		}

		/// <summary>
		///		Graba la configuración
		/// </summary>
		public void Save()
		{
			// Asigna las propiedades
			Properties.Settings.Default.LastPathSelected = LastPathSelected;
			Properties.Settings.Default.LastThemeSelected = LastThemeSelected;
			Properties.Settings.Default.EditorFontName = EditorFontName;
			Properties.Settings.Default.EditorFontSize = EditorFontSize;
			Properties.Settings.Default.EditorShowLinesNumber = EditorShowLinesNumber;
			Properties.Settings.Default.EditorZoom = EditorZoom;
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

		/// <summary>
		///		Nombre de la fuente del editor
		/// </summary>
		public string EditorFontName { get; set; } = "Consolas";

		/// <summary>
		///		Tamaños de la fuente del editor
		/// </summary>
		public double EditorFontSize { get; set; } = 18;

		/// <summary>
		///		Indica si se debe mostrar el número de línea en el editor
		/// </summary>
		public bool EditorShowLinesNumber { get; set; } = true;

		/// <summary>
		///		Indica el nivel de zoom del editor
		/// </summary>
		public double EditorZoom { get; set; } = 1.0;
	}
}
