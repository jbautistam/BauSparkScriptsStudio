using System;
using System.Windows;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.BauMvvm.Views.Controllers
{
	/// <summary>
	///		Controlador de ventanas
	/// </summary>
	public interface IHostViewsController
	{ 
		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		string ApplicationName { get; }

		/// <summary>
		///		Muestra un cuadro de diálogo
		/// </summary>
		SystemControllerEnums.ResultType ShowDialog(Window view);

		/// <summary>
		///		Muestra un cuadro de diálogo
		/// </summary>
		SystemControllerEnums.ResultType ShowDialog(Window owner, Window view);
	}
}
