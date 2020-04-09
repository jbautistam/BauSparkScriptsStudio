using System;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Controllers
{
	/// <summary>
	///		Interface para los controladores de solución
	/// </summary>
	public interface ISparkSolutionController
	{
		/// <summary>
		///		Controlador principal
		/// </summary>
		BauMvvm.ViewModels.Controllers.IHostController HostController { get; }

		/// <summary>
		///		Controlador de log
		/// </summary>
		LibLogger.Core.LogManager Logger { get; }

		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		string AppName { get; }

		/// <summary>
		///		Directorio de aplicación
		/// </summary>
		string AppPath { get; }

		/// <summary>
		///		Abre una ventana de detalles
		/// </summary>
		BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailsViewModel);

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel);

		/// <summary>
		///		Abre el explorador sobre un directorio
		/// </summary>
		void OpenExplorer(string path);

		/// <summary>
		///		Configuración
		/// </summary>
		Configuration.ConfigurationModel Configuration { get; }

		IDetailViewModel GetActiveDetails();
	}
}
