﻿using System;
using System.Data;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para visualización de archivos utilizando dataTable
	/// </summary>
	public abstract class BaseFileViewModel : BaseObservableObject, IDetailViewModel
	{
		// Variables privadas
		private string _header, _fileName, _formattedPage;
		private int _actualPage, _pages, _records, _recordsPerPage;
		private DataTable _dataResults;

		protected BaseFileViewModel(SolutionViewModel solutionViewModel, string fileName, string exportFilesExtensions) : base(false)
		{
			// Asigna las propiedades
			SolutionViewModel = solutionViewModel;
			ExportFilesExtensions =	exportFilesExtensions;
			FileName = fileName;
			ActualPage = 1;
			Pages = 0;
			Records = 0;
			RecordsPerPage = 10_000;
			// Asigna los comandos
			NextPageCommand = new BaseCommand(_ => GoToPage(ActualPage + 1), _ => ActualPage < Pages)
										.AddListener(this, nameof(Records))
										.AddListener(this, nameof(RecordsPerPage));
			PreviousPageCommand = new BaseCommand(_ => GoToPage(ActualPage - 1), _ => ActualPage > 1)
										.AddListener(this, nameof(Records))
										.AddListener(this, nameof(RecordsPerPage));
			FirstPageCommand = new BaseCommand(_ => GoToPage(1), _ => ActualPage > 1)
										.AddListener(this, nameof(Records))
										.AddListener(this, nameof(RecordsPerPage));
			LastPageCommand = new BaseCommand(_ => GoToPage(Pages), _ => ActualPage < Pages)
										.AddListener(this, nameof(Records))
										.AddListener(this, nameof(RecordsPerPage));
			FilePropertiesCommand = new BaseCommand(_ => OpenFileProperties());
		}

		/// <summary>
		///		Carga el archivo
		/// </summary>
		public void LoadFile()
		{
			int totalRecords = 0;

				// Carga el archivo
				try
				{
					DataResults = LoadFile(out totalRecords);
				}
				catch (Exception exception)
				{
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"Error when load {FileName}{Environment.NewLine}{exception.Message}");
				}
				// Asigna el número de registros
				Records = totalRecords;
				Pages = totalRecords / RecordsPerPage + 1;
		}

		/// <summary>
		///		Carga un archivo y obtiene una tabla paginada
		/// </summary>
		protected abstract DataTable LoadFile(out int totalRecords);

		/// <summary>
		///		Abre las propiedades del archivo
		/// </summary>
		protected abstract void OpenFileProperties();

		/// <summary>
		///		Calcula el texto de las páginas formateadas
		/// </summary>
		private void ComputeFormattedPage()
		{
			FormattedPage = $"{ActualPage:#,##0} / {Pages:#,##0} - {Records:#,##0}";
		}

		/// <summary>
		///		Carga la siguiente página
		/// </summary>
		private void GoToPage(int newPage)
		{
			ActualPage = newPage;
			LoadFile();
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			if (DataResults == null || DataResults.Rows.Count == 0)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No hay datos para exportar");
			else
			{
				string fileName = SolutionViewModel.MainViewModel.OpenDialogSave($"New file.{ExportFilesExtensions}", 
																				 $"Archivos {ExportFilesExtensions} (*.{ExportFilesExtensions})|*.{ExportFilesExtensions}" +
																					"|Todos los archivos (*.*)|*.*", 
																				 $".{ExportFilesExtensions}");

					if (!string.IsNullOrEmpty(fileName))
						using (LibLogger.Models.Log.BlockLogModel block = SolutionViewModel.MainViewModel.Manager.Logger.Default.
																				CreateBlock(LibLogger.Models.Log.LogModel.LogType.Debug,
																							$"Comienzo de grabación del archivo {fileName}"))
						{
							// Graba el archivo
							try
							{
								// Graba los datos en el archivo
								SaveFile(fileName);
								// Log
								block.Debug("Fin de grabación del archivo");
							}
							catch (Exception exception)
							{
								block.Error($"Error al grabar el archivo {fileName}. {exception.Message}");
								SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"Error al grabar el archivo {fileName}. {exception.Message}");
							}
							// Log
							SolutionViewModel.MainViewModel.MainController.Logger.Flush();
						}
			}
		}

		/// <summary>
		///		Graba el archivo como CSV
		/// </summary>
		protected abstract void SaveFile(string fileName);

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Máscara de grabación de archivos
		/// </summary>
		public string ExportFilesExtensions { get; }

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
			get { return GetType().ToString() + "_" + FileName; } 
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
						Header = "Parquet";
				}
			}
		}

		/// <summary>
		///		Página actual
		/// </summary>
		public int ActualPage
		{
			get { return _actualPage; }
			set 
			{ 
				if (CheckProperty(ref _actualPage, value))
					ComputeFormattedPage(); 
			}
		}

		/// <summary>
		///		Número de página
		/// </summary>
		public int Pages
		{
			get { return _pages; }
			set 
			{ 
				if (CheckProperty(ref _pages, value))
					ComputeFormattedPage(); 
			}
		}

		/// <summary>
		///		Número total de registros
		/// </summary>
		public int Records
		{
			get { return _records; }
			set 
			{ 
				if (CheckProperty(ref _records, value))
					ComputeFormattedPage();
			}
		}

		/// <summary>
		///		Registros por página
		/// </summary>
		public int RecordsPerPage
		{
			get { return _recordsPerPage; }
			set 
			{ 
				if (CheckProperty(ref _recordsPerPage, value))
					ComputeFormattedPage();
			}
		}

		/// <summary>
		///		Número de páginas formateado
		/// </summary>
		public string FormattedPage
		{
			get { return _formattedPage; }
			set { CheckProperty(ref _formattedPage, value); }
		}

		/// <summary>
		///		Comando para ir a la siguiente página
		/// </summary>
		public BaseCommand NextPageCommand { get; }

		/// <summary>
		///		Comando para ir a la página anterior
		/// </summary>
		public BaseCommand PreviousPageCommand { get; }

		/// <summary>
		///		Comando para ir a la primera página
		/// </summary>
		public BaseCommand FirstPageCommand { get; }

		/// <summary>
		///		Comando para ir a la última página
		/// </summary>
		public BaseCommand LastPageCommand { get; }

		/// <summary>
		///		Comando para ver las propiedades de un archivo
		/// </summary>
		public BaseCommand FilePropertiesCommand { get; }
	}
}
