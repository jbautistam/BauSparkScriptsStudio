using System;
using System.Data;

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
		private string _header, _fileName, _query, _executionTime, _errorMessage;
		private DataTable _dataResults;
		private ComboConnectionsViewModel _comboConnectionsViewModel;
		private bool _hasError;

		public ExecuteQueryViewModel(SolutionViewModel solutionViewModel, string query) : base(false)
		{
			// Asigna las propiedades
			SolutionViewModel = solutionViewModel;
			Query = query;
			Header = "Nueva consulta";
			// Carga el combo de conexiones
			ComboConnections = new ComboConnectionsViewModel(solutionViewModel);
			// Asigna los comandos
			ProcessCommand = new BaseCommand(_ => ExecuteQuery());
			ExportCommand = new BaseCommand(_ => Export(), _ => DataResults?.Rows.Count > 0)
										.AddListener(this, nameof(DataResults));
		}

		/// <summary>
		///		Ejecuta la consulta
		/// </summary>
		private void ExecuteQuery()
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
						System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

							// Limpia los datos
							DataResults = null;
							ErrorMessage = string.Empty;
							// Inicia el temporizador
							stopwatch.Start();
							// Ejecuta la consulta
							try
							{
								DataResults = SolutionViewModel.MainViewModel.Manager.GetDatatableQuery(connection, Query, TimeSpan.FromMinutes(5));
							}
							catch (Exception exception)
							{
								ErrorMessage = $"Error al ejecutar la consulta. {exception.Message}";
							}
							// Detiene el temporizador y ajusta el tiempo
							stopwatch.Stop();
							ExecutionTime = $"{stopwatch.Elapsed}";
					}
			}
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
				SolutionViewModel.TreeConnectionsViewModel.Load();
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
		///		Tiempo de ejecución
		/// </summary>
		public string ExecutionTime
		{
			get { return _executionTime; }
			set { CheckProperty(ref _executionTime, value); }
		}

		/// <summary>
		///		Mensaje de error
		/// </summary>
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set 
			{ 
				if (CheckProperty(ref _errorMessage, value))
					HasError = !string.IsNullOrEmpty(value);
			}
		}

		/// <summary>
		///		Indica si ha habido algún error al procesar la consulta
		/// </summary>
		public bool HasError
		{
			get { return _hasError; }
			set { CheckProperty(ref _hasError, value); }
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
		///		Comando para ejecutar la consulta
		/// </summary>
		public BaseCommand ProcessCommand { get; }

		/// <summary>
		///		Comando para grabar el resultado de una consulta
		/// </summary>
		public BaseCommand ExportCommand { get; }
	}
}
