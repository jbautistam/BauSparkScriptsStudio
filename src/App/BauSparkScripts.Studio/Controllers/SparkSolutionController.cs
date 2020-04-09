using System;

using Bau.Libraries.BauMvvm.Views.Controllers;
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
			HostHelperController = new HostHelperController(mainWindow);
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
							result = HostHelperController.ShowDialog(MainWindow, new Views.Connections.ConnectionView(viewModel));
						break;
					default:
							OpenWindowRequired?.Invoke(this, detailViewModel);
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialog(Libraries.BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel)
		{
			SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.Yes;

				// Muestra la ventana adecuada
				switch (dialogViewModel)
				{
					case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Cloud.StorageViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Cloud.StorageView(viewModel));
						break;
					case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections.ConnectionViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Connections.ConnectionView(viewModel));
						break;
					case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files.CsvFilePropertiesViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Files.CsvFilePropertiesView(viewModel));
						break;
					case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Deployments.DeploymentViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Deployments.DeploymentView(viewModel));
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Abre el explorador de Windows sobre un directorio
		/// </summary>
		public void OpenExplorer(string path)
		{
			Libraries.LibSystem.Files.WindowsFiles.ExecuteApplication("explorer.exe", path);
		}

		/// <summary>
		///		Obtiene el viewModel activo de detalles
		/// </summary>
		public IDetailViewModel GetActiveDetails()
		{
			return MainWindow.GetActiveDetails();
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public IHostController HostController { get; }

		/// <summary>
		///		Controlador de Windows
		/// </summary>
		public HostHelperController HostHelperController { get; }

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
