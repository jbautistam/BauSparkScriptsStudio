using System;
using System.Windows.Controls;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections;

namespace Bau.SparkScripts.Studio.Views.Connections
{
	/// <summary>
	///		Ventana para ejecutar una consulta
	/// </summary>
	public partial class ExecuteQueryView : UserControl
	{
		public ExecuteQueryView(ExecuteQueryViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			if (ViewModel != null)
			{
				// Asigna el nombre de archivo
				udtEditor.Text = ViewModel.Query;
				udtEditor.ChangeHighLightByExtension(".sql");
				// Indica que no ha habido modificaciones
				ViewModel.IsUpdated = false;
			}
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ExecuteQueryViewModel ViewModel { get; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}

		private void udtEditor_TextChanged(object sender, EventArgs e)
		{
			ViewModel.Query = udtEditor.Text;
		}
	}
}
