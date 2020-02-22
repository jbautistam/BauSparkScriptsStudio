using System;
using System.Windows;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.BauMvvm.Views.Controllers
{
	/// <summary>
	///		Controlador de cuadros de diálogo
	/// </summary>
	public class HostDialogsController: IHostDialogsController
	{
		public HostDialogsController(string applicationName, Window mainWindow)
		{
			ApplicationName = applicationName;
			MainWindow = mainWindow;
		}

		/// <summary>
		///		Convierte el resultado de un cuadro de diálogo
		/// </summary>
		private SystemControllerEnums.ResultType ConvertDialogResult(bool? result)
		{
			if (result == null)
				return SystemControllerEnums.ResultType.Cancel;
			else if (result ?? false)
				return SystemControllerEnums.ResultType.Yes;
			else
				return SystemControllerEnums.ResultType.No;
		}

		/// <summary>
		///		Abre el cuadro de diálogo de carga de archivos
		/// </summary>
		public string OpenDialogLoad(string defaultPath, string filter, string defaultFileName = null, string defaultExtension = null)
		{
			string[] files = OpenDialogLoadFiles(false, defaultPath, filter, defaultFileName, defaultExtension);

				// Devuelve el archivo
				if (files != null && files.Length > 0)
					return files[0];
				else
					return null;
		}

		/// <summary>
		///		Abre el cuadro de diálogo de carga de varios archivos
		/// </summary>
		public string[] OpenDialogLoadFilesMultiple(string defaultPath, string filter, string defaultFileName = null, string defaultExtension = null)
		{
			return OpenDialogLoadFiles(true, defaultPath, filter, defaultFileName, defaultExtension);
		}

		/// <summary>
		///		Abre el cuadro de diálogo de carga de varios archivos
		/// </summary>
		public string[] OpenDialogLoadFiles(bool multiple, string defaultPath, string filter, string defaultFileName, string defaultExtension)
		{
			Microsoft.Win32.OpenFileDialog file = new Microsoft.Win32.OpenFileDialog();

				// Si no se le ha pasado un directorio predeterminado, escoge "Mis documentos"
				if (string.IsNullOrWhiteSpace(defaultPath))
					defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				// Asigna las propiedades
				file.Multiselect = multiple;
				file.InitialDirectory = defaultPath;
				file.FileName = defaultFileName;
				file.DefaultExt = defaultExtension;
				file.Filter = filter;
				// Muestra el cuadro de diálogo y devuelve los archivos
				if (file.ShowDialog() ?? false)
					return file.FileNames;
				else
					return null;
		}

		/// <summary>
		///		Abre el cuadro de diálogo de grabación de archivos
		/// </summary>
		public string OpenDialogSave(string defaultPath, string filter, string defaultFileName = null, string defaultExtension = null)
		{
			Microsoft.Win32.SaveFileDialog file = new Microsoft.Win32.SaveFileDialog();

				// Asigna las propiedades
				file.InitialDirectory = defaultPath;
				file.FileName = defaultFileName;
				file.DefaultExt = defaultExtension;
				file.Filter = filter;
				// Muestra el cuadro de diálogo
				if (file.ShowDialog() ?? false)
					return file.FileName;
				else
					return null;
		}

		/// <summary>
		///		Muestra un cuadro de diálogo
		/// </summary>
		internal SystemControllerEnums.ResultType ShowDialog(Window owner, Window view)
		{ 
			// Si no se le ha pasado una ventana propietario, le asigna una
			if (owner == null)
				owner = MainWindow;
			// Muestra el formulario activo
			view.Owner = owner;
			view.ShowActivated = true;
			// Muestra el formulario y devuelve el resultado
			return ConvertDialogResult(view.ShowDialog());
		}

		/// <summary>
		///		Abre el cuadro de diálogo de selección de un directorio
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialogSelectPath(string pathource, out string path)
		{
			Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folder = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			SystemControllerEnums.ResultType type;

				// Inicializa los valores de salida
				path = null;
				// Asigna la carpeta inicial
				folder.SelectedPath = pathource;
				folder.ShowNewFolderButton = true;
				// Muestra el diálogo
				type = ConvertDialogResult(folder.ShowDialog());
				// Obtiene el directorio
				if (type == SystemControllerEnums.ResultType.Yes)
					path = folder.SelectedPath;
				// Devuelve el resultado
				return type;
		}

		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		public string ApplicationName { get; set; }

		/// <summary>
		///		Ventana principal de la aplicación
		/// </summary>
		public Window MainWindow { get; }
	}
}
