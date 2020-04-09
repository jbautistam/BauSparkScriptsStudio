using System;
using System.Threading.Tasks;

namespace Bau.Libraries.BauMvvm.ViewModels.Controllers
{
	/// <summary>
	///		Controlador general para las aplicaciones que funcionen mostrando los mensajes de sistema de forma asíncrona
	/// </summary>
    public interface IHostSystemControllerAsync
	{
		/// <summary>
		///		Muestra una ventana con un mensaje
		/// </summary>
		Task ShowMessageAsync(string message);

		/// <summary>
		///		Muestra una ventana con una pregunta
		/// </summary>
		Task<bool> ShowQuestionAsync(string message, string acceptTitle = "Aceptar", string cancelTitle = "Cancelar");

		/// <summary>
		///		Muestra una notificación
		/// </summary>
		Task ShowNotificationAsync(SystemControllerEnums.NotificationType type, string title, string message, TimeSpan expiration, 
								   string urlImage = null);

		/// <summary>
		///		Muestra una pregunta con tres posibles respuestas
		/// </summary>
		Task<SystemControllerEnums.ResultType> ShowQuestionCancelAsync(string message);

		/// <summary>
		///		Muestra un cuadro de diálogo para introducir un texto
		/// </summary>
		Task<(SystemControllerEnums.ResultType result, string input)> ShowInputStringAsync(string message, string defaultValue = null);

		/// <summary>
		///		Muestra un cuadro de diálogo para introducir un texto multilínea
		/// </summary>
		Task<(SystemControllerEnums.ResultType result, string input)> ShowInputMultilineStringAsync(string message, string defaultValue = null);
	}
}
