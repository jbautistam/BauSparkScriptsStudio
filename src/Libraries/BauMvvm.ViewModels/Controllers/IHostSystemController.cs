using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Controllers
{
	/// <summary>
	///		Controlador general para las aplicaciones que funcionen mostrando los mensajes de sistema
	/// </summary>
    public interface IHostSystemController
	{
		/// <summary>
		///		Muestra una ventana con un mensaje
		/// </summary>
		void ShowMessage(string message);

		/// <summary>
		///		Muestra una ventana con una pregunta
		/// </summary>
		bool ShowQuestion(string message, string acceptTitle = "Aceptar", string cancelTitle = "Cancelar");

		/// <summary>
		///		Muestra una notificación
		/// </summary>
		void ShowNotification(SystemControllerEnums.NotificationType type, string title, string message, TimeSpan expiration, string urlImage = null);

		/// <summary>
		///		Muestra una pregunta con tres posibles respuestas
		/// </summary>
		SystemControllerEnums.ResultType ShowQuestionCancel(string message);

		/// <summary>
		///		Muestra un cuadro de diálogo para introducir un texto
		/// </summary>
		SystemControllerEnums.ResultType ShowInputString(string message, ref string input);

		/// <summary>
		///		Muestra un cuadro de diálogo para introducir un texto multilínea
		/// </summary>
		SystemControllerEnums.ResultType ShowInputMultilineString(string message, ref string input);
	}
}
