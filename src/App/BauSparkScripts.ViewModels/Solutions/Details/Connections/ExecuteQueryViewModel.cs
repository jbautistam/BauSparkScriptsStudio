using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		ViewModel para ejecución de una consulta
	/// </summary>
	public class ExecuteQueryViewModel : BaseObservableObject, IDetailViewModel
	{
		// Variables privadas
		private string _header, _fileName, _query, _lastQuery, _executionTime;
		private DataTable _dataResults;
		private ComboConnectionsViewModel _comboConnectionsViewModel;
		private int _actualPage, _pageSize;
		private bool _paginateQuery;
		private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor;
		private bool _isExecuting;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _cancellationToken = CancellationToken.None;
		private System.Timers.Timer _timer;
		private System.Diagnostics.Stopwatch _stopwatch;

		public ExecuteQueryViewModel(SolutionViewModel solutionViewModel, string query) : base(false)
		{
			// Asigna las propiedades
			SolutionViewModel = solutionViewModel;
			Query = query;
			Header = "Consulta";
			PaginateQuery = true;
			ActualPage = 1;
			PageSize = 10_000;
			// Carga el combo de conexiones
			ComboConnections = new ComboConnectionsViewModel(solutionViewModel);
			// Asigna los comandos
			ProcessCommand = new BaseCommand(async _ => await ExecuteQueryAsync(), _ => !IsExecuting)
									.AddListener(this, nameof(IsExecuting));
			CancelQueryCommand = new BaseCommand(_ => CancelQuery(), _ => IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			ExportCommand = new BaseCommand(_ => Export(), _ => DataResults?.Rows.Count > 0)
										.AddListener(this, nameof(DataResults));
			FirstPageCommand = new BaseCommand(async _ => await GoPageAsync(1), _ => PaginateQuery && !IsExecuting)
									.AddListener(this, nameof(IsExecuting))
									.AddListener(this, nameof(PaginateQuery));
			PreviousPageCommand = new BaseCommand(async _ => await GoPageAsync(ActualPage - 1), _ => PaginateQuery && ActualPage > 1 && !IsExecuting)
									.AddListener(this, nameof(IsExecuting))
									.AddListener(this, nameof(PaginateQuery));
			NextPageCommand = new BaseCommand(async _ => await GoPageAsync(ActualPage + 1), _ => PaginateQuery && !IsExecuting)
									.AddListener(this, nameof(IsExecuting))
									.AddListener(this, nameof(PaginateQuery));
			LastPageCommand = new BaseCommand(async _ => await GoPageAsync(ActualPage + 1), _ => PaginateQuery && !IsExecuting)
									.AddListener(this, nameof(IsExecuting))
									.AddListener(this, nameof(PaginateQuery));
		}

		/// <summary>
		///		Ejecuta la consulta
		/// </summary>
		private async Task ExecuteQueryAsync()
		{
			if (string.IsNullOrWhiteSpace(Query))
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
			else
			{
				ConnectionModel connection = ComboConnections.GetSelectedConnection();

					if (connection == null)
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
					else
					{
						(Application.Connections.Models.ArgumentListModel arguments, string error) = SolutionViewModel.ConnectionExecutionViewModel.GetParameters();

							if (!string.IsNullOrWhiteSpace(error))
								SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
							else
							{
								// Limpia los datos
								DataResults = null;
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
								// Ejecuta la consulta
								try
								{
									// Actualiza la página actual si es una consulta nueva
									if (string.IsNullOrWhiteSpace(_lastQuery) || !Query.Equals(_lastQuery, StringComparison.CurrentCultureIgnoreCase))
										ActualPage = 1;
									// Carga la consulta
									if (PaginateQuery)
										DataResults = await SolutionViewModel.MainViewModel.Manager.GetDatatableQueryAsync(connection, Query, arguments, ActualPage, PageSize, 
																														   connection.TimeoutExecuteScript, 
																														   _cancellationToken);
									else
										DataResults = await SolutionViewModel.MainViewModel.Manager.GetDatatableQueryAsync(connection, Query, arguments, 0, 0,
																														   connection.TimeoutExecuteScript, 
																														   _cancellationToken);
									// Guarda la consulta que se acaba de lanzar
									_lastQuery = Query;
								}
								catch (Exception exception)
								{
									SolutionViewModel.MainViewModel.Manager.Logger.Default.LogItems.Error($"Error al ejecutar la consulta. {exception.Message}");
								}
								// Detiene la ejecucion
								StopQuery();
							}
					}
			}
		}

		/// <summary>
		///		Detiene la ejecución
		/// </summary>
		private void StopQuery()
		{
			// Detiene los temporizadores
			_timer.Stop();
			_stopwatch.Stop();
			// Inicializa el token de cancelación
			_cancellationToken = CancellationToken.None;
			// Indica que ya no se está ejecutando
			ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Black;
			IsExecuting = false;
			// Log
			SolutionViewModel.MainViewModel.Manager.Logger.Flush();

		}

		/// <summary>
		///		Cancela la ejecución de la consulta
		/// </summary>
		private void CancelQuery()
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
					StopQuery();
				}
			}
		}

		/// <summary>
		///		Salta a una página de la consulta
		/// </summary>
		private async Task GoPageAsync(int nextPage)
		{
			ActualPage = nextPage;
			await ExecuteQueryAsync();
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// Graba el archivo
			if (string.IsNullOrWhiteSpace(FileName) || newName)
			{
				string newFileName = SolutionViewModel.MainViewModel.OpenDialogSave("New query.sql", "Script SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*", ".sql");

					// Cambia el nombre de archivo si es necesario
					if (!string.IsNullOrWhiteSpace(newFileName))
						FileName = newFileName;
			}
			// Graba el archivo
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				// Graba el archivo
				LibHelper.Files.HelperFiles.SaveTextFile(FileName, Query, System.Text.Encoding.UTF8);
				// Actualiza el árbol
				SolutionViewModel.TreeFoldersViewModel.Load();
				// Indica que no ha habido modificaciones
				IsUpdated = false;
			}
		}

		/// <summary>
		///		Exporta la tabla de datos
		/// </summary>
		private void Export()
		{
			if (DataResults == null || DataResults.Rows.Count == 0)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No hay datos para exportar");
			else
			{
				string fileName = SolutionViewModel.MainViewModel.OpenDialogSave("Query.csv", "Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*", ".csv");

					if (!string.IsNullOrEmpty(fileName))
						using (LibLogger.Models.Log.BlockLogModel block = SolutionViewModel.MainViewModel.Manager.Logger.Default.
																				CreateBlock(LibLogger.Models.Log.LogModel.LogType.Debug,
																							$"Comienzo de grabación del archivo {fileName}"))
						{
							// Graba el archivo
							try
							{
								// Graba la tabla de datos en el archivo
								new LibCsvFiles.Controllers.CsvDataTableWriter().Save(DataResults, fileName);
								// Log
								block.Debug("Fin de grabación del archivo");
							}
							catch (Exception exception)
							{
								block.Error($"Error al grabar el archivo {fileName}. {exception.Message}");
							}
							// Envía el log
							SolutionViewModel.MainViewModel.MainController.Logger.Flush();
						}
			}
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

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
			get { return GetType().ToString() + "_" + Guid.NewGuid().ToString(); } 
		}

		/// <summary>
		///		Consulta a ejecutar
		/// </summary>
		public string Query
		{
			get { return _query; }
			set { CheckProperty(ref _query, value); }
		}

		/// <summary>
		///		Resultados de la ejecución de la consulta
		/// </summary>
		public DataTable DataResults
		{ 
			get { return _dataResults; }
			set { CheckObject(ref _dataResults, value); }
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
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set 
			{ 
				if (CheckProperty(ref _fileName, value))
				{
					if (!string.IsNullOrWhiteSpace(value))
						Header = System.IO.Path.GetFileName(value);
					else
						Header = "Consulta";
				}
			}
		}

		/// <summary>
		///		ViewModel del combo de conexiones
		/// </summary>
		public ComboConnectionsViewModel ComboConnections
		{
			get { return _comboConnectionsViewModel; }
			set { CheckObject(ref _comboConnectionsViewModel, value); }
		}

		/// <summary>
		///		Indica si se debe paginar la consulta
		/// </summary>
		public bool PaginateQuery
		{
			get { return _paginateQuery; }
			set { CheckProperty(ref _paginateQuery, value); }
		}

		/// <summary>
		///		Página actual
		/// </summary>
		public int ActualPage
		{
			get { return _actualPage; }
			set { CheckProperty(ref _actualPage, value); }
		}

		/// <summary>
		///		Tamaño de página
		/// </summary>
		public int PageSize
		{
			get { return _pageSize; }
			set { CheckProperty(ref _pageSize, value); }
		}

		/// <summary>
		///		Comando para ejecutar la consulta
		/// </summary>
		public BaseCommand ProcessCommand { get; }

		/// <summary>
		///		Comando para cancelar la ejecución de un script
		/// </summary>
		public BaseCommand CancelQueryCommand { get; }

		/// <summary>
		///		Comando para grabar el resultado de una consulta
		/// </summary>
		public BaseCommand ExportCommand { get; }

		/// <summary>
		///		Comando para ver la primera página
		/// </summary>
		public BaseCommand FirstPageCommand { get; }

		/// <summary>
		///		Comando para ver la primera página
		/// </summary>
		public BaseCommand PreviousPageCommand { get; }

		/// <summary>
		///		Comando para ver la primera página
		/// </summary>
		public BaseCommand NextPageCommand { get; }

		/// <summary>
		///		Comando para ver la primera página
		/// </summary>
		public BaseCommand LastPageCommand { get; }
	}
}