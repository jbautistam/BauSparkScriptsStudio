﻿using System;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauSparkScripts.Models.Connections;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para un archivo
	/// </summary>
	public class FileViewModel : BaseObservableObject, IDetailViewModel
	{
		// Eventos públicos
		public event EventHandler<EventArguments.EditorSelectedTextRequiredEventArgs> SelectedTextRequired;
		// Variables privadas
		private string _header, _fileName, _content;
		private Connections.ComboConnectionsViewModel _comboConnectionsViewModel;

		public FileViewModel(SolutionViewModel solutionViewModel, string fileName)
		{
			// Inicializa el viewModel
			SolutionViewModel = solutionViewModel;
			FileName = fileName;
			// Inicializa los objetos
			ComboConnections = new Connections.ComboConnectionsViewModel(solutionViewModel);
		}

		/// <summary>
		///		Carga el texto del archivo
		/// </summary>
		public void Load()
		{
			if (!string.IsNullOrWhiteSpace(FileName) && System.IO.File.Exists(FileName))
				Content = LibHelper.Files.HelperFiles.LoadTextFile(FileName);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// Graba el archivo
			if (string.IsNullOrWhiteSpace(FileName) || newName)
			{
				string newFileName = SolutionViewModel.MainViewModel.OpenDialogSave(FileName, "Script SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*", ".sql");

					// Cambia el nombre de archivo si es necesario
					if (!string.IsNullOrWhiteSpace(newFileName))
						FileName = newFileName;
			}
			// Graba el archivo
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				// Graba el archivo
				LibHelper.Files.HelperFiles.SaveTextFile(FileName, Content, System.Text.Encoding.UTF8);
				// Actualiza el árbol
				SolutionViewModel.TreeFoldersViewModel.Load();
				// Indica que no ha habido modificaciones
				IsUpdated = false;
			}
		}

		/// <summary>
		///		Ejecuta el script
		/// </summary>
		internal async Task ExecuteScriptAsync(ConnectionModel connection, Application.Connections.Models.ArgumentListModel arguments,
											   System.Threading.CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(Content))
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
			else if (connection == null)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
			else 
				using (LibLogger.Models.Log.BlockLogModel block = SolutionViewModel.MainViewModel.Manager.Logger.Default.CreateBlock(LibLogger.Models.Log.LogModel.LogType.Info,
																																	 $"Comienza la ejecución de la consulta"))
				{
					string selectedText = GetEditorSelectedText();

						// Ejecuta la consulta
						if (!string.IsNullOrEmpty(selectedText))
							await SolutionViewModel.MainViewModel.Manager.ExecuteQueryAsync(connection, selectedText, arguments, 
																							connection.TimeoutExecuteScript, cancellationToken);
						else
							await SolutionViewModel.MainViewModel.Manager.ExecuteQueryAsync(connection, Content, arguments, 
																							connection.TimeoutExecuteScript, cancellationToken);
						// Muestra el tiempo de ejecución
						block.Info($"Tiempo de ejecución: {SolutionViewModel.ConnectionExecutionViewModel.ExecutionTime}");
				}
		}

		/// <summary>
		///		Lanza un evento para solicitar el texto seleccionado al editor
		/// </summary>
		private string GetEditorSelectedText()
		{
			EventArguments.EditorSelectedTextRequiredEventArgs eventArgs = new EventArguments.EditorSelectedTextRequiredEventArgs();

				// Lanza el evento
				SelectedTextRequired?.Invoke(this, eventArgs);
				// Recupera el texto
				return eventArgs.SelectedText;
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + FileName; } 
		}

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
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
						Header = "Archivo";
				}
			}
		}

		/// <summary>
		///		Contenido del archivo
		/// </summary>
		public string Content
		{
			get { return _content; }
			set { CheckProperty(ref _content, value); }
		}

		/// <summary>
		///		Combo de conexiones
		/// </summary>
		public Connections.ComboConnectionsViewModel ComboConnections
		{
			get { return _comboConnectionsViewModel; }
			set { CheckObject(ref _comboConnectionsViewModel, value); }
		}
	}
}
