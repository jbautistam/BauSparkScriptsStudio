using System;
using System.Windows;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections;

namespace Bau.SparkScripts.Studio.Views.Connections
{
	/// <summary>
	///		Vista para mostrar los datos de una conexión
	/// </summary>
	public partial class ConnectionView : Window
	{
		public ConnectionView(ConnectionViewModel pathViewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = pathViewModel;
			ViewModel.Close += (sender, eventArgs) => 
									{
										DialogResult = eventArgs.IsAccepted; 
										Close();
									};
		}

		/// <summary>
		///		ViewModel de la conexión
		/// </summary>
		public ConnectionViewModel ViewModel { get; }
	}
}
