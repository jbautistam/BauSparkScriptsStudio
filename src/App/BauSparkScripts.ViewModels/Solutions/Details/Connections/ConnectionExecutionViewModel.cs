using System;
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

		public ConnectionExecutionViewModel(SolutionViewModel solutionViewModel)
		{
			// Asigna la solución
			SolutionViewModel = solutionViewModel;
			// Asigna los comandos
			ExecuteScripCommand = new BaseCommand(async _ => await ExecuteScriptAsync(), _ => GetSelectedConnection() != null && !IsExecuting)
											.AddListener(this, nameof(ComboViewModel.SelectedItem))
											.AddListener(this, nameof(FileNameParameters))
											.AddListener(this, nameof(IsExecuting));
			OpenParametersFileCommand = new BaseCommand(_ => OpenParametersFile());
			RemoveParametersFileCommand = new BaseCommand(_ => FileNameParameters = string.Empty);
		}

		/// <summary>
		///		Inicialize el viewModel (cuando ya se ha cargado la solución)
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
			if (GetSelectedConnection() == null)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
			else
			{
				(LibDataStructures.Collections.NormalizedDictionary<object> parameters, string error) = GetParameters();

					// Si ha podido cargar el archivo de parámetros, ejecuta el script
					if (!string.IsNullOrWhiteSpace(error))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
					else
					{
						System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(500).TotalMilliseconds);
						System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

							// Indica que se está ejecutando una tarea y arranca el temporizador
							IsExecuting = true;
							timer.Elapsed += (sender, args) => ExecutionTime = stopwatch.Elapsed.ToString();
							timer.Start();
							stopwatch.Start();
							ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Red;
							// Ejecuta la tarea
							try
							{
								await SolutionViewModel.MainViewModel.ExecuteScriptAsync(GetSelectedConnection(), parameters);
							}
							catch (Exception exception)
							{
								SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al ejecutar la consulta. {exception.Message}");
							}
							SolutionViewModel.MainViewModel.MainController.Logger.Flush();
							// Indica que ha finalizado la tarea y detiene el temporizador
							timer.Stop();
							stopwatch.Stop();
							ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Black;
							IsExecuting = false;
					}
			}
		}

		/// <summary>
		///		Obtiene los parámetros de un archivo
		/// </summary>
		internal (LibDataStructures.Collections.NormalizedDictionary<object> parameters, string error) GetParameters()
		{
			LibDataStructures.Collections.NormalizedDictionary<object> parameters = new LibDataStructures.Collections.NormalizedDictionary<object>();
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
										parameters.Add(column.ColumnName, table.Rows[0][column.Ordinal]);
						}
						catch (Exception exception)
						{
							error = $"Error cuando se cargaba el archivo de parámetros. {exception.Message}";
						}
				}
				// Devuelve el resultado
				return (parameters, error);
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
					SolutionViewModel.MainViewModel.SaveSolution(false);
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
		///		Comando para abrir un archivo de parámetros
		/// </summary>
		public BaseCommand OpenParametersFileCommand { get; }

		/// <summary>
		///		Comando para quitar el archivo de parámetros
		/// </summary>
		public BaseCommand RemoveParametersFileCommand { get; }
	}
}
