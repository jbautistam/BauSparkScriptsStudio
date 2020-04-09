using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.BauMvvm.Views.Controllers
{
	/// <summary>
	///		Base para los controladores de host
	/// </summary>
	public class HostController : IHostController
	{
		public HostController(string applicationName, System.Windows.Window mainWindow)
		{
			DialogsController = new HostDialogsController(applicationName, mainWindow);
			SystemController = new HostSystemController(applicationName, mainWindow);
			SystemControllerAsync = new	HostSystemControllerAsync(applicationName, mainWindow);
		}

		/// <summary>
		///		Controlador de diálogos
		/// </summary>
		public IHostDialogsController DialogsController { get; protected set; }

		/// <summary>
		///		Controlador de sistema
		/// </summary>
		public IHostSystemController SystemController { get; protected set; }

		/// <summary>
		///		Controlador asíncrono de sistema
		/// </summary>
		public IHostSystemControllerAsync SystemControllerAsync { get; protected set; }

		/// <summary>
		///		Controlador de mensajería
		/// </summary>
		public IHostMessengerController MessengerController { get; protected set; }
	}
}
