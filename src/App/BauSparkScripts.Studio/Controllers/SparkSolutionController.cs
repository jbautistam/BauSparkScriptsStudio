using System;
using System.Windows;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details;

namespace Bau.SparkScripts.Studio.Controllers
{
	/// <summary>
	///		Controlador principal
	/// </summary>
	public class SparkSolutionController : Libraries.BauSparkScripts.ViewModels.Controllers.ISparkSolutionController
	{
		// Eventos públicos
		public event EventHandler<IDetailViewModel> OpenWindowRequired;

		public SparkSolutionController(string applicationName, MainWindow mainWindow, string appPath)
		{
			// Asigna las propiedades
			HostController = new HostController(applicationName, mainWindow);
			MainWindow = mainWindow;
			Logger = new Libraries.LibLogger.Core.LogManager();
			AppName = applicationName;
			Configuration = new Libraries.BauSparkScripts.ViewModels.Configuration.ConfigurationModel(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
			// Directorio de aplicación
			if (string.IsNullOrWhiteSpace(appPath))
				appPath = Environment.CurrentDirectory;
			AppPath = System.IO.Path.Combine(appPath, applicationName);
			// Crea el directorio de aplicación
			Libraries.LibHelper.Files.HelperFiles.MakePath(appPath);
		}

		/// <summary>
		///		Abre una ventana de detalles
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailViewModel)
		{
			SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.Yes;

				// Muestra la ventana adecuada
				switch (detailViewModel)
				{
					case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections.ConnectionViewModel viewModel:
							result = ShowDialog(MainWindow, new Views.Connections.ConnectionView(viewModel));
						break;
					default:
							OpenWindowRequired?.Invoke(this, detailViewModel);
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Muestra un cuadro de diálogo
		/// </summary>
		//TODO --> Esto debería estar en el controlador base de las vistas pero depende de System.Windows (busca una solución)
		private SystemControllerEnums.ResultType ShowDialog(Window owner, Window view, WindowStyle style = WindowStyle.ToolWindow)
		{ 
			// Si no se le ha pasado una ventana propietario, le asigna una
			if (owner == null)
				owner = MainWindow;
			// Muestra el formulario activo
			view.Owner = owner;
			view.ShowActivated = true;
			view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			view.WindowStyle = style;
			if (style == WindowStyle.ToolWindow)
				view.ResizeMode = ResizeMode.NoResize;
			// Muestra el formulario y devuelve el resultado
			return ConvertDialogResult(view.ShowDialog());
		}

		/// <summary>
		///		Convierte el resultado de un cuadro de diálogo
		/// </summary>
		//TODO --> Esto debería estar en el controlador base de las vistas pero depende de System.Windows
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
		///		Controlador principal
		/// </summary>
		public IHostController HostController { get; }

		/// <summary>
		///		Logger
		/// </summary>
		public Libraries.LibLogger.Core.LogManager Logger { get; }

		/// <summary>
		///		Ventana principal
		/// </summary>
		internal MainWindow MainWindow { get; }

		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		public string AppName { get; }

		/// <summary>
		///		Directorio de aplicación
		/// </summary>
		public string AppPath { get; }

		/// <summary>
		///		Configuración de la aplicación
		/// </summary>
		public Libraries.BauSparkScripts.ViewModels.Configuration.ConfigurationModel Configuration { get; }
	}
}
