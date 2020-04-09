using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		ViewModel con un combo de conexiones
	/// </summary>
	public class ComboConnectionsViewModel : BaseObservableObject
	{
		// Variables privadas
		private ComboViewModel _connections;

		public ComboConnectionsViewModel(SolutionViewModel solutionViewModel)
		{
			SolutionViewModel = solutionViewModel;
			LoadComboConnections();
		}

		/// <summary>
		///		Carga el combo de conexiones
		/// </summary>
		private void LoadComboConnections()
		{
			// Inicializa el combo
			Connections = new ComboViewModel(this);
			// Carga las conexiones
			if (SolutionViewModel.Solution.Connections.Count == 0)
				Connections.AddItem(null, "<Seleccione una conexión>", null);
			else
				foreach (ConnectionModel connection in SolutionViewModel.Solution.Connections)
					Connections.AddItem(null, connection.Name, connection);
			// Si no se ha seleccionado nada, selecciona el primer elemento
			if (Connections.SelectedItem == null)
				Connections.SelectedItem = Connections.Items[0];
		}

		/// <summary>
		///		Obtiene la conexión seleccionada en el combo
		/// </summary>
		public ConnectionModel GetSelectedConnection()
		{
			if (Connections.SelectedItem?.Tag is ConnectionModel connection)
				return connection;
			else
				return null;
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Combo de conexiones
		/// </summary>
		public ComboViewModel Connections
		{
			get { return _connections; }
			set { CheckObject(ref _connections, value); }
		}
	}
}
