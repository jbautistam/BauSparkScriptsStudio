using System;
using System.Windows.Controls;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files;

namespace Bau.SparkScripts.Studio.Views.Files
{
	/// <summary>
	///		Ventana para mostrar el contenido de un archivo
	/// </summary>
	public partial class FileDetailsView : UserControl
	{
		public FileDetailsView(FileViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			if (ViewModel != null && !string.IsNullOrWhiteSpace(ViewModel.FileName) && !IsLoadedViewModel)
			{
				// Asigna el nombre de archivo
				udtEditor.FileName = ViewModel.FileName;
				// Carga el texto
				try
				{
					ViewModel.Load();
					udtEditor.Text = ViewModel.Content; 
				}
				catch (Exception exception)
				{
					udtEditor.IsEnabled = false;
					ViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"Error al abrir el archivo {exception.Message}");
				}
				// Indica que no ha habido modificaciones y que se ha cargado el archivo, si no se lanza
				ViewModel.IsUpdated = false;
				IsLoadedViewModel = true;
			}
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public FileViewModel ViewModel { get; }

		/// <summary>
		///		Indica si se ha cargado el archivo de ViewModel una vez (a usercontrol_loaded se llama cada vez que cambia de ficha)
		/// </summary>
		public bool	IsLoadedViewModel { get; private set; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}

		private void udtEditor_TextChanged(object sender, EventArgs e)
		{
			ViewModel.Content = udtEditor.Text;
			ViewModel.IsUpdated = true;
		}
	}
}
