using System;

namespace Bau.SparkScripts.Studio.Controllers
{
	/// <summary>
	///		controlador principal de la aplicación
	/// </summary>
	public class HostController : Libraries.BauMvvm.Views.Controllers.HostController
	{
		public HostController(string applicationName, MainWindow mainWindow) : base(applicationName, mainWindow) {}
	}
}
