using System;
using System.Threading.Tasks;
using System.Windows;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.BauMvvm.Views.Controllers
{
	/// <summary>
	///		Controlador de ventanas comunes
	/// </summary>
	public class HostSystemController: IHostSystemController
	{
		public HostSystemController(string applicationName, Window mainWindow)
		{
			ApplicationName = applicationName;
			MainWindow = mainWindow;
		}

		/// <summary>
		///		Muestra un cuadro de mensaje
		/// </summary>
		public void ShowMessage(string message)
		{
			MainWindow.Dispatcher.Invoke(new Action(() => MessageBox.Show(MainWindow, message, ApplicationName)),
										 System.Windows.Threading.DispatcherPriority.Normal);
		}

		/// <summary>
		///		Muestra un cuadro de mensaje
		/// </summary>
		public bool ShowQuestion(string message, string acceptTitle = "Aceptar", string cancelTitle = "Cancelar")
		{
			return MessageBox.Show(MainWindow, message, ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
		}

		/// <summary>
		///		Muestra un cuadro de mensaje para introducir un texto
		/// </summary>
		public SystemControllerEnums.ResultType ShowInputString(string message, ref string input)
		{
			SystemControllerEnums.ResultType type;
			Forms.Dialogs.InputBoxView view = new Forms.Dialogs.InputBoxView(this, message, input);

				// Muestra el cuadro de diálogo
				type = new HostDialogsController(ApplicationName, MainWindow).ShowDialog(MainWindow, view);
				// Si se ha aceptado, recoge el texto
				if (type == SystemControllerEnums.ResultType.Yes)
					input = view.InputText;
				// Devuelve el resultado
				return type;
		}

		/// <summary>
		///		Muestra una pregunta con tres posibles respuestas
		/// </summary>
		public SystemControllerEnums.ResultType ShowQuestionCancel(string message)
		{
			switch (MessageBox.Show(message, ApplicationName, MessageBoxButton.YesNoCancel))
			{
				case MessageBoxResult.Yes:
					return SystemControllerEnums.ResultType.Yes;
				case MessageBoxResult.No:
					return SystemControllerEnums.ResultType.No;
				default:
					return SystemControllerEnums.ResultType.Cancel;
			}
		}

		/// <summary>
		///		Muestra una notificación
		/// </summary>
		public virtual void ShowNotification(SystemControllerEnums.NotificationType type, string title, string message, TimeSpan expiration, string urlImage = null)
		{
			ShowMessage(message);
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
