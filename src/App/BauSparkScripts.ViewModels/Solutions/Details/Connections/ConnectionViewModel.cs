﻿using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		ViewModel de datos de conexión
	/// </summary>
	public class ConnectionViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _name, _description, _connectionString;
		private string _server, _user, _password, _database;
		private int _port;
		private bool _useIntegratedSecurity, _isServerConnection;
		private int _timeoutExecuteScriptMinutes;
		private bool _isNew;
		private ComboViewModel _comboTypes;

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
			// Carga el combo de tipos
			LoadComboTypes();
			// Asigna las propiedades
			Name = Connection.Name;
			if (string.IsNullOrWhiteSpace(Name))
				Name = "Nueva conexión";
			Description = Connection.Description;
			ComboTypes.SelectedID = (int) Connection.Type;
			Server = Connection.Parameters[nameof(Server)];
			Port = Connection.Parameters[nameof(Port)].GetInt(1433);
			User = Connection.Parameters[nameof(User)];
			Password = Connection.Parameters[nameof(Password)];
			Database = Connection.Parameters[nameof(Database)];
			UseIntegratedSecurity = Connection.Parameters[nameof(UseIntegratedSecurity)].GetBool();
			ConnectionString = Connection.Parameters[nameof(ConnectionString)];
			TimeoutExecuteScriptMinutes = (int) Connection.TimeoutExecuteScript.TotalMinutes;
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Carga el combo de tipos
		/// </summary>
		private void LoadComboTypes()
		{
			// Crea el combo
			ComboTypes = new ComboViewModel(this);
			// Añade los elementos
			ComboTypes.AddItem(null, "<Seleccione un tipo de conexión>");
			ComboTypes.AddItem((int) ConnectionModel.ConnectionType.Spark, "Spark");
			ComboTypes.AddItem((int) ConnectionModel.ConnectionType.SqlServer, "Sql server");
			ComboTypes.AddItem((int) ConnectionModel.ConnectionType.Odbc, "Odbc");
			// Selecciona el primer elemento
			ComboTypes.SelectedItem = ComboTypes.Items[0];
			// Asigna el manejador de eventos
			ComboTypes.PropertyChanged += (sender, args) =>
												{
													if (args.PropertyName.Equals(nameof(ComboTypes.SelectedItem)))
														IsServerConnection = ComboTypes.SelectedID == (int) ConnectionModel.ConnectionType.SqlServer;
												};
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
				else if (ComboTypes.SelectedID == null)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un tipo");
				else if (ValidateConnection())
					validated = true;
				// Devuelve el valor que indica si los datos son correctos
				return validated;
		}

		/// <summary>
		///		Comprueba la conexión
		/// </summary>
		private bool ValidateConnection()
		{
			bool validated = false;

				// Comprueba los datos
				if (IsServerConnection)
				{
					if (string.IsNullOrWhiteSpace(Server))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca la dirección del servidor");
					else if (!UseIntegratedSecurity && string.IsNullOrWhiteSpace(User))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el código de usuario");
					else if (!UseIntegratedSecurity && string.IsNullOrWhiteSpace(Password))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca la contraseña");
					else if (string.IsNullOrWhiteSpace(Database))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la base de datos");
					else
						validated = true;
				}
				else
				{
					if (string.IsNullOrWhiteSpace(ConnectionString))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca la cadena de conexión");
					else
						validated = true;
				}
				// Devuelve el valor que indica si los datos son correctos
				return validated;
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
				Connection.Type = (ConnectionModel.ConnectionType) (ComboTypes.SelectedID ?? 0);
				Connection.Parameters[nameof(Server)] = Server;
				Connection.Parameters[nameof(Port)] = Port.ToString();
				Connection.Parameters[nameof(User)] = User;
				Connection.Parameters[nameof(Password)] = Password;
				Connection.Parameters[nameof(Database)] = Database;
				Connection.Parameters[nameof(UseIntegratedSecurity)] = UseIntegratedSecurity.ToString();
				Connection.Parameters[nameof(ConnectionString)] = ConnectionString;
				Connection.TimeoutExecuteScript = TimeSpan.FromMinutes(TimeoutExecuteScriptMinutes);
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
		///		Tipos de conexión
		/// </summary>
		public ComboViewModel ComboTypes
		{
			get { return _comboTypes; }
			set { CheckObject(ref _comboTypes, value); }
		}

		/// <summary>
		///		Indica si es un tipo de conexión a servidor
		/// </summary>
		public bool IsServerConnection
		{
			get { return _isServerConnection; }
			set { CheckProperty(ref _isServerConnection, value); }
		}

		/// <summary>
		///		Servidor
		/// </summary>
		public string Server
		{
			get { return _server; }
			set { CheckProperty(ref _server, value); }
		}

		/// <summary>
		///		Puerto
		/// </summary>
		public int Port
		{
			get { return _port; }
			set { CheckProperty(ref _port, value); }
		}

		/// <summary>
		///		Usuario
		/// </summary>
		public string User
		{
			get { return _user; }
			set { CheckProperty(ref _user, value); }
		}

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password
		{
			get { return _password; }
			set { CheckProperty(ref _password, value); }
		}

		/// <summary>
		///		Base de datos
		/// </summary>
		public string Database
		{
			get { return _database; }
			set { CheckProperty(ref _database, value); }
		}

		/// <summary>
		///		Indica si se debe utilizar seguridad integrada
		/// </summary>
		public bool UseIntegratedSecurity
		{
			get { return _useIntegratedSecurity; }
			set { CheckProperty(ref _useIntegratedSecurity, value); }
		}

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public string ConnectionString
		{
			get { return _connectionString; }
			set { CheckProperty(ref _connectionString, value); }
		}

		/// <summary>
		///		Minutos de timeout para la ejecución de scripts
		/// </summary>
		public int TimeoutExecuteScriptMinutes
		{
			get { return _timeoutExecuteScriptMinutes; }
			set { CheckProperty(ref _timeoutExecuteScriptMinutes, value); }
		}
	}
}