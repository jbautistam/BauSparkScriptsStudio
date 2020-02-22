using System;

using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		ViewModel de datos de conexión
	/// </summary>
	public class ConnectionViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel, IDetailViewModel
	{
		// Variables privadas
		private string _header, _name, _description, _connectionString;
		private bool _isNew;

		public ConnectionViewModel(SolutionViewModel solutionViewModel, ConnectionModel connection)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			IsNew = connection == null;
			Connection = connection ?? new ConnectionModel(solutionViewModel.Solution);
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			Name = Connection.Name;
			if (string.IsNullOrWhiteSpace(Name))
				Name = "Nueva conexión";
			Description = Connection.Description;
			ConnectionString = Connection.ConnectionString;
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos introducidos
				if (string.IsNullOrWhiteSpace(Name))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la conexión");
				else if (string.IsNullOrWhiteSpace(ConnectionString))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca la cadena de conexión");
				else
					validated = true;
				// Devuelve el valor que indica si los datos son correctos
				return validated;
		}

		/// <summary>
		///		Graba los datos de la conexión
		/// </summary>
		public void SaveDetails(bool newName)
		{
			Save();
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected override void Save()
		{
			if (ValidateData())
			{
				// Asigna los datos al proyecto
				Connection.Name = Name;
				Connection.Description = Description;
				Connection.ConnectionString = ConnectionString;
				// Añade la conexión a la solución si es necesario
				if (IsNew)
					SolutionViewModel.Solution.Connections.Add(Connection);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Indica que ya no es nuevo y está grabado
				IsNew = false;
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Datos de conexión
		/// </summary>
		public ConnectionModel Connection { get; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + Connection.GlobalId; } 
		}

		/// <summary>
		///		Indica si es nuevo
		/// </summary>
		public bool IsNew
		{
			get { return _isNew; }
			set { CheckProperty(ref _isNew, value); }
		}

		/// <summary>
		///		Nombre
		/// </summary>
		public string Name 
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
		}

		/// <summary>
		///		Descripción
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { CheckProperty(ref _description, value); }
		}

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public string ConnectionString
		{
			get { return _connectionString; }
			set { CheckProperty(ref _connectionString, value); }
		}
	}
}