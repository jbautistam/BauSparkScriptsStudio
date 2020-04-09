using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		ViewModel con los datos de ejecución de conexiones
	/// </summary>
	public class ConnectionExecutionViewModel : BaseObservableObject
	{
		// Cobmo de conexiones
		private ComboViewModel _connections;
		private string _fileNameParameters, _shortFileName, _executionTime;
		private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor;
		private bool _isExecuting;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _cancellationToken = CancellationToken.None;
		private System.Timers.Timer _timer;
		private System.Diagnostics.Stopwatch _stopwatch;

		public ConnectionExecutionViewModel(SolutionViewModel solutionViewModel)
		{
			// Asigna la solución
			SolutionViewModel = solutionViewModel;
			// Asigna los comandos
			ExecuteScripCommand = new BaseCommand(async _ => await ExecuteScriptAsync(), _ => !IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			CancelScriptExecutionCommand = new BaseCommand(_ => CancelScriptExecution(), _ => IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			OpenParametersFileCommand = new BaseCommand(_ => OpenParametersFile());
			RemoveParametersFileCommand = new BaseCommand(_ => FileNameParameters = string.Empty);
		}

		/// <summary>
		///		Inicializa el viewModel (cuando ya se ha cargado la solución)
		/// </summary>
		public void Initialize()
		{
			// Carga el nombre del archivo de parámetros de la solución
			if (System.IO.File.Exists(SolutionViewModel.Solution.LastParametersFileName))
				FileNameParameters = SolutionViewModel.Solution.LastParametersFileName;
			else
				FileNameParameters = string.Empty;
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
		///		Ejecuta un script
		/// </summary>
		private async Task ExecuteScriptAsync()
		{
			if (IsExecuting)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Ya se está ejecutando una consulta");
			else
			{
				ConnectionModel connection = GetSelectedConnection();

					if (connection == null)
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
					else
					{
						(Application.Connections.Models.ArgumentListModel arguments, string error) = GetParameters();

							// Si ha podido cargar el archivo de parámetros, ejecuta el script
							if (!string.IsNullOrWhiteSpace(error))
								SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
							else
							{
								// Inicializa el temporizador
								_timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(500).TotalMilliseconds);
								_stopwatch = new System.Diagnostics.Stopwatch();
								// Indica que se está ejecutando una tarea y arranca el temporizador
								IsExecuting = true;
								_timer.Elapsed += (sender, args) => ExecutionTime = _stopwatch.Elapsed.ToString();
								_timer.Start();
								_stopwatch.Start();
								ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Red;
								// Obtiene el token de cancelación
								_tokenSource = new CancellationTokenSource();
								_cancellationToken = _tokenSource.Token;
								// Ejecuta la tarea
								try
								{
									await SolutionViewModel.MainViewModel.ExecuteScriptAsync(connection, arguments, _cancellationToken);
								}
								catch (Exception exception)
								{
									SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al ejecutar la consulta. {exception.Message}");
								}
								// Indica que ha finalizado la tarea y detiene el temporizador
								StopExecuting();
							}
					}
			}
		}

		/// <summary>
		///		Detiene la ejecución
		/// </summary>
		private void StopExecuting()
		{
			// Detiene los temporizadores
			_timer.Stop();
			_stopwatch.Stop();
			// Indica que ya no es está ejecutando
			ExecutionTime = _stopwatch.Elapsed.ToString();
			ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Black;
			IsExecuting = false;
			// Vacía el token de cancelación
			_cancellationToken = CancellationToken.None;
			// Log
			SolutionViewModel.MainViewModel.MainController.Logger.Flush();
		}

		/// <summary>
		///		Cancela la ejecución del script
		/// </summary>
		private void CancelScriptExecution()
		{
			if (IsExecuting && _cancellationToken != null && _cancellationToken != CancellationToken.None)
			{
				if (_cancellationToken.CanBeCanceled)
				{
					// Cancela las tareas
					_tokenSource.Cancel();
					// Log
					SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error("Consulta cancelada");
					// Indica que ya no está en ejecución
					StopExecuting();
				}
			}
		}

		/// <summary>
		///		Obtiene los parámetros de un archivo
		/// </summary>
		internal (Application.Connections.Models.ArgumentListModel arguments, string error) GetParameters()
		{
			Application.Connections.Models.ArgumentListModel arguments = new Application.Connections.Models.ArgumentListModel();
			string error = string.Empty;

				// Carga los parámetros si es necesario
				if (!string.IsNullOrWhiteSpace(FileNameParameters))
				{
					if (!System.IO.File.Exists(FileNameParameters))
						error = "No se encuentra el archivo de parámetros";
					else 
						try
						{
							System.Data.DataTable table = new LibJsonConversor.JsonToDataTableConversor().ConvertToDataTable(LibHelper.Files.HelperFiles.LoadTextFile(FileNameParameters));

								// Crea la colección de parámetros a partir de la tabla
								if (table.Rows.Count == 0)
									error = "No se ha encontrado ningún parámetro en el archivo";
								else
									foreach (System.Data.DataColumn column in table.Columns)
									{
										if (column.ColumnName.StartsWith("Constant.", StringComparison.CurrentCultureIgnoreCase))
											arguments.Constants.Add(column.ColumnName.Substring("Constant.".Length), table.Rows[0][column.Ordinal]);
										else
											arguments.Parameters.Add(column.ColumnName, table.Rows[0][column.Ordinal]);
									}
						}
						catch (Exception exception)
						{
							error = $"Error cuando se cargaba el archivo de parámetros. {exception.Message}";
						}
				}
				// Devuelve el resultado
				return (arguments, error);
		}

		/// <summary>
		///		Abre el archivo de parámetros
		/// </summary>
		private void OpenParametersFile()
		{
			string fileName = SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogLoad
									(SolutionViewModel.MainViewModel.LastPathSelected, "Archivo json (*.json)|*.json|Todos los archivos (*.*)|*.*", null, "*.json");

				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
				{
					// Guarda el nombre de archivo en los parámetros
					FileNameParameters = fileName;
					// Guarda el nombre de archivo en la solución
					SolutionViewModel.Solution.LastParametersFileName = fileName;
					SolutionViewModel.MainViewModel.SaveSolution();
					// Cambia el último directorio seleccionado
					SolutionViewModel.MainViewModel.LastPathSelected = System.IO.Path.GetDirectoryName(FileNameParameters);
				}
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

		/// <summary>
		///		Nombre del archivo de parámetros
		/// </summary>
		public string FileNameParameters
		{
			get { return _fileNameParameters; }
			set 
			{ 
				if (CheckProperty(ref _fileNameParameters, value))
				{
					if (string.IsNullOrWhiteSpace(value))
						ShortFileName = string.Empty;
					else
						ShortFileName = System.IO.Path.GetFileName(value);
				}
			}
		}

		/// <summary>
		///		Nombre corto del archivo de parámetros
		/// </summary>
		public string ShortFileName
		{
			get { return _shortFileName; }
			set { CheckProperty(ref _shortFileName, value); }
		}

		/// <summary>
		///		Indica si se está ejecutando una tarea
		/// </summary>
		public bool IsExecuting
		{
			get { return _isExecuting; }
			set { CheckProperty(ref _isExecuting, value); }
		}

		/// <summary>
		///		Tiempo de ejecución de la última consulta
		/// </summary>
		public string ExecutionTime
		{
			get { return _executionTime; }
			set { CheckProperty(ref _executionTime, value); }
		}
		
		/// <summary>
		///		Color del texto del tiempo de ejecución
		/// </summary>
		public BauMvvm.ViewModels.Media.MvvmColor ExecutionTimeColor
		{
			get { return _executionTimeColor; }
			set { CheckObject(ref _executionTimeColor, value); }
		}

		/// <summary>
		///		Comando de ejecución de un script
		/// </summary>
		public BaseCommand ExecuteScripCommand { get; }

		/// <summary>
		///		Comando para cancelar la ejecución de un script
		/// </summary>
		public BaseCommand CancelScriptExecutionCommand { get; }

		/// <summary>
		///		Comando para abrir un archivo de parámetros
		/// </summary>
		public BaseCommand OpenParametersFileCommand { get; }

		/// <summary>
		///		Comando para quitar el archivo de parámetros
		/// </summary>
		public BaseCommand RemoveParametersFileCommand { get; }
	}
}
