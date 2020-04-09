using System;
using System.Windows;

namespace Bau.Libraries.BauMvvm.Views.Forms.Dialogs
{
	/// <summary>
	///		Ventana para mostrar un texto (de una sola línea)
	/// </summary>
	public partial class InputBoxMonolineView : Window
	{
		public InputBoxMonolineView(Controllers.HostSystemController controller, string message, string inputText)
		{ 
			// Inicializa los componentes
			InitializeComponent();
			// Inicializa las propiedades
			Controller = controller;
			Message = message;
			InputText = inputText;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			lblMessage.Text = Message;
			txtInput.Text = InputText;
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validate = false;

				// Comprueba los datos
				if (string.IsNullOrWhiteSpace(txtInput.Text))
					Controller.ShowMessage("Introduzca el texto");
				else
					validate = true;
				// Devuelve el valor que indica si los datos son correctos
				return validate;
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		private void Save()
		{
			if (ValidateData())
			{ 
				// Asigna el texto
				InputText = txtInput.Text;
				// Cierra el formulario
				DialogResult = true;
				Close();
			}
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.HostSystemController Controller { get; }

		/// <summary>
		///		Mensaje
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		///		Texto introducido por el usuario
		/// </summary>
		public string InputText { get; set; }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitForm();
		}

		private void cmdSave_Click(object sender, RoutedEventArgs e)
		{
			Save();
		}
	}
}
