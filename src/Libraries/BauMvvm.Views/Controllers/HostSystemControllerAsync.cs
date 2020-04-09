using System;
using System.Threading.Tasks;
using System.Windows;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.BauMvvm.Views.Controllers
{
	/// <summary>
	///		Controlador de ventanas comunes
	/// </summary>
	public class HostSystemControllerAsync : IHostSystemControllerAsync
	{
		public HostSystemControllerAsync(string applicationName, Window mainWindow)
		{
			ApplicationName = applicationName;
			MainWindow = mainWindow;
		}

		/// <summary>
		///		Muestra un cuadro de mensaje
		/// </summary>
		public async Task ShowMessageAsync(string message)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Muestra un cuadro de mensaje
		/// </summary>
		public async Task<bool> ShowQuestionAsync(string message, string acceptTitle = "Aceptar", string cancelTitle = "Cancelar")
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Muestra un cuadro de mensaje para introducir un texto
		/// </summary>
		public Task<(SystemControllerEnums.ResultType result, string input)> ShowInputStringAsync(string message, string defaultValue = null)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Muestra un cuadro de mensaje para introducir un texto
		/// </summary>
		public Task<(SystemControllerEnums.ResultType result, string input)> ShowInputMultilineStringAsync(string message, string defaultValue = null)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Muestra una pregunta con tres posibles respuestas
		/// </summary>
		public Task<SystemControllerEnums.ResultType> ShowQuestionCancelAsync(string message)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Muestra una notificación
		/// </summary>
		public virtual Task ShowNotificationAsync(SystemControllerEnums.NotificationType type, string title, string message, TimeSpan expiration, string urlImage = null)
		{
			throw new NotImplementedException();
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
